using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

// AI控制器,用于计算AI的行为
public class CharacterControllerAI : CharacterController
{
	protected CharacterOther mCharacter;
	protected CharacterData mData;
	protected float mTargetDistanceOffset;
	public CharacterControllerAI(Type type, string name)
		:base(type, name)
	{
		mTargetDistanceOffset = 10.0f;
	}
	public override void init(ComponentOwner owner)
	{
		base.init(owner);
		mCharacter = owner as CharacterOther;
		mData = mCharacter.getCharacterData();
	}
	public override void update(float elapsedTime)
	{
		base.update(elapsedTime);
		int targetIndex = mWayPointManager.getPointIndexFromDistance(mData.mRunDistance + mTargetDistanceOffset, mData.mCurWayPoint);
		float targetDirection = mWayPointManager.getPointDirection(targetIndex);
		// 自身的朝向往目标点所在路段的朝向靠拢
		float curDirection = mCharacter.getRotation().y;
		MathUtility.adjustAngle180(ref curDirection);
		float dirDelta = targetDirection - curDirection;
		// 如果目标方向与当前方向的差值超过180,则转换到0-360再计算
		if (Mathf.Abs(dirDelta) > 180.0f)
		{
			MathUtility.adjustAngle360(ref curDirection);
			MathUtility.adjustAngle360(ref targetDirection);
			dirDelta = targetDirection - curDirection;
		}
		mData.mTurnAngle = MathUtility.lerp(mData.mTurnAngle, dirDelta, 0.1f);
		float curTargetSpeed = 8.0f + mData.mNumber;
		CharacterSpeedHardware speedHardware = mCharacter.getFirstComponent<CharacterSpeedHardware>();
		if (!MathUtility.isFloatEqual(speedHardware.getTargetSpeed(), curTargetSpeed) && mCharacter.getProcessExternalSpeed())
		{
			CommandCharacterHardwareSpeed cmd = newCmd(out cmd);
			cmd.mSpeed = curTargetSpeed;
			cmd.mExternalSpeed = true;
			pushCommand(cmd, mCharacter);
		}
		List<CharacterOther> allCharacterList = mRoleSystem.getAllCharacterList();
		// 如果是AI,则一旦发现视野内有人,就会发射导弹
		foreach (CharacterOther item in allCharacterList)
		{
			if (item != mCharacter)
			{
				Vector3 mPos = item.getWorldPosition();
				if (MathUtility.isInRange(item.getCharacterData().mRunDistance - mCharacter.getCharacterData().mRunDistance, 0.0f, 100.0f))
				{
					if (UnityUtility.whetherGameObjectInScreen(mPos))
					{
						if (mCharacter.getStateMachine().hasState(PLAYER_STATE.PS_AIM))
						{
							// 给角色添加瞄准状态
							CommandCharacterAddState cmdCharacterAddState = newCmd(out cmdCharacterAddState);
							cmdCharacterAddState.mState = PLAYER_STATE.PS_AIM;
							pushCommand(cmdCharacterAddState, mCharacter);
							break;
						}	
					}
				}
			}
		}
	}
	public void notifyAIGetBoxItem(PlayerItemBase playerItem )
	{
		if (playerItem.getItemType() == PLAYER_ITEM.PI_MISSILE)
		{
			// 如果已经在瞄准状态中,则不能使用
			if (mCharacter.hasState(PLAYER_STATE.PS_AIM))
			{
				return;
			}
			// 给角色添加瞄准状态
			CommandCharacterAddState cmdCharacterAddState = newCmd(out cmdCharacterAddState);
			cmdCharacterAddState.mState = PLAYER_STATE.PS_AIM;
			pushCommand(cmdCharacterAddState, mCharacter);
		}
		else
		{
			CommandCharacterUseItem cmd = newCmd(out cmd);
			cmd.mItemIndex = mCharacter.getPlayerPack().getSelectedIndex();
			pushCommand(cmd, mCharacter);
		}
	}
	//-----------------------------------------------------------------------------------------------------------------------------------
	protected override bool isType(Type type)
	{
		return base.isType(type) || type == typeof(CharacterControllerAI);
	}
}