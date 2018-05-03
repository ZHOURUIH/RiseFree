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
	protected float mOriginStickAngle = 0.0f;   // 轴的转向角度,向左为负,向右为正,范围-90~90,未矫正的值
	protected float mStickAngle = 0.0f;			// 轴的转向角度,向左为负,向右为正,范围-90~90,矫正后的值
	protected bool mTurnLeft = false;			// 是否做了左转操作,只检测单次左转操作
	protected bool mTurnRight = false;			// 是否做了右转操作,只检测单次右转操作
	protected float mTurnThreshold = 15.0f;     // 转向判断阈值
	protected float mTurnAngleOffset = 0.0f;    // 角度矫正值
	protected float mStickTurnSpeed = 90.0f;    // 使用键盘模拟时转向的速度
	protected float mStickRevertSpeed = 360.0f;	// 转向回弹速度
	protected Dictionary<KeyCode, KEY_STATE> mKeyState;	// 按键状态列表
	protected Dictionary<KeyCode, bool> mKeyStateCache;	// 按键状态列表缓存,用于接收当前按键设置
	protected bool mKeyboardEnable;
	protected bool mDeviceConnected;            // 保存设备的连接状态,提高访问效率
	protected float mTurnSensitive = 1.0f;
	public GameInputManager(string name)
		:base(name)
	{
		mKeyState = new Dictionary<KeyCode, KEY_STATE>();
		mKeyState.Add(KeyCode.A, KEY_STATE.KS_KEEP_UP);
		mKeyState.Add(KeyCode.B, KEY_STATE.KS_KEEP_UP);
		mKeyState.Add(KeyCode.X, KEY_STATE.KS_KEEP_UP);
		mKeyState.Add(KeyCode.Y, KEY_STATE.KS_KEEP_UP);
		mKeyStateCache = new Dictionary<KeyCode, bool>();
		mKeyStateCache.Add(KeyCode.A, false);
		mKeyStateCache.Add(KeyCode.B, false);
		mKeyStateCache.Add(KeyCode.X, false);
		mKeyStateCache.Add(KeyCode.Y, false);
	}
	public override void init()
	{
		base.init();
		mKeyboardEnable = (int)mGameConfig.getFloatParam(GAME_DEFINE_FLOAT.GDF_KEYBOARD_ENABLE) != 0;
		mTurnThreshold = mGameConfig.getFloatParam(GAME_DEFINE_FLOAT.GDF_TURN_THRESHOLD);
		mTurnAngleOffset = mGameConfig.getFloatParam(GAME_DEFINE_FLOAT.GDF_TURN_ANGLE_OFFSET);
		mTurnSensitive = mGameConfig.getFloatParam(GAME_DEFINE_FLOAT.GDF_TURN_SENSITIVE);
	}
	public override void destroy()
	{
		base.destroy();
	}
	public override void update(float elapsedTime)
	{
		base.update(elapsedTime);
		mDeviceConnected = mUSBManager.isDeviceConnect();
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
		// 更新按键状态
		List<KeyCode> keys = new List<KeyCode>(mKeyState.Keys);
		int count = keys.Count;
		for(int i = 0; i < count; ++i)
		{
			if (mKeyStateCache[keys[i]])
			{
				if (mKeyState[keys[i]] == KEY_STATE.KS_CURRENT_UP || mKeyState[keys[i]] == KEY_STATE.KS_KEEP_UP)
				{
					mKeyState[keys[i]] = KEY_STATE.KS_CURRENT_DOWN;
				}
				else if (mKeyState[keys[i]] == KEY_STATE.KS_CURRENT_DOWN || mKeyState[keys[i]] == KEY_STATE.KS_KEEP_DOWN)
				{
					mKeyState[keys[i]] = KEY_STATE.KS_KEEP_DOWN;
				}
			}
			else
			{
				if (mKeyState[keys[i]] == KEY_STATE.KS_CURRENT_UP || mKeyState[keys[i]] == KEY_STATE.KS_KEEP_UP)
				{
					mKeyState[keys[i]] = KEY_STATE.KS_KEEP_UP;
				}
				else if (mKeyState[keys[i]] == KEY_STATE.KS_CURRENT_DOWN || mKeyState[keys[i]] == KEY_STATE.KS_KEEP_DOWN)
				{
					mKeyState[keys[i]] = KEY_STATE.KS_CURRENT_UP;
				}
			}
		}
		mTurnLeft = mLastStickAngle * mTurnSensitive > -mTurnThreshold && mStickAngle * mTurnSensitive < -mTurnThreshold;
		mTurnRight = mLastStickAngle * mTurnSensitive < mTurnThreshold && mStickAngle * mTurnSensitive > mTurnThreshold;
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
		mOriginStickAngle = stickAngle;
		// 再使用偏移值校正
		mStickAngle = mOriginStickAngle - mTurnAngleOffset;
	}
	public float getStickAngle()
	{
		return mStickAngle;
	}
	public float getOriginStickAngle()
	{
		return mOriginStickAngle;
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
		if (!mKeyStateCache.ContainsKey(key))
		{
			return;
		}
		mKeyStateCache[key] = state;
	}
	//--------------------------------------------------------------------------------------------------------------------------------------
	protected bool keyboardEnabled()
	{
		return mKeyboardEnable || !mDeviceConnected;
	}
}