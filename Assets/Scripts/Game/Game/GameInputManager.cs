using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Net;

public enum KEY_STATE
{
	KS_CURRENT_DOWN,
	KS_CURRENT_UP,
	KS_KEEP_DOWN,
	KS_KEEP_UP,
}

public class GameInputManager : InputManager
{
	protected float mLastStickAngle = 0.0f;
	protected float mStickAngle = 0.0f;			// 轴的转向角度,向左为负,向右为正,范围-90~90
	protected bool mTurnLeft = false;			// 是否做了左转操作,只检测单次左转操作
	protected bool mTurnRight = false;			// 是否做了右转操作,只检测单次右转操作
	protected float mTurnThreshold = 15.0f;     // 转向判断阈值
	protected float mTurnAngleOffset = 0.0f;    // 角度矫正值
	protected float mStickTurnSpeed = 90.0f;    // 使用键盘模拟时转向的速度
	protected float mStickRevertSpeed = 360.0f;	// 转向回弹速度
	protected Dictionary<KeyCode, KEY_STATE> mKeyState; // 按键状态列表
	protected bool mKeyboardEnable;
	protected bool mDeviceConnected;			// 保存设备的连接状态,提高访问效率
	public GameInputManager(string name)
		:base(name)
	{
		mKeyState = new Dictionary<KeyCode, KEY_STATE>();
		mKeyState.Add(KeyCode.A, KEY_STATE.KS_KEEP_UP);
		mKeyState.Add(KeyCode.B, KEY_STATE.KS_KEEP_UP);
		mKeyState.Add(KeyCode.X, KEY_STATE.KS_KEEP_UP);
		mKeyState.Add(KeyCode.Y, KEY_STATE.KS_KEEP_UP);
	}
	public override void init()
	{
		base.init();
		mKeyboardEnable = (int)mGameConfig.getFloatParam(GAME_DEFINE_FLOAT.GDF_KEYBOARD_ENABLE) != 0;
		mTurnThreshold = mGameConfig.getFloatParam(GAME_DEFINE_FLOAT.GDF_TURN_THRESHHOLD);
		mTurnAngleOffset = mGameConfig.getFloatParam(GAME_DEFINE_FLOAT.GDF_TURN_ANGLE_OFFSET);
	}
	public override void destroy()
	{
		base.destroy();
	}
	public override void update(float elapsedTime)
	{
		base.update(elapsedTime);
		mDeviceConnected = mSerialPortManager.isDeviceConnect();
		// 此处只能判断设备是否连接,否则未按下键盘时,自动回弹逻辑会影响设备转向
		if (!mDeviceConnected)
		{
			// 使用键盘模拟转向
			bool leftDown = base.getKeyDown(KeyCode.LeftArrow);
			bool rightDown = base.getKeyDown(KeyCode.RightArrow);
			// 键盘左方向键按下
			if (leftDown)
			{
				mStickAngle -= mStickTurnSpeed * elapsedTime;
			}
			// 键盘右方向键按下
			if (rightDown)
			{
				mStickAngle += mStickTurnSpeed * elapsedTime;
			}
			MathUtility.clamp(ref mStickAngle, -90.0f, 90.0f);
			// 方向键未按下,则回弹到中间位置
			if (!leftDown && !rightDown)
			{
				if (Mathf.Abs(mStickAngle) < mStickRevertSpeed * elapsedTime)
				{
					mStickAngle = 0.0f;
				}
				else
				{
					if (mStickAngle > 0.0f)
					{
						mStickAngle -= mStickRevertSpeed * elapsedTime;
					}
					else if (mStickAngle < 0.0f)
					{
						mStickAngle += mStickRevertSpeed * elapsedTime;
					}
				}
			}
		}
		mTurnLeft = mLastStickAngle > -mTurnThreshold && mStickAngle < -mTurnThreshold;
		mTurnRight = mLastStickAngle < mTurnThreshold && mStickAngle > mTurnThreshold;
		mLastStickAngle = mStickAngle;
	}
	public bool isLeft()
	{
		return mStickAngle < -mTurnThreshold;
	}
	public bool isRight()
	{
		return mStickAngle > mTurnThreshold;
	}
	public bool turnLeft()
	{
		return mTurnLeft;
	}
	public bool turnRight()
	{
		return mTurnRight;
	}
	public void setStickAngle(float stickAngle)
	{
		mStickAngle = stickAngle;
		// 再使用偏移值校正
		mStickAngle -= mTurnAngleOffset;
	}
	public float getStickAngle()
	{
		return mStickAngle;
	}
	public override bool getKeyCurrentDown(KeyCode key)
	{
		bool ret = false;
		// 先判断设备按键
		if (mKeyState.ContainsKey(key))
		{
			ret = (mKeyState[key] == KEY_STATE.KS_CURRENT_DOWN);
		}
		// 如果设备按键未按下,并且可以读取键盘消息,则先读取键盘消息
		if (!ret && keyboardEnabled())
		{
			ret = base.getKeyCurrentDown(key);
		}
		return ret;
	}
	public override bool getKeyCurrentUp(KeyCode key)
	{
		bool ret = false;
		// 先判断设备按键
		if (mKeyState.ContainsKey(key))
		{
			ret = (mKeyState[key] == KEY_STATE.KS_CURRENT_UP);
		}
		// 如果设备按键未按下,并且可以读取键盘消息,则先读取键盘消息
		if (!ret && keyboardEnabled())
		{
			ret = base.getKeyCurrentUp(key);
		}
		return ret;
	}
	public override bool getKeyDown(KeyCode key)
	{
		bool ret = false;
		// 先判断设备按键
		if (mKeyState.ContainsKey(key))
		{
			ret = (mKeyState[key] == KEY_STATE.KS_KEEP_DOWN || mKeyState[key] == KEY_STATE.KS_CURRENT_DOWN);
		}
		// 如果设备按键未按下,并且可以读取键盘消息,则先读取键盘消息
		if (!ret && keyboardEnabled())
		{
			ret = base.getKeyDown(key);
		}
		return ret;
	}
	public override bool getKeyUp(KeyCode key)
	{
		bool ret = false;
		// 先判断设备按键
		if (mKeyState.ContainsKey(key))
		{
			ret = (mKeyState[key] == KEY_STATE.KS_KEEP_UP || mKeyState[key] == KEY_STATE.KS_CURRENT_UP);
		}
		// 如果设备按键未按下,并且可以读取键盘消息,则先读取键盘消息
		if (!ret && keyboardEnabled())
		{
			ret = base.getKeyUp(key);
		}
		return ret;
	}
	public void setKeyState(KeyCode key, bool state)
	{
		if (!mKeyState.ContainsKey(key))
		{
			return;
		}
		if (state)
		{
			if (mKeyState[key] == KEY_STATE.KS_KEEP_UP || mKeyState[key] == KEY_STATE.KS_CURRENT_UP)
			{
				mKeyState[key] = KEY_STATE.KS_CURRENT_DOWN;
			}
			else if (mKeyState[key] == KEY_STATE.KS_CURRENT_DOWN)
			{
				mKeyState[key] = KEY_STATE.KS_KEEP_DOWN;
			}
		}
		else
		{
			if (mKeyState[key] == KEY_STATE.KS_KEEP_DOWN || mKeyState[key] == KEY_STATE.KS_CURRENT_DOWN)
			{
				mKeyState[key] = KEY_STATE.KS_CURRENT_UP;
			}
			else if (mKeyState[key] == KEY_STATE.KS_CURRENT_UP)
			{
				mKeyState[key] = KEY_STATE.KS_KEEP_UP;
			}
		}
	}
	//--------------------------------------------------------------------------------------------------------------------------------------
	protected bool keyboardEnabled()
	{
		return mKeyboardEnable || !mDeviceConnected;
	}
}