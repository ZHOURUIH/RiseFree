//using UnityEngine;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.IO.Ports;
//using System.Threading;

//public class SerialPortManager : FrameComponent
//{
//	protected const int MAX_RECEIVE_PACKET_COUNT = 256;
//	protected const int MAX_SEND_PACKET_COUNT = 256;
//	protected Dictionary<COM_PACKET, Type> mComPacketRegisteList;
//	protected SerialPort mComDevice;
//	protected int mInputBufferSize;
//	protected byte[] mInputBuffer;
//	protected int mInputDataSize;
//	protected CustomThread mReceiveThread;
//	protected CustomThread mParseThread;
//	protected CustomThread mDisconnectDeviceTryOpenThread;
//	protected List<byte[]> mOutputBufferList;
//	protected ThreadLock mInputBufferLock;
//	protected ThreadLock mOutputBufferLock;
//	protected ThreadLock mReceivedPacketLock;
//	protected ThreadLock mSendPacketLock;
//	protected List<SerialPortPacket> mReceivedPacket;
//	protected List<SerialPortPacket> mSendPacket;
//	protected List<string> mSerialPortList;
//	protected DateTime mLastPacketTime;
//	protected DEVICE_CONNENT mDeviceConnect = DEVICE_CONNENT.DC_NONE;
//	protected byte[] mRecvBytes = null;
//	public SerialPortManager(string name)
//		:base(name)
//	{
//		mComPacketRegisteList = new Dictionary<COM_PACKET, Type>();
//		mReceivedPacket = new List<SerialPortPacket>();
//		mSendPacket = new List<SerialPortPacket>();
//		mSerialPortList = new List<string>();
//		mInputBufferSize = 1024;
//		mInputBuffer = new byte[mInputBufferSize];
//		mInputDataSize = 0;
//		mOutputBufferList = new List<byte[]>();
//		mInputBufferLock = new ThreadLock();
//		mOutputBufferLock = new ThreadLock();
//		mReceivedPacketLock = new ThreadLock();
//		mSendPacketLock = new ThreadLock();
//		mReceiveThread = new CustomThread("SerialPortReceive");
//		mParseThread = new CustomThread("SerialPortParse");
//		mDisconnectDeviceTryOpenThread = new CustomThread("OpenSerialPortDevice");
//	}
//	public override void init()
//	{
//		registeAllPacket();
//		mReceiveThread.start(receiveThread, 10);
//		mParseThread.start(parseThread, 10);
//		mDisconnectDeviceTryOpenThread.start(disconnectDeviceTryOpenThread, 50);
//	}
//	public override void destroy()
//	{
//		mReceiveThread.destroy();
//		mParseThread.destroy();
//		mDisconnectDeviceTryOpenThread.destroy();
//		closeDevice();
//		base.destroy();
//		UnityUtility.logInfo("串口管理器退出完毕!");
//	}
//	public override void update(float elapsedTime)
//	{
//		processReceived();
//	}
//	public bool isDeviceConnect()
//	{
//		return mComDevice != null && mComDevice.IsOpen;
//	}
//	public void closeDevice()
//	{
//		if(mComDevice != null)
//		{
//			if(mComDevice.IsOpen)
//			{
//				mComDevice.Close();
//			}
//			mComDevice = null;
//		}
//	}
//	public void openDevice()
//	{
//		try
//		{
//			if(mSerialPortList.Count > 1)
//			{
//				mDeviceConnect = DEVICE_CONNENT.DC_PROCEED;
//				// 正在连接设备
//				// 因为列表中第0个始终都是COM1,所以只打开第1个串口
//				mComDevice = new SerialPort(mSerialPortList[1], 115200, Parity.None, 8, StopBits.One);
//				mComDevice.Open();
//				// 设备已连接
//				mDeviceConnect = DEVICE_CONNENT.DC_SUCCESS;
//			}
//		}
//		catch(Exception)
//		{
//			closeDevice();
//		}
//	}
//	public void sendPacket(SerialPortPacket packet)
//	{
//		mSendPacketLock.waitForUnlock();
//		if(mSendPacket.Count < MAX_SEND_PACKET_COUNT)
//		{
//			mSendPacket.Add(packet);
//		}
//		mSendPacketLock.unlock();
//	}
//	public SerialPortPacket createPacket(COM_PACKET type)
//	{
//		if (mComPacketRegisteList.ContainsKey(type))
//		{
//			return UnityUtility.createInstance<SerialPortPacket>(mComPacketRegisteList[type], type);
//		}
//		return null;
//	}
//	public T createPacket<T>(out T packet, COM_PACKET type) where T : SerialPortPacket
//	{
//		packet = createPacket(type) as T;
//		return packet;
//	}
//	public DateTime getLastPacketTime() { return mLastPacketTime; }
//	public DEVICE_CONNENT getDeviceConnect() { return mDeviceConnect; }
//	//------------------------------------------------------------------------------------------------------------------------------
//	protected void processReceived()
//	{
//		mReceivedPacketLock.waitForUnlock();
//		int count = mReceivedPacket.Count;
//		for(int i = 0; i < count; ++i)
//		{
//			mReceivedPacket[i].execute();
//		}
//		mReceivedPacket.Clear();
//		mReceivedPacketLock.unlock();
//	}
//	protected void receiveThread()
//	{
//		if(!isDeviceConnect())
//		{
//			return;
//		}
//		if(mRecvBytes == null)
//		{
//			int readCount = mComDevice.ReadBufferSize;
//			mRecvBytes = new byte[readCount];
//		}
//		int recvCount = mComDevice.Read(mRecvBytes, 0, mRecvBytes.Length);
//		mInputBufferLock.waitForUnlock();
//		addDataToInputBuffer(mRecvBytes, recvCount);
//		mInputBufferLock.unlock();

//		// 先同步发送列表
//		mSendPacketLock.waitForUnlock();
//		int count = mSendPacket.Count;
//		for (int i = 0; i < count; ++i)
//		{
//			mOutputBufferList.Add(mSendPacket[i].toBytes());
//		}
//		mSendPacket.Clear();
//		mSendPacketLock.unlock();
//		// 发送所有需要发送的数据
//		int sendCount = mOutputBufferList.Count;
//		for (int i = 0; i < sendCount; ++i)
//		{
//			if(mOutputBufferList[i] != null)
//			{
//				mComDevice.Write(mOutputBufferList[i], 0, mOutputBufferList[i].Length);
//			}
//		}
//		mOutputBufferList.Clear();
//	}
//	protected void parseThread()
//	{
//		if (mInputDataSize > 0)
//		{
//			mInputBufferLock.waitForUnlock();
//			// 首先解析包头
//			PacketHeader header = new PacketHeader(GameDefine.REPORT_IN);
//			PARSE_RESULT result = header.parseData(mInputBuffer, mInputDataSize);
//			// 解析失败,则将缓冲区清空
//			if (result == PARSE_RESULT.PR_ERROR)
//			{
//				clearInputBuffer();
//			}
//			// 数据不足,继续等待接收数据
//			else if (result == PARSE_RESULT.PR_NOT_ENOUGH)
//			{
//				;
//			}
//			// 解析成功,判断包类型,继续解析
//			else if (result == PARSE_RESULT.PR_SUCCESS)
//			{
//				SerialPortPacket packet = createPacket(mInputBuffer[header.mHeaderLength]);
//				if (packet != null)
//				{
//					int offset = 0;
//					packet.parseData(header, mInputBuffer, mInputDataSize, ref offset);
//					// 解析数量与包头不一致,清空缓冲区
//					if (offset != header.mHeaderLength + header.mPayloadLength)
//					{
//						clearInputBuffer();
//					}
//					// 解析正确,移除已解析的数据
//					else
//					{
//						removeDataFromInputBuffer(0, offset);
//						// 加入接收的消息包列表
//						mReceivedPacketLock.waitForUnlock();
//						if (mReceivedPacket.Count < MAX_RECEIVE_PACKET_COUNT)
//						{
//							mReceivedPacket.Add(packet);
//						}
//						mReceivedPacketLock.unlock();
//					}
//					mLastPacketTime = DateTime.Now;
//				}
//				else
//				{
//					clearInputBuffer();
//				}
//			}
//			mInputBufferLock.unlock();
//		}
//	}
//	public void disconnectDeviceTryOpenThread()
//	{
//		// 如果串口设备有变化,则需要关闭当前串口
//		if (mSerialPortList.Count != SerialPort.GetPortNames().Length)
//		{
//			mSerialPortList.Clear();
//			foreach (string item in SerialPort.GetPortNames())
//			{
//				mSerialPortList.Add(item);
//			}
//			closeDevice();
//		}
//		// 如果串口设备有变化,则需要关闭当前串口
//		// 如果发现设备串口被关闭,则一直尝试打开串口
//		if (!isDeviceConnect())
//		{
//			// 设备已断开
//			mDeviceConnect = DEVICE_CONNENT.DC_CLOSE;
//			openDevice();
//		}
//	}
//	protected void addDataToInputBuffer(byte[] data, int count)
//	{
//		// 缓冲区足够放下数据时才处理
//		if (count <= mInputBuffer.Length - mInputDataSize)
//		{
//			BinaryUtility.memcpy(mInputBuffer, data, mInputDataSize, 0, count);
//			mInputDataSize += count;
//		}
//	}
//	protected void removeDataFromInputBuffer(int start, int count)
//	{
//		if (mInputDataSize >= start + count)
//		{
//			BinaryUtility.memmove(mInputBuffer, 0, start, count);
//			mInputDataSize -= count;
//		}
//	}
//	protected void clearInputBuffer()
//	{
//		mInputDataSize = 0;
//	}
//	protected void registeAllPacket()
//	{
//		registePacket<SerialPortPacketFitData>(COM_PACKET.CP_FIT_DATA);
//		registePacket<SerialPortPacketFriction>(COM_PACKET.CP_FRICTION);
//		if(mComPacketRegisteList.Count != (int)COM_PACKET.CP_MAX)
//		{
//			UnityUtility.logError("not all com packet registed!");
//		}
//	}
//	protected void registePacket<T>(COM_PACKET packetType)
//	{
//		mComPacketRegisteList.Add(packetType, typeof(T));
//	}
//	protected COM_PACKET getPacketType(int cmdID)
//	{
//		COM_PACKET packetType = COM_PACKET.CP_MAX;
//		if (cmdID == 1)
//		{
//			packetType = COM_PACKET.CP_FIT_DATA;
//		}
//		else if (cmdID == 2)
//		{
//			packetType = COM_PACKET.CP_FRICTION;
//		}
//		return packetType;
//	}
//	protected SerialPortPacket createPacket(int cmdID)
//	{
//		return createPacket(getPacketType(cmdID));
//	}
//}