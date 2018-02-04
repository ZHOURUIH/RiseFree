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
	public override void parseData(PacketHeader header, byte[] data, int dataCount, ref int offset)
	{
		base.parseData(header, data, dataCount, ref offset);
		mFriction = BinaryUtility.readByte(data, ref offset);
	}
}