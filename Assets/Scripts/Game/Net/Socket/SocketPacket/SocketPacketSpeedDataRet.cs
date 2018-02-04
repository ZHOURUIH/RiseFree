using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SocketPacketSpeedDataRet : SocketPacket
{
	public SocketPacketSpeedDataRet(SOCKET_PACKET type, int dataCount)
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
		// 只有在主场景中才能接收速度消息
		GameScene gameScene = mGameSceneManager.getCurScene();
		if (gameScene.getSceneType() != GAME_SCENE_TYPE.GST_MAIN)
		{
			return;
		}
		if ((mData[0] == (byte)0xFE && mData[11] == (byte)0xFF))
		{
			// 检查校验位 【1 - 7】中1的个数之和等于第10位
			int sum = 0;
			for (int i = 1; i < 8; ++i)
			{
				sum += BinaryUtility.crc_check(mData[i]);
			}
			// 校验正确
			if (sum == mData[10])
			{
				// [0]	  [1]		  [2] [3][4]	[5][6]	  [7]   [8]		    [9]			[10]		[11]
				// FE	  00(包类型)  00   功率		速度	   00   FF      机器号[0-19]		crc		     FF

				// 5-6 速度标识
				// 3-4使用功率来作为速度(不使用5-6的速度值)
				int speed = mData[6] * 256 + mData[5];
				speed = speed > 600 ? 600 : speed;
			}
		}
	}
}