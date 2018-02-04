using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SocketPacketFriction : SocketPacket
{
	public int mFriction;
	public SocketPacketFriction(SOCKET_PACKET type, int dataCount)
		:
		base(type, dataCount)
	{ }
	public void setFrictionParam(int priction)
	{
		mFriction = priction;
	}
	public override void fillData()
	{
		// 阻力回复包固定6个字节
		mData = new byte[mDataCount];
		// FE 03 1个字节阻力值 机器号 校验码 FF
		mData[0] = (byte)0xFE;
		mData[1] = (byte)0x03;
		mData[2] = (byte)mFriction;
		mData[3] = (byte)0;
		int checkCount = 0;
		for (int i = 1; i < mData.Length; ++i)
		{
			checkCount += BinaryUtility.crc_check(mData[i]);
		}
		mData[4] = (byte)checkCount;
		mData[5] = (byte)0xFF;
	}
	public override void execute() { }
}