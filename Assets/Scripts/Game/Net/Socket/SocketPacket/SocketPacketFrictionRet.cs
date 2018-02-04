using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SocketPacketFrictionRet : SocketPacket
{
	public SocketPacketFrictionRet(SOCKET_PACKET type, int dataCount)
		:
		base(type, dataCount)
	{ }
	public override void readData(byte[] data, int dataSize)
	{
		mData = new byte[dataSize];
		BinaryUtility.memcpy(mData, data, 0, 0, dataSize);
	}
	public override void execute()
	{
		// 检查包头包尾
		if ((mData[0] != 0xFE || mData[mData.Length - 1] != 0xFF))
		{
			return;
		}

		// 检查校验位
		int sum = 0;
		for (int i = 1; i < 4; ++i)
		{
			sum += BinaryUtility.crc_check(mData[i]);
		}

		// 校验不通过则直接返回
		if (sum != mData[4])
		{
			return;
		}
	}
}