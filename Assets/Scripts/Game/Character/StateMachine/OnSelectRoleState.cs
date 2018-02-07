using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class AnimationOdds
{
	public string mAnimName;
	public string mNextAnim;
	public float mOdds;
	public AnimationOdds(string name, string nextAnim, float odds)
	{
		mAnimName = name;
		mNextAnim = nextAnim;
		mOdds = odds;
	}
}

class OnSelectRoleState : SelecteState
{
	protected List<AnimationOdds> mAnimationList;
	public OnSelectRoleState(PLAYER_STATE type)
		:base(type)
	{
		mAnimationList = new List<AnimationOdds>();
		mAnimationList.Add(new AnimationOdds(GameDefine.ANIM_RIDE, GameDefine.ANIM_RIDE, 0.7f));
		mAnimationList.Add(new AnimationOdds(GameDefine.ANIM_SHAKE_BIKE, GameDefine.ANIM_SHAKE_BIKE, 0.1f));
		mAnimationList.Add(new AnimationOdds(GameDefine.ANIM_SPEED_UP, GameDefine.ANIM_RIDE, 0.1f));
		mAnimationList.Add(new AnimationOdds(GameDefine.ANIM_SPEED_UP_SHARP, GameDefine.ANIM_RIDE, 0.1f));
	}
	public override void enter()
	{
		mCharacterManager.activeCharacter(mPlayer, true);
		RoleDisplay scene = mSceneSystem.getScene<RoleDisplay>(GameDefine.ROLE_DISPLAY);
		ObjectTools.MOVE_OBJECT_EX(mPlayer, scene.mRolePosition1, scene.mRolePosition0, 1.0f, onMoveDone);
		mPlayer.resetRotation();
		// 设置静止状态拖尾,播放骑行动作
		mPlayer.activeTrail(true);
		AnimationOdds anim = generateAnimation();
		mAnimation.Play(anim.mAnimName);
		mAnimation[anim.mAnimName].speed = 1.5f;
		mAnimation.PlayQueued(anim.mNextAnim);
		mAnimation[anim.mNextAnim + GameDefine.QUEUE_SUFFIX].speed = 1.5f;
		mPlayer.getSelectAnimation().Clear();
		mPlayer.getSelectAnimation().Add(anim.mAnimName);
		mPlayer.getSelectAnimation().Add(anim.mNextAnim);
	}
	//-----------------------------------------------------------------------------------------------------------------------
	protected void onMoveDone(ComponentKeyFrameBase component, object userData, bool breakTremling, bool done)
	{
		if (breakTremling)
		{
			return;
		}
		CommandCharacterAddState cmdCharacterAddState = newCmd(out cmdCharacterAddState);
		cmdCharacterAddState.mState = PLAYER_STATE.PS_SELECTED_ROLE;
		pushCommand(cmdCharacterAddState, mPlayer);
	}
	protected AnimationOdds generateAnimation()
	{
		float rand = MathUtility.randomFloat(0.01f, 1.0f);
		int count = mAnimationList.Count;
		float curMin = 0.0f;
		AnimationOdds animation = null;
		for (int i = 0; i < count; ++i)
		{
			if(rand > curMin && rand <= curMin + mAnimationList[i].mOdds)
			{
				animation = mAnimationList[i];
				break;
			}
			curMin += mAnimationList[i].mOdds;
		}
		return animation;
	}
}
