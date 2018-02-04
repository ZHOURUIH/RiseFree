using UnityEngine;
using System.Collections;

public class CommandCharacterNotifyFinish : Command
{
	public bool mFinish;
	public override void init()
	{
		base.init();
		mFinish = false;
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
	}
	public override string showDebugInfo()
	{
		return this.GetType().ToString() + " : finish : " + mFinish;
	}
}