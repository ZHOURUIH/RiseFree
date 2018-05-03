using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
public class ScriptDebugInfo : LayoutScript
{
	protected txUIObject mGrid;
	protected txNGUIText mResistance;
	protected txNGUIText mSpeed;
	protected txNGUIText mPitchAngle;
	protected txNGUIText mDevice;
	protected txNGUIText mLastPacketTime;
	protected txNGUIText mOriginStickAngle;
	protected txNGUIText mRPM;
	protected txNGUIText mPower;
	protected DEVICE_CONNENT mDeviceConnect = DEVICE_CONNENT.DC_MAX;
	public ScriptDebugInfo(string name, GameLayout layout)
		:
		base(name, layout)
	{
		;
	}
	public override void assignWindow()
	{
		newObject(out mGrid, "Grid1");
		newObject(out mResistance, mGrid, "Resistance");
		newObject(out mSpeed, mGrid, "Speed");
		newObject(out mPitchAngle, mGrid, "PitchAngle");
		newObject(out mDevice, mGrid, "Device");
		newObject(out mLastPacketTime, mGrid, "LastPacketTime");
		newObject(out mOriginStickAngle, mGrid, "OriginStickAngle");
		newObject(out mRPM, mGrid, "RPM");
		newObject(out mPower, mGrid, "Power");
	}
	public override void init()
	{
		;
	}
	public override void onReset()
	{
		base.onReset();
	}
	public override void onShow(bool immediately, string param)
	{
		;
	}
	public void notityFriction(int friction)
	{
		mResistance.setLabel(StringUtility.intToString(friction));
	}
	public void notitySpeed(float speed)
	{
		mSpeed.setLabel(StringUtility.floatToString(speed, 3));
	}
	public void notityPitch(float pitch)
	{
		mPitchAngle.setLabel(StringUtility.floatToString(pitch, 3));
	}
	public void setDevice(DEVICE_CONNENT device)
	{
		string str = "";
		if (device == DEVICE_CONNENT.DC_NONE)
		{
			str = "未连接";
		}
		else if (device == DEVICE_CONNENT.DC_PROCEED)
		{
			str = "正在连接";
		}
		else if (device == DEVICE_CONNENT.DC_SUCCESS)
		{
			str = "已连接";
		}
		else if (device == DEVICE_CONNENT.DC_CLOSE)
		{
			str = "已断开";
		}
		mDevice.setLabel(str);
	}
	public void setRPM(int rpm)
	{
		mRPM.setLabel(StringUtility.intToString(rpm));
	}
	public void setPower(int power)
	{
		mPower.setLabel(StringUtility.intToString(power));
	}
	public void setOrginStackAngle(float stackAngle)
	{
		mOriginStickAngle.setLabel(StringUtility.floatToString(stackAngle,3));
	}
	public void setTimeSinceLastPacket(int timeMS)
	{
		if (mDeviceConnect == DEVICE_CONNENT.DC_SUCCESS)
		{
			float timeS = timeMS / 1000.0f;
			mLastPacketTime.setLabel(StringUtility.floatToString(timeS, 3));
		}
		else
		{
			mLastPacketTime.setLabel(StringUtility.floatToString(0.0f));
		}
	}
	public override void update(float elapsedTime)
	{
		base.update(elapsedTime);
		if (mDeviceConnect != mUSBManager.getDeviceConnect())
		{
			mDeviceConnect = mUSBManager.getDeviceConnect();
			setDevice(mDeviceConnect);
		}
		TimeSpan delta = DateTime.Now - mUSBManager.getLastPacketTime();
		setTimeSinceLastPacket((int)delta.TotalMilliseconds);
	}
	public override void onHide(bool immediately, string param)
	{
		;
	}
}

