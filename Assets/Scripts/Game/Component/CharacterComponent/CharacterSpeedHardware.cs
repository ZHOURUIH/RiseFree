using UnityEngine;
using System;
using System.Collections;

public class CharacterSpeedHardware : GameComponent
{
	protected float mNormalAcceleration;	// 默认的从当前速度加速到目标速度的快慢
	protected float mAcceleration;			// 实际的从当前速度加速到目标速度的快慢,如果与默认值不一致,则会逐渐向默认值靠近
	protected float mAccelerationFactor;	// mCurAddSpeedFactor向mNormalAddSpeedFactor靠近的快慢
	protected float mMinDeltaSpeed;			// 最小速度差,当速度差小于该值时就直接设置为目标速度
	protected float mTargetSpeed;           // 当前的目标速度
	protected CharacterOther mCharacter;
	protected CharacterData mData;
	public CharacterSpeedHardware(Type type, string name)
		:
	   base(type, name)
	{
		mTargetSpeed = 0.0f;
		mMinDeltaSpeed = 0.1f;
		mNormalAcceleration = 15.0f;
		mAcceleration = mNormalAcceleration;
		mAccelerationFactor = 0.015f;
	}
	public override void init(ComponentOwner owner)
	{
		base.init(owner);
		mCharacter = mComponentOwner as CharacterOther;
		mData = mCharacter.getCharacterData();
	}
	public override void update(float elaspedTime)
	{
		if (!MathUtility.isFloatEqual(mData.mSpeed, mTargetSpeed))
		{
			if(!MathUtility.isFloatEqual(mNormalAcceleration, mAcceleration))
			{
				mAcceleration = MathUtility.lerp(mAcceleration, mNormalAcceleration, mAccelerationFactor * elaspedTime);
			}
			// 当前速度逐渐向目标速度靠近
			float speed = MathUtility.lerp(mData.mSpeed, mTargetSpeed, mAcceleration * elaspedTime);
			// 小于最小间隔时直接设置为目标速度
			if((Mathf.Abs(speed - mTargetSpeed) < mMinDeltaSpeed))
			{
				speed = mTargetSpeed;
			}
			// 如果速度降到了0(小于0.01则认为是0),并且角色当前正在跑动,则通知角色停下
			if (MathUtility.isFloatZero(speed, 0.01f) && mCharacter.hasState(PLAYER_STATE.PS_RIDING))
			{
				pushCommand<CommandCharacterStop>(mCharacter);
			}
			// 速度不为0,并且角色当前正处于休闲状态,则通知角色跑动
			else if (speed > 0.0f && mCharacter.hasState(PLAYER_STATE.PS_IDLE))
			{
				pushCommand<CommandCharacterStartRun>(mCharacter);
			}
			// 加速
			else if (speed > mData.mSpeed)
			{
				CommandCharacterSpeedUp cmdSpeedUp = newCmd(out cmdSpeedUp, false);
				cmdSpeedUp.mSpeed = speed;
				cmdSpeedUp.mSpeedDelta = speed - mData.mSpeed;
				pushCommand(cmdSpeedUp, mCharacter);
			}
			// 减速
			else if (speed < mData.mSpeed)
			{
				CommandCharacterSpeedSlow cmdSpeedSlow = newCmd(out cmdSpeedSlow, false);
				cmdSpeedSlow.mSpeed = speed;
				pushCommand(cmdSpeedSlow, mCharacter);
			}

			// 改变速度
			CommandCharacterChangeSpeed cmd = newCmd(out cmd, false);
			cmd.mSpeed = speed;
			pushCommand(cmd, mCharacter);
		}
		base.update(elaspedTime);
	}
	public float getTargetSpeed() { return mTargetSpeed; }
	public void setHardwareSpeed(float speed, bool directSpeed = false)
	{
		if (speed < 0.0f)
		{
			speed = 0.0f;
		}
		mTargetSpeed = speed;
		if (directSpeed)
		{
			CommandCharacterChangeSpeed cmd = newCmd(out cmd,false);
			cmd.mSpeed = mTargetSpeed;
			pushCommand(cmd, mComponentOwner);
		}
	}
	public void setAcceleration(float factor){ mAcceleration = factor; }
	//------------------------------------------------------------------------------------------------------------------------------
	protected override void setBaseType() { mBaseType = typeof(CharacterSpeedHardware); }
	protected override bool isType(Type type) { return type == typeof(CharacterSpeedHardware); }
}
