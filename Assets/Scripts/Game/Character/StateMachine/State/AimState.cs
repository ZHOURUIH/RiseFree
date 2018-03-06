using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 瞄准状态
public class AimState : PlayerState
{
	protected CharacterOther mAimTarget;
	protected List<CharacterOther> mAimCharacterList;
	public AimState(PLAYER_STATE type)
		:
		base(type)
	{
		mAimCharacterList = new List<CharacterOther>();
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
	public override void enter(StateParam param)
	{
		// 显示瞄准图标,只有玩家自己进入瞄准状态才显示
		if (mPlayer.isType(CHARACTER_TYPE.CT_MYSELF))
		{
			LayoutTools.LOAD_NGUI_SHOW(LAYOUT_TYPE.LT_AIMING, 20);
		}
	}
	public override void update(float elapsedTime)
	{
		base.update(elapsedTime);
		mAimCharacterList.Clear();
		SortedDictionary<int, CharacterOther> allCharacterList = mRoleSystem.getAllPlayer();
		float playerDistance = mPlayer.getCharacterData().mRunDistance;
		foreach (var item in allCharacterList)
		{
			if (item.Value != mPlayer)
			{
				float curDistance = item.Value.getCharacterData().mRunDistance - playerDistance;
				if (MathUtility.isInRange(curDistance, 0.0f, GameDefine.MAX_LAUNCH_MISSILE_DISTANCE))
				{
					if (UnityUtility.whetherGameObjectInScreen(item.Value.getWorldPosition()))
					{
						mAimCharacterList.Add(item.Value);
					}
				}
			}
		}
		if (mAimCharacterList.Count == 0)
		{
			mAimTarget = null;
		}
		else
		{
			if(mAimTarget == null || !mAimCharacterList.Contains(mAimTarget))
			{
				int aim = MathUtility.randomInt(0, mAimCharacterList.Count - 1);
				mAimTarget = mAimCharacterList[aim];
			}
		}
		CommandCharacterAimTarget cmdAim = newCmd(out cmdAim, false);
		cmdAim.mTarget = mAimTarget;
		pushCommand(cmdAim, mPlayer);
		if (mAimTarget != null && mPlayer.isType(CHARACTER_TYPE.CT_AI))
		{
			launchMissile();
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
		mAimTarget = null;
		// 隐藏瞄准图标
		if (mPlayer.isType(CHARACTER_TYPE.CT_MYSELF))
		{
			LayoutTools.HIDE_LAYOUT(LAYOUT_TYPE.LT_AIMING);
		}
	}
	protected void launchMissile()
	{
		// 发射导弹
		bool success = false;
		if (mAimTarget != null)
		{
			MissileParam param = new MissileParam();
			param.mPosition = mPlayer.getPosition() + new Vector3(0.0f, 2.5f, 0.0f);
			param.mTarget = mAimTarget;
			SceneMissile missile = mItemManager.createItem<SceneMissile>(SCENE_ITEM.SI_MISSILE, param);
			if (mPlayer.isType(CHARACTER_TYPE.CT_MYSELF))
			{
				GameTools.PLAY_AUDIO_OBJECT(missile, SOUND_DEFINE.SD_FIRE_MISSILE);
			}
			// 通知锁定玩家
			CommandCharacterNotifyMissileLocked cmdLock = newCmd(out cmdLock);
			cmdLock.mLocked = true;
			cmdLock.mMissile = missile;
			pushCommand(cmdLock, mAimTarget);
			success = true;
		}
		// 移除瞄准状态
		CommandCharacterRemoveState cmdState = newCmd(out cmdState);
		cmdState.mState = mType;
		pushCommand(cmdState, mPlayer);
		// 发射成功后移除角色背包中的导弹道具
		if(success)
		{
			CommandCharacterRemoveItem cmdRemove = newCmd(out cmdRemove);
			cmdRemove.mItem = mPlayer.getPlayerPack().getCurItem();
			pushCommand(cmdRemove, mPlayer);
		}
	}
}