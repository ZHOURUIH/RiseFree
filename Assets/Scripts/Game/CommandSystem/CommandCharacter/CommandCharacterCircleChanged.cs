using UnityEngine;
using System.Collections;

public class CommandCharacterCircleChanged : Command
{
	public int mCircle;
	public override void init()
	{
		base.init();
		mCircle = 0;
	}
	public override void execute()
	{
		Character character = mReceiver as Character;
		CharacterData data = character.getCharacterData();
		data.mCircle = mCircle;
		// 如果已经达到赛道的最大圈数,则完成比赛
		if (data.mCircle >= mRaceSystem.getCurGameTrack().mCircleCount)
		{
			// 通知玩家是否完成比赛
			CommandCharacterNotifyFinish cmdFinish = newCmd(out cmdFinish);
			cmdFinish.mFinish = true;
			pushCommand(cmdFinish, character);
			// 只要有玩家完成了比赛,并且不在比赛结束倒计时流程,则跳转到比赛结束的倒计时流程
			GameScene gameScene = mGameSceneManager.getCurScene();
			if (!gameScene.atProcedure(PROCEDURE_TYPE.PT_MAIN_GAMING_FINISH))
			{
				CommandGameSceneChangeProcedure cmdProcedure = newCmd(out cmdProcedure);
				cmdProcedure.mProcedure = PROCEDURE_TYPE.PT_MAIN_GAMING_FINISH;
				pushCommand(cmdProcedure, gameScene);
			}
		}
		else
		{
			// 提示圈数
			if(character.isType(CHARACTER_TYPE.CT_MYSELF))
			{
				mScriptCircleTip.notifyFinishedCircle(mCircle);
			}
		}
	}
	public override string showDebugInfo()
	{
		return this.GetType().ToString() + " : circle : " + mCircle;
	}
}