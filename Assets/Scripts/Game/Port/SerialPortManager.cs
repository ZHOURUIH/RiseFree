using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;

public class SerialPortManager : FrameComponent
{
	protected Dictionary<COM_PACKET, Type> mComPacketRegisteList;
	protected SerialPort mComDevice;
	protected int mInputBufferSize;
	protected byte[] mInputBuffer;
	protected int mInputDataSize;
	protected Thread mReceiveThread;
	protected Thread mSendThread;
	protected Thread mParseThread;
	protected bool mRunning = false;
	protected bool mReceiveFinish = true;
	protected bool mSendFinish = true;
	protected bool mParseFinish = true;
	protected List<byte[]> mOutputBufferList;
	protected ThreadLock mInputBufferLock;
	protected ThreadLock mOutputBufferLock;
	protected ThreadLock mReceivedPacketLock;
	protected List<SerialPortPacket> mReceivedPacket;
	protected List<string> mSerialPortList;
	public SerialPortManager(string name)
		:base(name)
	{
		mComPacketRegisteList = new Dictionary<COM_PACKET, Type>();
		mReceivedPacket = new List<SerialPortPacket>();
		mSerialPortList = new List<string>();
		mInputBufferSize = 1024;
		mInputBuffer = new byte[mInputBufferSize];
		mInputDataSize = 0;
		mOutputBufferList = new List<byte[]>();
		mInputBufferLock = new ThreadLock();
		mOutputBufferLock = new ThreadLock();
		mReceivedPacketLock = new ThreadLock();
	}
	public override void init()
	{
		registeAllPacket();
		mReceiveFinish = true;
		mSendFinish = true;
		mParseFinish = true;
		mRunning = true;
		mReceiveThread = new Thread(receiveThread);
		mSendThread = new Thread(sendThread);
		mParseThread = new Thread(parseThread);
		mReceiveThread.Start();
		mSendThread.Start();
		mParseThread.Start();
	}
	public override void destroy()
	{
		closeDevice();
		mRunning = false;
		while (!mReceiveFinish) { }
		if (mReceiveThread != null)
		{
			mReceiveThread.Abort();
			mReceiveThread = null;
		}
		while (!mSendFinish) { }
		if (mSendThread != null)
		{
			mSendThread.Abort();
			mSendThread = null;
		}
		while (!mParseFinish) { }
		if (mParseThread != null)
		{
			mParseThread.Abort();
			mParseThread = null;
		}
		base.destroy();
		UnityUtility.logInfo("串口管理器退出完毕!");
	}
	public override void update(float elapsedTime)
	{
		processReceived();
		// 如果串口设备有变化,则需要关闭当前串口
		if(mSerialPortList.Count != SerialPort.GetPortNames().Length)
		{
			mSerialPortList.Clear();
			foreach (string item in SerialPort.GetPortNames())
			{
				mSerialPortList.Add(item);
			}
			closeDevice();
		}
		// 如果发现设备串口被关闭,则一直尝试打开串口
		if (!isDeviceConnect())
		{
			openDevice();
		}
	}
	public bool isDeviceConnect()
	{
		return mComDevice != null && mComDevice.IsOpen;
	}
	public void closeDevice()
	{
		if(mComDevice != null)
		{
			if(mComDevice.IsOpen)
			{
				mComDevice.Close();
			}
			mComDevice = null;
		}
	}
	public void openDevice()
	{
		try
		{
			if(mSerialPortList.Count > 1)
			{
				// 因为列表中第0个始终都是COM1,所以只打开第1个串口
				mComDevice = new SerialPort(mSerialPortList[1], 115200, Parity.None, 8, StopBits.One);
				mComDevice.Open();
			}
		}
		catch(Exception)
		{
			closeDevice();
		}
	}
	//------------------------------------------------------------------------------------------------------------------------------
	protected void processReceived()
	{
		mReceivedPacketLock.waitForUnlock();
		int count = mReceivedPacket.Count;
		for(int i = 0; i < count; ++i)
		{
			mReceivedPacket[i].execute();
		}
		mReceivedPacket.Clear();
		mReceivedPacketLock.unlock();
	}
	protected void receiveThread()
	{
		mReceiveFinish = false;
		ThreadTimeLock timeLock = new ThreadTimeLock(10);
		while (mRunning)
		{
			timeLock.update();
			try
			{
				if(!isDeviceConnect())
				{
					continue;
				}
				string receivedData = mComDevice.ReadExisting();
				mInputBufferLock.waitForUnlock();
				addDataToInputBuffer(BinaryUtility.stringToBytes(receivedData));
				mInputBufferLock.unlock();
			}
			catch (Exception e)
			{
				UnityUtility.logInfo("捕获接收串口异常 : " + e.Message + ", stack : " + e.StackTrace, LOG_LEVEL.LL_FORCE);
				break;
			}
		}
		UnityUtility.logInfo("退出串口接收线程!");
		mReceiveFinish = true;
	}
	protected void sendThread()
	{
		mSendFinish = false;
		ThreadTimeLock timeLock = new ThreadTimeLock(10);
		while (mRunning)
		{
			timeLock.update();
			try
			{
				if (!isDeviceConnect())
				{
					continue;
				}
			}
			catch (Exception e)
			{
				UnityUtility.logInfo("捕获发送串口异常 : " + e.Message + ", stack : " + e.StackTrace, LOG_LEVEL.LL_FORCE);
				break;
			}
		}
		UnityUtility.logInfo("退出串口发送线程!");
		mSendFinish = true;
	}
	protected void parseThread()
	{
		mParseFinish = false;
		ThreadTimeLock timeLock = new ThreadTimeLock(10);
		while (mRunning)
		{
			timeLock.update();
			try
			{
				if (mInputDataSize > 0)
				{
					mInputBufferLock.waitForUnlock();
					// 首先解析包头
					PacketHeader header = new PacketHeader();
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
								mReceivedPacket.Add(packet);
								mReceivedPacketLock.unlock();
							}
						}
						else
						{
							clearInputBuffer();
						}
					}
					mInputBufferLock.unlock();
				}
			}
			catch (Exception e)
			{
				UnityUtility.logInfo("捕获解析异常 : " + e.Message + ", stack : " + e.StackTrace, LOG_LEVEL.LL_FORCE);
				break;
			}
		}
		UnityUtility.logInfo("退出串口解析线程!");
		mParseFinish = true;
	}
	protected void addDataToInputBuffer(byte[] data)
	{
		// 缓冲区足够放下数据时才处理
		if (data.Length <= mInputBuffer.Length - mInputDataSize)
		{
			BinaryUtility.memcpy(mInputBuffer, data, mInputDataSize);
			mInputDataSize += data.Length;
		}
	}
	protected void removeDataFromInputBuffer(int start, int count)
	{
		if (mInputDataSize >= start + count)
		{
			BinaryUtility.memmove(mInputBuffer, 0, start, count);
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
		if(mComPacketRegisteList.Count != (int)COM_PACKET.CP_MAX)
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
	protected SerialPortPacket createPacket(COM_PACKET type)
	{
		if(mComPacketRegisteList.ContainsKey(type))
		{
			return UnityUtility.createInstance<SerialPortPacket>(mComPacketRegisteList[type], type);
		}
		return null;
	}
}