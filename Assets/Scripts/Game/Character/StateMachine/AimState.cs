using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 瞄准状态
public class AimState : PlayerState
{
	protected Character mAimTarget;
	protected int aimCount;
	protected List<CharacterOther> aimCharacterList;
	protected Vector3 mCharacterTopHead;
	public AimState(PLAYER_STATE type)
		:
		base(type)
	{
		aimCharacterList = new List<global::CharacterOther>();
		mCharacterTopHead = new Vector3(0.0f,2.0f,0.0f);
	}
	public override bool canEnter()
	{
		// 检查角色是否有导弹,没有导弹不能瞄准
		PlayerItemBase playerItem = mPlayer.getPlayerPack().getCurItem();
		if (playerItem == null || playerItem.getItemType() != PLAYER_ITEM.PI_MISSILE)
		{
			return false;
		}
		return true;
	}
	public override void enter()
	{
		// 显示瞄准图标,只有玩家自己进入瞄准状态才显示
		if (mPlayer.isType(CHARACTER_TYPE.CT_MYSELF))
		{
			LayoutTools.LOAD_LAYOUT_SHOW(LAYOUT_TYPE.LT_AIMING, 20);
		}
	}
	public override void update(float elapsedTime)
	{
		base.update(elapsedTime);
		List<CharacterOther> allCharacterList = mRoleSystem.getAllCharacterList();
		aimCount = aimCharacterList.Count;
		float curDistance = 0.0f ;
		foreach (CharacterOther item in allCharacterList)
		{
			if (item != mPlayer)
			{
				curDistance = item.getCharacterData().mRunDistance - mPlayer.getCharacterData().mRunDistance;
				if (MathUtility.isInRange(curDistance, 0.0f,GameDefine.MAX_LAUNCH_MISSILE_DISTANCE))
				{
					if (UnityUtility.whetherGameObjectInScreen(item.getWorldPosition()))
					{
						if (!aimCharacterList.Contains(item))
						{
							aimCharacterList.Add(item);
						}
					}
				}
				else
				{
					if (aimCharacterList.Contains(item))
					{
						aimCharacterList.Remove(item);
					}
				}
			}
		}
		if (aimCharacterList.Count == 0)
		{
			if (mPlayer.isType(CHARACTER_TYPE.CT_MYSELF))
			{
				Vector2 screenPos = UnityUtility.worldPosToScreenPos(mPlayer.getWorldPosition()+mCharacterTopHead);
				mScriptAiming.setAiming(screenPos, mScriptAiming.getOriginHeight(),false);
				mAimTarget = null;
			}
		}
		else
		{
			int aim = MathUtility.randomInt(0, aimCharacterList.Count - 1);
			if (mPlayer.isType(CHARACTER_TYPE.CT_MYSELF))
			{
				if (aimCount != aimCharacterList.Count)
				{
					mAimTarget = aimCharacterList[aim];
				}
				Vector2 screenPos = UnityUtility.worldPosToScreenPos(mAimTarget.getWorldPosition());
				Vector2 screenPosHead= UnityUtility.worldPosToScreenPos(mAimTarget.getWorldPosition() + mCharacterTopHead);
				float distance = MathUtility.getLength(screenPosHead - screenPos);
				if (distance < mScriptAiming.getOriginHeight()/2)
				{
					distance = mScriptAiming.getOriginHeight() / 2;
				}
				mScriptAiming.setAiming((screenPos + screenPosHead) / 2.0f, distance, true);
				
			}
			else
			{
				mAimTarget = aimCharacterList[aim];
				launchMissile();
			}
		}
	}
	public override void keyProcess(float elapsedTime)
	{
		if (mGameInputManager.getKeyCurrentDown(KeyCode.A))
		{
			launchMissile();
		}
	}
	public override void leave()
	{
		aimCharacterList.Clear();
		aimCount = 0;
		// 隐藏瞄准图标
		if (mPlayer.isType(CHARACTER_TYPE.CT_MYSELF))
		{
			LayoutTools.HIDE_LAYOUT(LAYOUT_TYPE.LT_AIMING);
		}
	}
	protected void launchMissile()
	{
		// 发射导弹
		if (mAimTarget != null)
		{
			MissileParam param = new MissileParam();
			param.mPosition = mPlayer.getPosition() + new Vector3(0.0f, 2.5f, 0.0f);
			param.mTarget = mAimTarget;
			mItemManager.createItem(SCENE_ITEM.SI_MISSILE, param);
			//移除角色背包中的导弹道具
			CommandCharacterRemoveItem cmdRemove = newCmd(out cmdRemove);
			cmdRemove.mItem = mPlayer.getPlayerPack().getCurItem();
			pushCommand(cmdRemove, mPlayer);
		}
		// 移除瞄准状态
		CommandCharacterRemoveState cmdState = newCmd(out cmdState);
		cmdState.mState = mType;
		pushCommand(cmdState, mPlayer);
	}
}