using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

// 自由角色控制器
public class CharacterControllerFree : CharacterController
{
	protected Character mCharacter;
	protected float mTargetSpeed;	// 目标速度
	protected float mFixedTime;		// 从当前速度加速到目标速度所需要的时间
	protected Vector3 mMoveDir;     // 根据当前的按键来判断移动方向
	protected float mTargetWorldYaw;
	protected float mMinDeltaSpeed;
	protected float mMinDeltaRotation;
	protected float mForwardYaw;
	public CharacterControllerFree(Type type, string name)
		:base(type, name)
	{
		mFixedTime = 0.1f;
		mMinDeltaSpeed = 0.1f;
		mMinDeltaRotation = 5.0f;
		mForwardYaw = 0.0f;
	}
	public override void init(ComponentOwner owner)
	{
		base.init(owner);
		mCharacter = mComponentOwner as Character;
	}
	public override void update(float elapsedTime)
	{
		base.update(elapsedTime);
		processKeyboard(elapsedTime);
		processMouse(elapsedTime);
	}
	//------------------------------------------------------------------------------------------------------------------
	protected override bool isType(Type type)
	{
		return base.isType(type) || type == typeof(CharacterControllerFree);
	}
	protected void processKeyboard(float elapsedTime)
	{
		bool redirection = false;
		bool run = false;
		mTargetSpeed = 0.0f;
		// 有方向键按下和放开就重新判断目标速度方向
		if (mInputManager.getKeyDown(KeyCode.W) ||
			mInputManager.getKeyDown(KeyCode.A) ||
			mInputManager.getKeyDown(KeyCode.S) ||
			mInputManager.getKeyDown(KeyCode.D))
		{
			redirection = true;
			mTargetSpeed = 5.0f;
		}
		if(mInputManager.getKeyDown(KeyCode.LeftShift))
		{
			run = true;
			mTargetSpeed *= 2.0f;
		}
		// 有按键放开时,还有被按下的按键时才会重新判断方向
		if (redirection && (
			mInputManager.getKeyUp(KeyCode.W) ||
			mInputManager.getKeyUp(KeyCode.A) ||
			mInputManager.getKeyUp(KeyCode.S) ||
			mInputManager.getKeyUp(KeyCode.D)))
		{
			redirection = true;
		}
		if (redirection)
		{
			mMoveDir = Vector3.zero;
			if (mInputManager.getKeyDown(KeyCode.W))
			{
				mMoveDir += Vector3.forward;
			}
			if (mInputManager.getKeyDown(KeyCode.S))
			{
				mMoveDir += Vector3.back;
			}
			if (mInputManager.getKeyDown(KeyCode.A))
			{
				mMoveDir += Vector3.left;
			}
			if (mInputManager.getKeyDown(KeyCode.D))
			{
				mMoveDir += Vector3.right;
			}
			mMoveDir = mMoveDir.normalized;
		}
		// 计算当前速度
		// 速度发生改变时,当前速度逐渐向目标速度靠近
		float curSpeed = mCharacter.getSpeed().magnitude;
		smoothTarget(mFixedTime, elapsedTime, mMinDeltaSpeed, mTargetSpeed, ref curSpeed);
		//mCharacter.setCurMoveSpeed(curSpeed);
		onSpeed(curSpeed, run);
		// 如果速度为0,则不再继续判断
		if (MathUtility.isFloatZero(mTargetSpeed))
		{
			return;
		}
		// 方向,所有都是角度制的
		mForwardYaw = mCameraManager.getMainCamera().getRotation().y;
		float dirYaw = 0.0f, pitch = 0.0f;
		MathUtility.getDegreeYawPitchFromDirection(mMoveDir, ref dirYaw, ref pitch);
		mTargetWorldYaw = mForwardYaw + dirYaw;
		MathUtility.adjustAngle360(ref mTargetWorldYaw);
		Vector3 curEuler = mCharacter.getWorldRotation();
		// 差值大于180度,则转换为-180到180之间
		if (Mathf.Abs(mTargetWorldYaw - curEuler.y) >= 180.0f)
		{
			MathUtility.adjustAngle180(ref mTargetWorldYaw);
			MathUtility.adjustAngle180(ref curEuler.y);
			smoothTarget(mFixedTime, elapsedTime, mMinDeltaRotation, mTargetWorldYaw, ref curEuler.y);
			MathUtility.adjustAngle360(ref mTargetWorldYaw);
			MathUtility.adjustAngle360(ref curEuler.y);
		}
		else
		{
			smoothTarget(mFixedTime, elapsedTime, mMinDeltaRotation, mTargetWorldYaw, ref curEuler.y);
		}
		mCharacter.setWorldRotation(curEuler);

		Vector3 worldDir = MathUtility.getDirectionFromDegreeYawPitch(mTargetWorldYaw, 0.0f);
		mCharacter.move(worldDir * curSpeed * elapsedTime * 0.3f, Space.World);
	}
	protected void processMouse(float elapsedTime)
	{
		if(mInputManager.getMouseDown(MOUSE_BUTTON.MB_RIGHT))
		{
			Vector2 delta = mInputManager.getMouseDelta();
			if (!MathUtility.isFloatZero(delta.x))
			{
				GameCamera mainCamera = mCameraManager.getMainCamera();
				Vector3 relative = mainCamera.getCurLinker().getRelativePosition();
				relative = MathUtility.rotateVector3(relative, -delta.x * Mathf.Deg2Rad * 0.2f);
				mainCamera.getCurLinker().setRelativePosition(relative);
			}
		}
	}
	protected static void smoothTarget(float fixedTime, float elapsedTime, float minDelta, float target, ref float cur)
	{
		float delta = target - cur;
		if (Mathf.Abs(delta) > minDelta)
		{
			float added = delta / fixedTime * elapsedTime;
			cur = Mathf.Abs(added) >= Mathf.Abs(delta) ? target : cur + added;
		}
		else
		{
			cur = target;
		}
	}
	protected virtual void onSpeed(float speed, bool run)
	{
		//Animation anim = mCharacter.getAvatar().mAnimation;
		//if (speed > 0.0f)
		//{
		//	if (run)
		//	{
		//		anim.CrossFade("Move_Sprint450");
		//	}
		//	else
		//	{
		//		anim.CrossFade("Move_Walk225");
		//	}
		//}
		//else
		//{
		//	anim.CrossFade("Idle_Combat_Base");
		//}
	}
}