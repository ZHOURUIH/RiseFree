using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;

public class PacketHeader
{
	public int mHeaderLength;     // 包头长度
	public byte mMagicByte;       // 固定0xa5
	public byte mVersion;         // 当前版本号,0
	public ushort mPayloadLength; // payload数据长度
	public ushort mCRC16;         // payload数据CRC16校验结果
	public ushort mSeqID;         // 保留字段,固定为0
	public PacketHeader()
	{
		mMagicByte = 0xA5;
		mVersion = 0;
		mPayloadLength = 0;
		mCRC16 = 0;
		mSeqID = 0;
		mHeaderLength = sizeof(byte) + sizeof(byte) + sizeof(ushort) + sizeof(ushort) + sizeof(ushort);
	}
	public PARSE_RESULT parseData(byte[] data, int count)
	{
		// 长度不够包头时,数据不足
		if (count < mHeaderLength)
		{
			return PARSE_RESULT.PR_NOT_ENOUGH;
		}
		// 确保一些固定数值是正确的
		if (data[0] != mMagicByte || data[1] != mVersion || data[mHeaderLength - 1] != mSeqID)
		{
			return PARSE_RESULT.PR_ERROR;
		}
		int offset = sizeof(byte) + sizeof(byte);
		ushort payloadLength = (ushort)BinaryUtility.readShort(data, ref offset, true);
		// 长度不够记录的数据长度,数据不足
		if (payloadLength > count - mHeaderLength)
		{
			return PARSE_RESULT.PR_NOT_ENOUGH;
		}
		ushort crc = (ushort)BinaryUtility.readShort(data, ref offset, true);
		// 校验失败,数据错误
		ushort dataCRC16 = BinaryUtility.crc16(0xFF, data, payloadLength, mHeaderLength);
		if (crc != dataCRC16)
		{
			return PARSE_RESULT.PR_ERROR;
		}
		// 只有解析成功时,才保存数据
		mPayloadLength = payloadLength;
		mCRC16 = crc;
		return PARSE_RESULT.PR_SUCCESS;
	}
	public byte[] toBytes()
	{
		byte[] data = new byte[mHeaderLength];
		int offset = 0;
		BinaryUtility.writeByte(data, ref offset, mMagicByte);
		BinaryUtility.writeByte(data, ref offset, mVersion);
		BinaryUtility.writeShort(data, ref offset, (short)mPayloadLength, true);
		BinaryUtility.writeShort(data, ref offset, (short)mCRC16, true);
		BinaryUtility.writeShort(data, ref offset, (short)mSeqID, true);
		return data;
	}
}