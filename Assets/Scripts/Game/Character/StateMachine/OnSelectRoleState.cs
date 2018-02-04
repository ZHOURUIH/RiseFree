using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class OnSelectRoleState : SelecteState
{
	public OnSelectRoleState(PLAYER_STATE type)
		:base(type)
	{
		;
	}
	public override void enter()
	{
		mCharacterManager.activeCharacter(mPlayer, true);
		RoleDisplay scene = mSceneSystem.getScene<RoleDisplay>(GameDefine.ROLE_DISPLAY);
		ObjectTools.MOVE_OBJECT_EX(mPlayer, scene.mRolePosition1, scene.mRolePosition0, 1.0f, onMoveDone);
		mPlayer.resetRotation();
		// 设置静止状态拖尾,播放骑行动作
		mPlayer.activeTrail(true);
		UnityUtility.playAnimation(mPlayer.getAnimation(), GameDefine.ANIM_RIDE, true, "", false);
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
}
