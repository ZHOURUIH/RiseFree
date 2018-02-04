using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using UnityEngine;

public class RegisterTool : FrameComponent
{
	protected byte[] REGISTER_CODE;
	protected int CODE_LEN;
	public RegisterTool(string name)
		:base(name)
	{
		string registerCode = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUBWXYZ";
		REGISTER_CODE = BinaryUtility.stringToBytes(registerCode);
		CODE_LEN = REGISTER_CODE.Length;
	}
	public string generateRequestCode()
	{
		string systemInfo = mHardwareInfo.mHDD + mHardwareInfo.mMainbord + mHardwareInfo.mCPU + mHardwareInfo.mBIOS + mHardwareInfo.mMainbordType;
		return getMD5(systemInfo);
	}
	// 生成注册码
	public string generateRegisteCode(string requestCode, string encodeKey)
	{
		byte[] encodeBytes = BinaryUtility.stringToBytes(getMD5(encodeKey));
		// 再次计算MD5
		string retStr = getMD5(requestCode);
		// 然后再加密
		retStr = encode(retStr, encodeBytes);
		return retStr;
	}
	protected string encode(string str, byte[] encodeKeyBytes)
	{
		byte[] strBytes = BinaryUtility.stringToBytes(str);
		int byteLen = strBytes.Length;
		int encodeKeyLen = encodeKeyBytes.Length;
		for (int i = 0; i < byteLen; ++i)
		{
			sbyte oriByte = (sbyte)strBytes[i];
			sbyte encodeByte = (sbyte)encodeKeyBytes[i % encodeKeyLen];
			oriByte ^= encodeByte;
			strBytes[i] = REGISTER_CODE[Mathf.Abs((oriByte + encodeByte) + 0xff) % CODE_LEN];
		}
		str = BinaryUtility.bytesToString(strBytes);
		return str;
	}
	protected string getMD5(string source)
	{
		MD5 md5 = new MD5CryptoServiceProvider();
		byte[] sourceBytes = BinaryUtility.stringToBytes(source);
		byte[] result = md5.ComputeHash(sourceBytes);
		string md5Value = BinaryUtility.bytesToHEXString(result, false, false);
		return md5Value;
	}
}