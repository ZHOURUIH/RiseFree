using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Net.NetworkInformation;
using System.Diagnostics;
using System.IO;

public class HardwareInfo : FrameComponent
{
	public string mOriMAC;      // 网卡原生MAC地址
	public string mHDD;         // 硬盘序列号
	public string mMainbord;    // 主板序列号
	public string mCPU;         // CPU ID
	public string mBIOS;        // BIOS序列号
	public string mMainbordType;// 主板型号
	public string mCurMAC;      // 网卡当前MAC地址
	public HardwareInfo(string name)
		: base(name)
	{
		;
	}
	public override void init()
	{
		Process p = Process.Start(CommonDefine.F_HELPER_EXE_PATH + "HardwareInfo.exe", CommonDefine.F_HELPER_EXE_PATH);
		p.WaitForExit();
		// 获得文件中的字符串
		string fileBuffer = FileUtility.openTxtFile(CommonDefine.F_HELPER_EXE_PATH + "Hardware.txt");
		FileUtility.deleteFile(CommonDefine.F_HELPER_EXE_PATH + "Hardware.txt");
		if (fileBuffer == "")
		{
			return;
		}
		string[] strList = StringUtility.split(fileBuffer, false, "\r\n");
		if (strList.Length != 7)
		{
			return;
		}
		mOriMAC = strList[0];
		mHDD = strList[1];
		mMainbord = strList[2];
		mCPU = strList[3];
		mBIOS = strList[4];
		mMainbordType = strList[5];
		mCurMAC = strList[6];
	}
}