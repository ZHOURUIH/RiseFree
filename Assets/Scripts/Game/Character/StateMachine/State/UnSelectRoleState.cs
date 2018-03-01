using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class UnSelectRoleState : SelecteState
{
	public UnSelectRoleState(PLAYER_STATE type)
		:base(type)
	{
		;
	}
	public override void enter()
	{
		mAnimation.CrossFade(GameDefine.ANIM_TURN_RIGHT);
		mAnimation.CrossFadeQueued(GameDefine.ANIM_RIDE);
		RoleDisplay scene = mSceneSystem.getScene<RoleDisplay>(GameDefine.ROLE_DISPLAY);
		ObjectTools.MOVE_OBJECT_EX(mPlayer, scene.mRolePosition0, scene.mRolePosition2, 1.0f, onMoveDone);
	}
	public void onMoveDone(ComponentKeyFrameBase component, object userData, bool breakTremling, bool done)
	{
		if (breakTremling)
		{
			return;
		}
		mCharacterManager.activeCharacter(mPlayer, false);
	}
	public override void leave()
	{
		// 恢复动作的播放速度
		List<string> animList = mPlayer.getSelectAnimation();
		int count = animList.Count;
		for(int i = 0; i < count; ++i)
		{
			mAnimation[animList[i]].speed = 1.0f;
		}
		animList.Clear();
	}
}