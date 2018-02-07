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
		// 如果AI背包中有导弹,则一直搜寻可以瞄准的目标
		int missileIndex = mCharacter.getPlayerPack().getFirstItemIndex(PLAYER_ITEM.PI_MISSILE);
		if (missileIndex != -1 && !mCharacter.hasState(PLAYER_STATE.PS_AIM))
		{
			bool hasAvailbleTarget = false;
			List<CharacterOther> allCharacterList = mRoleSystem.getAllCharacterList();
			float playerDistance = mCharacter.getCharacterData().mRunDistance;
			foreach (CharacterOther item in allCharacterList)
			{
				if (item != mCharacter)
				{
					float curDistance = item.getCharacterData().mRunDistance - playerDistance;
					if (MathUtility.isInRange(curDistance, 0.0f, GameDefine.MAX_LAUNCH_MISSILE_DISTANCE))
					{
						if (UnityUtility.whetherGameObjectInScreen(item.getWorldPosition()))
						{
							hasAvailbleTarget = true;
							break;
						}
					}
				}
			}
			if (hasAvailbleTarget)
			{
				// 需要选中导弹
				CommandCharacterSelectItem cmdSelect = newCmd(out cmdSelect);
				cmdSelect.mIndex = missileIndex;
				pushCommand(cmdSelect, mCharacter);
				// 给角色添加瞄准状态
				CommandCharacterAddState cmdCharacterAddState = newCmd(out cmdCharacterAddState);
				cmdCharacterAddState.mState = PLAYER_STATE.PS_AIM;
				pushCommand(cmdCharacterAddState, mCharacter);
			}
		}
	}
	public void notifyAIGetBoxItem(PlayerItemBase playerItem )
	{
		if (playerItem.getItemType() != PLAYER_ITEM.PI_MISSILE)
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