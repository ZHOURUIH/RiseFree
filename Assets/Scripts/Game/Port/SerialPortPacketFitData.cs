using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class SerialPortPacketFitData : SerialPortPacket
{
	protected ushort mPower;
	protected ushort mRPM;
	protected short mAngle;
	protected byte mKeyStatus;
	public SerialPortPacketFitData(COM_PACKET type)
		:base(type)
	{
		;
	}
	public override void parseData(PacketHeader header, byte[] data, int dataCount, ref int offset)
	{
		base.parseData(header, data, dataCount, ref offset);
		mPower = (ushort)BinaryUtility.readShort(data, ref offset, true);
		mRPM = (ushort)BinaryUtility.readShort(data, ref offset, true);
		// 最高位为0,角度为正
		if ((data[offset] & 0x80) == 0)
		{
			mAngle = BinaryUtility.readShort(data, ref offset, true);
		}
		// 最高位为1,角度为负
		else
		{
			data[offset] &= 0x7f;
			mAngle = BinaryUtility.readShort(data, ref offset, true);
			mAngle = (short)-mAngle;
		}
		mKeyStatus = BinaryUtility.readByte(data, ref offset);
	}
	public override void execute()
	{
		CharacterMyself myself = mCharacterManager.getMyself();
		// 速度
		if(myself != null && myself.getProcessExternalSpeed())
		{
			CommandCharacterHardwareSpeed cmdSpeed = newCmd(out cmdSpeed, false);
			cmdSpeed.mDirectSpeed = false;
			cmdSpeed.mExternalSpeed = true;
			if(GameUtility.mReadRPM)
			{
				cmdSpeed.mSpeed = GameUtility.HWSToMS(mRPM);
			}
			else
			{
				cmdSpeed.mSpeed = GameUtility.HWSToMS(mPower);
			}
			pushCommand(cmdSpeed, mCharacterManager.getMyself());
		}
		// 按键
		KeyCode[] key = new KeyCode[] {KeyCode.A, KeyCode.B, KeyCode.X, KeyCode.Y};
		int count = key.Length;
		for(int i = 0; i < count; ++i)
		{
			mGameInputManager.setKeyState(key[i], isKeyDown(i));
		}
		// 转向
		mGameInputManager.setStickAngle(mAngle);
		if (mScriptDebugInfo != null)
		{
			mScriptDebugInfo.setOrginStackAngle(mAngle);
			mScriptDebugInfo.setRPM(mRPM);
			mScriptDebugInfo.setPower(mPower);
		}
	}
	bool isKeyDown(int index)
	{
		return (mKeyStatus & (0x00000001 << index)) != 0;
	}
}