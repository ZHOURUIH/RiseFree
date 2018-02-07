using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;

public class SerialPortPacketFriction : SerialPortPacket
{
	protected byte mFriction;
	public SerialPortPacketFriction(COM_PACKET type)
		: base(type)
	{
		;
	}
	public void setFriction(byte friction) { mFriction = friction; }
	public override void parseData(PacketHeader header, byte[] data, int dataCount, ref int offset)
	{
		base.parseData(header, data, dataCount, ref offset);
		mFriction = BinaryUtility.readByte(data, ref offset);
	}
	public override byte[] toBytes()
	{
		mCmdID = 2;
		mKeyID = 1;
		mValueLength = sizeof(byte);
		int payloadLen = sizeof(byte) + sizeof(byte) + sizeof(byte) + mValueLength;
		byte[] payload = new byte[payloadLen];
		int index = 0;
		BinaryUtility.writeByte(payload, ref index, mCmdID);
		BinaryUtility.writeByte(payload, ref index, mKeyID);
		BinaryUtility.writeByte(payload, ref index, mValueLength);
		BinaryUtility.writeByte(payload, ref index, mFriction);
		mHeader = new PacketHeader();
		mHeader.mPayloadLength = (ushort)payloadLen;
		mHeader.mCRC16 = BinaryUtility.crc16(0xFF, payload, payloadLen);
		byte[] headerData = mHeader.toBytes();
		byte[] packetData = new byte[payload.Length + headerData.Length];
		BinaryUtility.memcpy(packetData, headerData, 0);
		BinaryUtility.memcpy(packetData, payload, headerData.Length);
		return packetData;
	}
}