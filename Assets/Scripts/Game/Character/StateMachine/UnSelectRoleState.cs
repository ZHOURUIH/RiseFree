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
		UnityUtility.playAnimation(mPlayer.getAnimation(), GameDefine.ANIM_TURN_RIGHT, false, GameDefine.ANIM_RIDE, true);
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
}