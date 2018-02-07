using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;

public class SerialPortPacket : GameBase
{
	protected PacketHeader mHeader;
	protected COM_PACKET mPacketType;
	protected byte mCmdID;
	protected byte mKeyID;
	protected byte mValueLength;
	public SerialPortPacket(COM_PACKET type)
	{
		mPacketType = type;
	}
	public virtual void parseData(PacketHeader header, byte[] data, int dataCount, ref int offset)
	{
		mHeader = header;
		offset = mHeader.mHeaderLength;
		mCmdID = BinaryUtility.readByte(data, ref offset);
		mKeyID = BinaryUtility.readByte(data, ref offset);
		mValueLength = BinaryUtility.readByte(data, ref offset);
	}
	public virtual byte[] toBytes()
	{
		return null;
	}
	public virtual void execute() { }
}