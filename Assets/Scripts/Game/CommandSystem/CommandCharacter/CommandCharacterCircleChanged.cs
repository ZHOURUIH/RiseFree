﻿using UnityEngine;
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
			// 通知玩家完成比赛
			pushCommand<CommandCharacterNotifyFinish>(character);
			// 只要有玩家完成了比赛,并且不在比赛结束倒计时流程,则跳转到比赛结束的倒计时流程
			GameScene gameScene = mGameSceneManager.getCurScene();
			if (!gameScene.atProcedure(PROCEDURE_TYPE.PT_MAIN_GAMING_FINISH))
			{
				CommandGameSceneChangeProcedure cmdProcedure = newCmd(out cmdProcedure);
				cmdProcedure.mProcedure = PROCEDURE_TYPE.PT_MAIN_GAMING_FINISH;
				pushCommand(cmdProcedure, gameScene);
			}
			// 完成比赛 在倒计时流程 如果是玩家 那么就隐藏倒计时布局
			if (gameScene.atProcedure(PROCEDURE_TYPE.PT_MAIN_GAMING_FINISH))
			{
				if (character.isType(CHARACTER_TYPE.CT_MYSELF))
				{
					LayoutTools.HIDE_LAYOUT(LAYOUT_TYPE.LT_END_COUNT_DOWN, true);
				}
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
		// 通知布局
		mScriptPlayerRaceInfo.notifyCurCircle(data.mNumber, data.mCircle);
	}
	public override string showDebugInfo()
	{
		return this.GetType().ToString() + " : circle : " + mCircle;
	}
}