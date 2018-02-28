using UnityEngine;
using System.Collections;

public class CommandCharacterNotifyFinish : Command
{
	public bool mFinish;
	public override void init()
	{
		base.init();
		mFinish = true;
	}
	public override void execute()
	{
		Character character = mReceiver as Character;
		// 无论是否完成比赛,都需要添加完成比赛的状态
		CommandCharacterAddState cmdState = newCmd(out cmdState);
		cmdState.mState = PLAYER_STATE.PS_FINISH;
		pushCommand(cmdState, character);
		// 如果是玩家自己,则显示是否比赛完成
		if (character.isType(CHARACTER_TYPE.CT_MYSELF))
		{
			mScriptCircleTip.notifyFinishRace(mFinish);
		}
		// 移除瞄准状态
		if ((character as CharacterOther).getStateMachine().hasState(PLAYER_STATE.PS_AIM))
		{
			CommandCharacterRemoveState cmdRemoveState = newCmd(out cmdRemoveState);
			cmdRemoveState.mState = PLAYER_STATE.PS_AIM;
			pushCommand(cmdRemoveState, character as CharacterOther);
		}
	}
	public override string showDebugInfo()
	{
		return this.GetType().ToString() + " : finish : " + mFinish;
	}
}