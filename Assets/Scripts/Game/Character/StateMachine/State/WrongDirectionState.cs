using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 逆行状态
public class WrongDirectionState : PlayerState
{
	protected float mWrongDirectionTime;
	public WrongDirectionState(PLAYER_STATE type)
		:
		base(type)
	{
		mWrongDirectionTime = 0.0f;
	}
	public override void enter(StateParam param)
	{
		// 显示方向提示界面,只有玩家自己进入瞄准状态才显示
		if (mPlayer.isType(CHARACTER_TYPE.CT_MYSELF))
		{
			LayoutTools.LOAD_NGUI_SHOW(LAYOUT_TYPE.LT_DIRECTION_TIPS, 20);
		}
	}
	public override void update(float elapsedTime)
	{
		base.update(elapsedTime);
		mWrongDirectionTime += elapsedTime;
		if(mWrongDirectionTime >=GameDefine.WRONG_DIRECTION_TIME)
		{
			// 重置角色方向
			pushCommand<CommandCharacterReset>(mPlayer);
			// 退出当前状态
			CommandCharacterRemoveState cmdState = newCmd(out cmdState);
			cmdState.mState = mType;
			pushCommand(cmdState, mPlayer);
		}
	}
	public override void leave()
	{
		// 隐藏方向提示界面
		if (mPlayer.isType(CHARACTER_TYPE.CT_MYSELF))
		{
			LayoutTools.HIDE_LAYOUT(LAYOUT_TYPE.LT_DIRECTION_TIPS);
		}
	}
}