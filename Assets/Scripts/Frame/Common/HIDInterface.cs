﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Win32.SafeHandles;
using System.IO;

public struct InterfaceDetails
{
	public string manufacturer;
	public string product;
	public string serialNumber;
	public ushort VID;
	public ushort PID;
	public string devicePath;
	public int IN_reportByteLength;
	public int OUT_reportByteLength;
	public ushort versionNumber;
}
public class HIDDevice
{
    #region globals
    public bool deviceConnected { get; set; }
    public SafeFileHandle handle;
    private HIDP_CAPS capabilities;
    public InterfaceDetails productInfo;
    public byte[] readData;
    #endregion

    #region static_methods

    public static InterfaceDetails[] getConnectedDevices()
    {
		InterfaceDetails[] devices = new InterfaceDetails[0];

        //Create structs to hold interface information
        SP_DEVINFO_DATA devInfo = new SP_DEVINFO_DATA();
        SP_DEVICE_INTERFACE_DATA devIface = new SP_DEVICE_INTERFACE_DATA();
        devInfo.cbSize = (uint)Marshal.SizeOf(devInfo);
        devIface.cbSize = (uint)(Marshal.SizeOf(devIface));

        Guid G = new Guid();
        HID.HidD_GetHidGuid(ref G); //Get the guid of the HID device class

        IntPtr i = SetupAPI.SetupDiGetClassDevs(ref G, IntPtr.Zero, IntPtr.Zero, SetupAPI.DIGCF_DEVICEINTERFACE | SetupAPI.DIGCF_PRESENT);

        //Loop through all available entries in the device list, until false
        SP_DEVICE_INTERFACE_DETAIL_DATA didd = new SP_DEVICE_INTERFACE_DETAIL_DATA();
        if (IntPtr.Size == 8) // for 64 bit operating systems
		{
			didd.cbSize = 8;
		}
        else
		{
			didd.cbSize = 4 + Marshal.SystemDefaultCharSize; // for 32 bit systems
		}

        int j = -1;
        bool b = true;
        SafeFileHandle tempHandle;
        while (b)
        {
			++j;
            b = SetupAPI.SetupDiEnumDeviceInterfaces(i, IntPtr.Zero, ref G, (uint)j, ref devIface);
            if (b == false)
			{
				break;
			}

            uint requiredSize = 0;
            SetupAPI.SetupDiGetDeviceInterfaceDetail(i, ref devIface, ref didd, 256, out requiredSize, ref devInfo);
            string devicePath = didd.DevicePath;

            //create file handles using CT_CreateFile
            tempHandle = Kernel32.CreateFile(devicePath, Kernel32.GENERIC_READ | Kernel32.GENERIC_WRITE, Kernel32.FILE_SHARE_READ | Kernel32.FILE_SHARE_WRITE,
                IntPtr.Zero, Kernel32.OPEN_EXISTING, 0, IntPtr.Zero);

            //get capabilites - use getPreParsedData, and getCaps
            //store the reportlengths
            IntPtr ptrToPreParsedData = new IntPtr();
            bool ppdSucsess = HID.HidD_GetPreparsedData(tempHandle, ref ptrToPreParsedData);
            if (ppdSucsess == false)
                continue;

            HIDP_CAPS capabilities = new HIDP_CAPS();
            HID.HidP_GetCaps(ptrToPreParsedData, ref capabilities);

            HIDD_ATTRIBUTES attributes = new HIDD_ATTRIBUTES();
            HID.HidD_GetAttributes(tempHandle, ref attributes);

            string productName = "";
            string SN = "";
            string manfString = "";
            IntPtr buffer = Marshal.AllocHGlobal(126);//max alloc for string; 
            if (HID.HidD_GetProductString(tempHandle, buffer, 126)) productName = Marshal.PtrToStringAuto(buffer);
            if (HID.HidD_GetSerialNumberString(tempHandle, buffer, 126)) SN = Marshal.PtrToStringAuto(buffer);
            if (HID.HidD_GetManufacturerString(tempHandle, buffer, 126)) manfString = Marshal.PtrToStringAuto(buffer);
            Marshal.FreeHGlobal(buffer);

			//Call freePreParsedData to release some stuff
			HID.HidD_FreePreparsedData(ref ptrToPreParsedData);

			//If connection was sucsessful, record the values in a global struct
			InterfaceDetails productInfo = new InterfaceDetails();
            productInfo.devicePath = devicePath;
            productInfo.manufacturer = manfString;
            productInfo.product = productName;
            productInfo.PID = (ushort)attributes.ProductID;
            productInfo.VID = (ushort)attributes.VendorID;
            productInfo.versionNumber = (ushort)attributes.VersionNumber;
            productInfo.IN_reportByteLength = (int)capabilities.InputReportByteLength;
            productInfo.OUT_reportByteLength = (int)capabilities.OutputReportByteLength;
            productInfo.serialNumber = SN;     //Check that serial number is actually a number
                
            int newSize = devices.Length + 1;
            Array.Resize(ref devices, newSize);
            devices[newSize - 1] = productInfo;
        }
        SetupAPI.SetupDiDestroyDeviceInfoList(i);

        return devices;
    }
        
    #endregion

    #region constructors
    /// <summary>
    /// Creates an object to handle read/write functionality for a USB HID device
    /// Uses one filestream for each of read/write to allow for a write to occur during a blocking
    /// asnychronous read
    /// </summary>
    /// <param name="VID">The vendor ID of the USB device to connect to</param>
    /// <param name="PID">The product ID of the USB device to connect to</param>
    /// <param name="serialNumber">The serial number of the USB device to connect to</param>
    /// <param name="useAsyncReads">True - Read the device and generate events on data being available</param>
    public HIDDevice(ushort VID, ushort PID)
    {
		InterfaceDetails[] devices = getConnectedDevices();
            
        //loop through all connected devices to find one with the correct details
        for (int i = 0; i < devices.Length; i++)
        {
            if ((devices[i].VID == VID) && (devices[i].PID == PID))
			{
				initDevice(devices[i].devicePath);
				break;
			}
        }
    }

    /// <summary>
    /// Creates an object to handle read/write functionality for a USB HID device
    /// Uses one filestream for each of read/write to allow for a write to occur during a blocking
    /// asnychronous read
    /// </summary>
    /// <param name="devicePath">The USB device path - from getConnectedDevices</param>
    /// <param name="useAsyncReads">True - Read the device and generate events on data being available</param>
    public HIDDevice(string devicePath)
    {
        initDevice(devicePath);
    }
    #endregion

    #region functions
    private void initDevice(string devicePath)
    {
        deviceConnected = false;

		//create file handles using CT_CreateFile
		handle = Kernel32.CreateFile(devicePath, Kernel32.GENERIC_READ | Kernel32.GENERIC_WRITE, Kernel32.FILE_SHARE_READ | Kernel32.FILE_SHARE_WRITE, IntPtr.Zero, Kernel32.OPEN_EXISTING, 0, IntPtr.Zero);

        //get capabilites - use getPreParsedData, and getCaps
        //store the reportlengths
        IntPtr ptrToPreParsedData = new IntPtr();
        HID.HidD_GetPreparsedData(handle, ref ptrToPreParsedData);

        capabilities = new HIDP_CAPS();
        HID.HidP_GetCaps(ptrToPreParsedData, ref capabilities);

        HIDD_ATTRIBUTES attributes = new HIDD_ATTRIBUTES();
        HID.HidD_GetAttributes(handle, ref attributes);

        string productName = "";
        string SN = "";
        string manfString = "";
        IntPtr buffer = Marshal.AllocHGlobal(126);//max alloc for string; 
        if (HID.HidD_GetProductString(handle, buffer, 126))
		{
			productName = Marshal.PtrToStringAuto(buffer);
		}
		if (HID.HidD_GetSerialNumberString(handle, buffer, 126))
		{
			SN = Marshal.PtrToStringAuto(buffer);
		}
		if (HID.HidD_GetManufacturerString(handle, buffer, 126))
		{
			manfString = Marshal.PtrToStringAuto(buffer);
		}
        Marshal.FreeHGlobal(buffer);

		//Call freePreParsedData to release some stuff
		HID.HidD_FreePreparsedData(ref ptrToPreParsedData);

        if (handle.IsInvalid)
            return;

        deviceConnected = true;

        //If connection was sucsessful, record the values in a global struct
        productInfo = new InterfaceDetails();
        productInfo.devicePath = devicePath;
        productInfo.manufacturer = manfString;
        productInfo.product = productName;
        productInfo.serialNumber = SN;
        productInfo.PID = (ushort)attributes.ProductID;
        productInfo.VID = (ushort)attributes.VendorID;
        productInfo.versionNumber = (ushort)attributes.VersionNumber;
        productInfo.IN_reportByteLength = (int)capabilities.InputReportByteLength;
        productInfo.OUT_reportByteLength = (int)capabilities.OutputReportByteLength;
    }
    public void close()
	{ 
		try
		{
			if (handle != null && !handle.IsInvalid)
			{
				//Kernel32.PurgeComm(handle, Kernel32.PURGE_TXABORT | Kernel32.PURGE_RXABORT);
				//Kernel32.CloseHandle(handle);
				handle.Close();
				handle = null;
				UnityUtility.logInfo("设备已关闭");
			}
			this.deviceConnected = false;
		}
		catch(Exception e)
		{
			UnityUtility.logInfo("exception : " + e.Message);
		}
    }
    public bool write(byte[] data)  
    {
        if (data.Length >= capabilities.OutputReportByteLength)
		{
			return false;
		}
		uint writeCount = 0;
		return Kernel32.WriteFile(handle, data, (uint)data.Length, ref writeCount, IntPtr.Zero);
	}
    public int read(ref byte[] data, int expectCount = 0)
    {
		if (data.Length < capabilities.InputReportByteLength)
		{
			return 0;
		}
		uint readCount = 0;
		Kernel32.ReadFile(handle, data, (uint)data.Length, ref readCount, IntPtr.Zero);
		// 如果读取的数量和期望的数量不一致,则丢弃数据
		if(expectCount > 0 && expectCount != readCount)
		{
			return 0;
		}
		return (int)readCount;
	}
	#endregion
}