using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PacketInfo
{
	public PacketInfo(SOCKET_PACKET type, Type classType, int dataCount)
	{
		mType = type;
		mClassType = classType;
		mDataCount = dataCount;
	}
	public SOCKET_PACKET mType;
	public Type mClassType;
	public int mDataCount;
}

public class SocketFactory
{
	protected Dictionary<SOCKET_PACKET, PacketInfo> mSocketPacketTypeList = new Dictionary<SOCKET_PACKET, PacketInfo>();
	public void init()
	{
		registerPacket<SocketPacketSpeedDataRet>(SOCKET_PACKET.SP_SPEED_DATA_RET, 12);
		registerPacket<SocketPacketFriction>(SOCKET_PACKET.SP_FRICTION, 6);
		registerPacket<SocketPacketFrictionRet>(SOCKET_PACKET.SP_FRICTION_RET, 6);
	}
	public SocketPacket createPacket(SOCKET_PACKET type)
	{
		if (mSocketPacketTypeList.ContainsKey(type))
		{
			PacketInfo info = mSocketPacketTypeList[type];
			return UnityUtility.createInstance<SocketPacket>(mSocketPacketTypeList[type].mClassType, info.mType, info.mDataCount);
		}
		return null;
	}
	public int getPacketSize(SOCKET_PACKET type)
	{
		return mSocketPacketTypeList[type].mDataCount;
	}
	public SOCKET_PACKET getSocketType(byte[] buff, int bufflength)
	{
		if (bufflength < 2)
		{
			return SOCKET_PACKET.SP_MAX;
		}
		else if (bufflength == getPacketSize(SOCKET_PACKET.SP_SPEED_DATA_RET) && buff[1] == 0)
		{
			return SOCKET_PACKET.SP_SPEED_DATA_RET;
		}
		else if (bufflength == getPacketSize(SOCKET_PACKET.SP_FRICTION_RET) && buff[1] == 2)
		{
			return SOCKET_PACKET.SP_FRICTION_RET;
		}
		else if (bufflength == getPacketSize(SOCKET_PACKET.SP_FRICTION) && buff[1] == 3)
		{
			return SOCKET_PACKET.SP_FRICTION;
		}
		return SOCKET_PACKET.SP_MAX;
	}
	//-------------------------------------------------------------------------------------------------------------------------------
	protected void registerPacket<T>(SOCKET_PACKET type, int dataCount)
	{
		PacketInfo info = new PacketInfo(type, typeof(T), dataCount);
		mSocketPacketTypeList.Add(type, info);
	}
}