using UnityEngine;
using System.Collections;

// 正常骑行状态
public class IdleState : PlayerState
{
	public IdleState(PLAYER_STATE type)
		:
		base(type)
	{
		mEnableStatus.mProcessTurn = false;
	}
	public override void enter(StateParam param)
	{
		;
	}
	public override void leave()
	{
		;
	}
}