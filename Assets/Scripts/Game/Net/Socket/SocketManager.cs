﻿using UnityEngine;
using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Collections.Generic;

public class OUTPUT_STREAM
{
	public OUTPUT_STREAM(byte[] data, int dataSize)
	{
		mData = data;
		mDataSize = dataSize;
	}
	public byte[] mData;
	public int mDataSize;
};

public class INPUT_ELEMENT
{
	public INPUT_ELEMENT(SOCKET_PACKET type, byte[] data, int dataSize)
	{
		mType = type;
		mData = data;
		mDataSize = dataSize;
	}
	public SOCKET_PACKET mType;
	public byte[] mData;
	public int mDataSize;
};

public class SocketManager : FrameComponent
{
	protected const int	mMaxReceiveCount = 1024;
	protected Socket mServerSoket;
	protected Socket mBroadcastSocket;
	protected EndPoint mBroadcastEP;
	protected CustomThread mReceiveThread;
	protected CustomThread mOutputThread;
	protected List<OUTPUT_STREAM> mOutputList;
	protected List<INPUT_ELEMENT> mInputList;
	protected List<INPUT_ELEMENT> mRecieveList;
	protected ThreadLock mOutputLock;
	protected ThreadLock mInputLock;
	protected SocketFactory mSocketFactory;
	public SocketManager(string name)
		:base(name)
	{
		mOutputList = new List<OUTPUT_STREAM>();
		mInputList = new List<INPUT_ELEMENT>();
		mRecieveList = new List<INPUT_ELEMENT>();
		mOutputLock = new ThreadLock();
		mInputLock = new ThreadLock();
		mSocketFactory = new SocketFactory();
		mReceiveThread = new CustomThread("SocketReceive");
		mOutputThread = new CustomThread("SocketOutput");
	}
	public override void init()
	{
		try
		{
			mSocketFactory.init();
			// 创建socket  
			mServerSoket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			// 绑定地址
			int port = (int)mFrameConfig.getFloatParam(GAME_DEFINE_FLOAT.GDF_SOCKET_PORT);
			mServerSoket.Bind(new IPEndPoint(IPAddress.Any, port));
			mBroadcastSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			mBroadcastSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
			// 广播端
			int broadcastPort = (int)mFrameConfig.getFloatParam(GAME_DEFINE_FLOAT.GDF_BROADCAST_PORT);
			mBroadcastEP = new IPEndPoint(IPAddress.Broadcast, broadcastPort);
		}
		catch(Exception)
		{
			UnityUtility.logError("初始化网络失败!请确保测试软件等其他可能占用网络端口的程序已关闭!");
			mGameFramework.stop();
			return;
		}
		mReceiveThread.start(updateUdpServer, 1);
		mOutputThread.start(updateOutput, 30);
	}
	public override void update(float elapsedTime)
	{
		processInput();
	}
	public override void destroy()
	{
		if (mServerSoket != null)
		{
			mServerSoket.Close();
			mServerSoket = null;
		}
		if(mBroadcastSocket != null)
		{
			mBroadcastSocket.Close();
			mBroadcastSocket = null;
		}
		mReceiveThread.destroy();
		mOutputThread.destroy();
		base.destroy();
		UnityUtility.logInfo("网络管理器退出完毕", LOG_LEVEL.LL_FORCE);
	}
	public SocketPacket createPacket(SOCKET_PACKET type)
	{
		return mSocketFactory.createPacket(type);
	}
	public void sendMessage(SocketPacket packet)
	{
		// 将消息包中的数据准备好,然后放入发送列表中
		packet.fillData();
		mOutputLock.waitForUnlock();
		mOutputList.Add(new OUTPUT_STREAM(packet.getData(), packet.getSize()));
		mOutputLock.unlock();
	}
	//-------------------------------------------------------------------------------------------------------------------------
	protected void processInput()
	{
		// 等待解锁接收流的读写,并锁定接收流
		mInputLock.waitForUnlock();
		int receiveCount = mRecieveList.Count;
		for (int i = 0; i < receiveCount; ++i)
		{
			mInputList.Add(mRecieveList[i]);
		}
		mRecieveList.Clear();
		mInputLock.unlock();

		int streamCount = mInputList.Count;
		for (int i = 0; i < streamCount; ++i)
		{
			INPUT_ELEMENT element = mInputList[i];
			SocketPacket packetReply = createPacket(element.mType);
			packetReply.readData(element.mData, element.mDataSize);
			packetReply.execute();
		}
		mInputList.Clear();
	}
	protected bool updateOutput()
	{
		if (mBroadcastSocket == null)
		{
			return false;
		}
		mOutputLock.waitForUnlock();
		List<OUTPUT_STREAM> tempList = new List<OUTPUT_STREAM>(mOutputList);
		mOutputList.Clear();
		mOutputLock.unlock();
		int outputCount = tempList.Count;
		for (int i = 0; i < outputCount; ++i)
		{
			mBroadcastSocket.SendTo(tempList[i].mData, mBroadcastEP);
		}
		return true;
	}
	protected void receivePacket(SOCKET_PACKET type, byte[] data, int dataSize)
	{
		mInputLock.waitForUnlock();
		mRecieveList.Add(new INPUT_ELEMENT(type, data, dataSize));
		mInputLock.unlock();
	}
	protected bool updateUdpServer()
	{
		IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, 0);
		EndPoint ep = (EndPoint)endpoint;
		byte[] recBuff = new byte[mMaxReceiveCount];
		if (mServerSoket == null)
		{
			return false;
		}
		int intReceiveLength = mServerSoket.ReceiveFrom(recBuff, ref ep);
		if (intReceiveLength > 0)
		{
			SOCKET_PACKET spType = mSocketFactory.getSocketType(recBuff, intReceiveLength);
			receivePacket(spType, recBuff, intReceiveLength);
		}
		return true;
	}
}