using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 比赛流程中的比赛结束时倒计时流程
public class MainSceneGamingFinish : SceneProcedure
{
	public MainSceneGamingFinish()
	{ }
	public MainSceneGamingFinish(PROCEDURE_TYPE type, GameScene gameScene)
		:
		 base(type, gameScene)
	{

	}
	protected override void onInit(SceneProcedure lastProcedure, string intent)
	{
		// 显示比赛结束倒计时
		LayoutTools.SHOW_LAYOUT(LAYOUT_TYPE.LT_END_COUNT_DOWN);
		// 10秒后跳转到结束流程
		CommandGameScenePrepareChangeProcedure cmdProcedure = newCmd(out cmdProcedure);
		cmdProcedure.mProcedure = PROCEDURE_TYPE.PT_MAIN_FINISH;
		cmdProcedure.mPrepareTime = 10.0f;
		pushCommand(cmdProcedure, mGameScene);
	}
	protected override void onUpdate(float elapsedTime)
	{
		;
	}
	protected override void onExit(SceneProcedure nextProcedure)
	{
		// 隐藏比赛结束倒计时
		LayoutTools.HIDE_LAYOUT(LAYOUT_TYPE.LT_END_COUNT_DOWN);
	}
	protected override void onKeyProcess(float elapsedTime)
	{
		;
	}
}