using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class USBManager : FrameComponent
{
	protected const int MAX_RECEIVE_PACKET_COUNT = 256;
	protected const int MAX_SEND_PACKET_COUNT = 256;
	protected Dictionary<COM_PACKET, Type> mComPacketRegisteList;
	protected int mInputBufferSize;
	protected byte[] mInputBuffer;
	protected int mInputDataSize;
	protected CustomThread mReceiveThread;
	protected CustomThread mParseThread;
	protected CustomThread mOpenDeviceThread;
	protected List<byte[]> mOutputBufferList;
	protected ThreadLock mInputBufferLock;
	protected ThreadLock mOutputBufferLock;
	protected ThreadLock mReceivedPacketLock;
	protected ThreadLock mSendPacketLock;
	protected List<SerialPortPacket> mReceivedPacket;
	protected List<SerialPortPacket> mSendPacket;
	protected DateTime mLastPacketTime;
	protected DEVICE_CONNENT mDeviceConnect = DEVICE_CONNENT.DC_NONE;
	protected byte[] mRecvBytes = null;
	protected ushort VID = 0x0483;
	protected ushort PID = 0x5750;
	protected HIDDevice mHIDDevice;
	protected int mCurDeviceCount;	// 用于记录当前连接设备数量,数量有改变时检查输入设备是否可用
	public USBManager(string name)
		: base(name)
	{
		mComPacketRegisteList = new Dictionary<COM_PACKET, Type>();
		mReceivedPacket = new List<SerialPortPacket>();
		mSendPacket = new List<SerialPortPacket>();
		mInputBufferSize = 1024;
		mInputBuffer = new byte[mInputBufferSize];
		mInputDataSize = 0;
		mOutputBufferList = new List<byte[]>();
		mInputBufferLock = new ThreadLock();
		mOutputBufferLock = new ThreadLock();
		mReceivedPacketLock = new ThreadLock();
		mSendPacketLock = new ThreadLock();
		mReceiveThread = new CustomThread("USBReceive");
		mParseThread = new CustomThread("USBParse");
		mOpenDeviceThread = new CustomThread("OpenUSBDevice");
		mCurDeviceCount = 0;
	}
	public override void init()
	{
		registeAllPacket();
		mReceiveThread.start(receiveThread, 10);
		mReceiveThread.setBackground(true);	// 终止接收线程时需要强制终止
		mParseThread.start(parseThread, 10);
		mOpenDeviceThread.start(disconnectDeviceTryOpenThread, 500);
		mOpenDeviceThread.setBackground(true);
	}
	public override void destroy()
	{
		// 首先停止自动连接设备的线程,再关闭设备
		mOpenDeviceThread.destroy();
		closeDevice();
		mReceiveThread.destroy();
		mParseThread.destroy();
		base.destroy();
		UnityUtility.logInfo("USB管理器退出完毕!");
	}
	public override void update(float elapsedTime)
	{
		processReceived();
	}
	public bool isDeviceConnect()
	{
		return mHIDDevice != null && mHIDDevice.deviceConnected;
	}
	public void sendPacket(SerialPortPacket packet)
	{
		mSendPacketLock.waitForUnlock();
		if (mSendPacket.Count < MAX_SEND_PACKET_COUNT)
		{
			mSendPacket.Add(packet);
		}
		mSendPacketLock.unlock();
	}
	public SerialPortPacket createPacket(COM_PACKET type)
	{
		if (mComPacketRegisteList.ContainsKey(type))
		{
			return UnityUtility.createInstance<SerialPortPacket>(mComPacketRegisteList[type], type);
		}
		return null;
	}
	public T createPacket<T>(out T packet, COM_PACKET type) where T : SerialPortPacket
	{
		packet = createPacket(type) as T;
		return packet;
	}
	public DateTime getLastPacketTime() { return mLastPacketTime; }
	public DEVICE_CONNENT getDeviceConnect() { return mDeviceConnect; }
	//-----------------------------------------------------------------------------------------------------------------------------------------------------------------
	protected void closeDevice()
	{
		if(mHIDDevice != null)
		{
			mHIDDevice.close();
			mHIDDevice = null;
		}
		mDeviceConnect = DEVICE_CONNENT.DC_CLOSE;
	}
	protected void openDevice(string devicePath = "")
	{
		if (mHIDDevice != null)
		{
			return;
		}
		try
		{
			mDeviceConnect = DEVICE_CONNENT.DC_PROCEED;
			if(devicePath != "")
			{
				mHIDDevice = new HIDDevice(devicePath);
			}
			else
			{
				mHIDDevice = new HIDDevice(VID, PID);
			}
			
			if(!mHIDDevice.deviceConnected)
			{
				mDeviceConnect = DEVICE_CONNENT.DC_CLOSE;
				UnityUtility.logInfo("无法连接输入设备!");
			}
			else
			{
				mDeviceConnect = DEVICE_CONNENT.DC_SUCCESS;
				UnityUtility.logInfo("输入设备连接成功!");
			}
		}
		catch (Exception)
		{
			closeDevice();
		}
	}
	protected void processReceived()
	{
		mReceivedPacketLock.waitForUnlock();
		int count = mReceivedPacket.Count;
		for (int i = 0; i < count; ++i)
		{
			mReceivedPacket[i].execute();
		}
		mReceivedPacket.Clear();
		mReceivedPacketLock.unlock();
	}
	protected bool receiveThread()
	{
		if (!isDeviceConnect())
		{
			return true;
		}
		const int MaxRecvCount = 32;
		if (mRecvBytes == null)
		{
			mRecvBytes = new byte[MaxRecvCount];
		}
		// 只接收23个字节,如果不是23个字节,则丢弃该数据
		int readCount = mHIDDevice.read(ref mRecvBytes, 23);
		if(readCount != 0)
		{
			BinaryUtility.memmove(ref mRecvBytes, 0, 1, MaxRecvCount - 1);
			readCount -= 4;
			// 去除第一个字节和最后3个字节
			mInputBufferLock.waitForUnlock();
			addDataToInputBuffer(mRecvBytes, readCount);
			mInputBufferLock.unlock();
		}

		// 先同步发送列表
		mSendPacketLock.waitForUnlock();
		int count = mSendPacket.Count;
		for (int i = 0; i < count; ++i)
		{
			mOutputBufferList.Add(mSendPacket[i].toBytes());
		}
		mSendPacket.Clear();
		mSendPacketLock.unlock();
		// 发送所有需要发送的数据
		int sendCount = mOutputBufferList.Count;
		for (int i = 0; i < sendCount; ++i)
		{
			if (mOutputBufferList[i] != null)
			{
				mHIDDevice.write(mOutputBufferList[i]);
			}
		}
		mOutputBufferList.Clear();
		return true;
	}
	protected bool parseThread()
	{
		if (mInputDataSize > 0)
		{
			mInputBufferLock.waitForUnlock();
			// 首先解析包头
			PacketHeader header = new PacketHeader(GameDefine.REPORT_IN);
			PARSE_RESULT result = header.parseData(mInputBuffer, mInputDataSize);
			// 解析失败,则将缓冲区清空
			if (result == PARSE_RESULT.PR_ERROR)
			{
				clearInputBuffer();
			}
			// 数据不足,继续等待接收数据
			else if (result == PARSE_RESULT.PR_NOT_ENOUGH)
			{
				;
			}
			// 解析成功,判断包类型,继续解析
			else if (result == PARSE_RESULT.PR_SUCCESS)
			{
				SerialPortPacket packet = createPacket(mInputBuffer[header.mHeaderLength]);
				if (packet != null)
				{
					int offset = 0;
					packet.parseData(header, mInputBuffer, mInputDataSize, ref offset);
					// 解析数量与包头不一致,清空缓冲区
					if (offset != header.mHeaderLength + header.mPayloadLength)
					{
						clearInputBuffer();
					}
					// 解析正确,移除已解析的数据
					else
					{
						removeDataFromInputBuffer(0, offset);
						// 加入接收的消息包列表
						mReceivedPacketLock.waitForUnlock();
						if (mReceivedPacket.Count < MAX_RECEIVE_PACKET_COUNT)
						{
							mReceivedPacket.Add(packet);
						}
						mReceivedPacketLock.unlock();
					}
					mLastPacketTime = DateTime.Now;
				}
				else
				{
					clearInputBuffer();
				}
			}
			mInputBufferLock.unlock();
		}
		return true;
	}
	public bool disconnectDeviceTryOpenThread()
	{
		string devicePath = "";
		bool exist = false;
		// 检查当前设备是否可用
		InterfaceDetails[] devices = HIDDevice.getConnectedDevices();
		int deviceCount = devices.Length;
		if (mCurDeviceCount != deviceCount)
		{
			mCurDeviceCount = deviceCount;
			exist = false;
			for (int i = 0; i < deviceCount; ++i)
			{
				if (devices[i].VID == VID && devices[i].PID == PID)
				{
					devicePath = devices[i].devicePath;
					exist = true;
					break;
				}
			}
			if (!exist)
			{
				closeDevice();
			}
		}
		// 有设备,并且设备数量未变化,则认为有设备连接
		else if (deviceCount > 0)
		{
			exist = true;
		}
		// 如果发现设备被关闭,并且设备存在,则一直尝试打开设备
		if (!isDeviceConnect() && exist)
		{
			openDevice(devicePath);
		}
		// 暂时不实现热插拔
		if(isDeviceConnect())
		{
			return false;
		}
		return true;
	}
	protected void addDataToInputBuffer(byte[] data, int count)
	{
		// 缓冲区足够放下数据时才处理
		if (count <= mInputBuffer.Length - mInputDataSize)
		{
			BinaryUtility.memcpy(mInputBuffer, data, mInputDataSize, 0, count);
			mInputDataSize += count;
		}
	}
	protected void removeDataFromInputBuffer(int start, int count)
	{
		if (mInputDataSize >= start + count)
		{
			BinaryUtility.memmove(ref mInputBuffer, 0, start + count, mInputDataSize - count);
			mInputDataSize -= count;
		}
	}
	protected void clearInputBuffer()
	{
		mInputDataSize = 0;
	}
	protected void registeAllPacket()
	{
		registePacket<SerialPortPacketFitData>(COM_PACKET.CP_FIT_DATA);
		registePacket<SerialPortPacketFriction>(COM_PACKET.CP_FRICTION);
		if (mComPacketRegisteList.Count != (int)COM_PACKET.CP_MAX)
		{
			UnityUtility.logError("not all com packet registed!");
		}
	}
	protected void registePacket<T>(COM_PACKET packetType)
	{
		mComPacketRegisteList.Add(packetType, typeof(T));
	}
	protected COM_PACKET getPacketType(int cmdID)
	{
		COM_PACKET packetType = COM_PACKET.CP_MAX;
		if (cmdID == 1)
		{
			packetType = COM_PACKET.CP_FIT_DATA;
		}
		else if (cmdID == 2)
		{
			packetType = COM_PACKET.CP_FRICTION;
		}
		return packetType;
	}
	protected SerialPortPacket createPacket(int cmdID)
	{
		return createPacket(getPacketType(cmdID));
	}
}