﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class CommandCameraLinkTarget : Command
{
	public MovableObject mTarget;
	public CAMERA_LINKER_SWITCH mSwitchType;
	public string mLinkerName;
	public bool mLookAtTarget;				// 是否始终看向目标
	public Vector3 mLookatOffset;           // 看向目标的位置偏移
	public bool mAutoProcessKey;            // 是否在断开连接器后可以使用按键控制摄像机
	public bool mImmediately;				// 是否直接将摄像机设置到当前连接器的正常位置
	protected bool mUseOriginRelative;		// 是否使用连接器原来的相对位置
	protected Vector3 mRelativePosition;	// 相对位置
	protected bool mUseLastSwitchSpeed;		// 是否使用当前连接器的速度
	protected float mSwitchSpeed;           // 转换器的速度
	public override void init()
	{
		base.init();
		mTarget = null;
		mSwitchType = CAMERA_LINKER_SWITCH.CLS_NONE;
		mLinkerName = "";
		mLookAtTarget = true;
		mLookatOffset = Vector3.zero;
		mUseOriginRelative = true;
		mRelativePosition = Vector3.zero;
		mUseLastSwitchSpeed = true;
		mSwitchSpeed = 10.0f;
		mAutoProcessKey = false;
		mImmediately = false;
	}
	public void setRelativePosition(Vector3 relative)
	{
		mUseOriginRelative = false;
		mRelativePosition = relative;
	}
	public void setSwitchSpeed(float speed)
	{
		mUseLastSwitchSpeed = false;
		mSwitchSpeed = speed;
	}
	public override void execute()
	{
		GameCamera camera = (mReceiver) as GameCamera;
		camera.linkTarget(mLinkerName, mTarget);
		if (mTarget != null)
		{
			CameraLinker linker = camera.getComponent(mLinkerName) as CameraLinker;
			if (linker != null)
			{
				linker.setLookAtTarget(mLookAtTarget);
				linker.setLookAtOffset(mLookatOffset);
				// 不使用原来的相对位置,则设置新的相对位置
				if (!mUseOriginRelative)
				{	
					linker.setRelativePosition(mRelativePosition, mSwitchType, mUseLastSwitchSpeed, mSwitchSpeed);
				}
				if(mImmediately)
				{
					linker.applyRelativePosition(linker.getNormalRelativePosition());
				}
			}
			else if(mLinkerName != "")
			{
				UnityUtility.logError("linker name is not empty, but can not find linker, name : " + mLinkerName);
			}
		}
	}
	public override string showDebugInfo()
	{
		return this.GetType().ToString();
	}
};