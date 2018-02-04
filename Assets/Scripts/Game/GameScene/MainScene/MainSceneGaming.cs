using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 游戏主流程
public class MainSceneGaming : SceneProcedure
{
	public MainSceneGaming()
	{ }
	public MainSceneGaming(PROCEDURE_TYPE type, GameScene gameScene)
		:
		 base(type, gameScene)
	{

	}
	protected override void onInit(SceneProcedure lastProcedure, string intent)
	{
		// 给所有角色添加游戏中状态
		mRoleSystem.notifyAllPlayerGame();
		mRaceSystem.notifyGameStart();
		LayoutTools.LOAD_LAYOUT_SHOW(LAYOUT_TYPE.LT_CIRCLE_TIP, 10);
		LayoutTools.LOAD_LAYOUT_HIDE(LAYOUT_TYPE.LT_END_COUNT_DOWN, 10);
	}
	protected override void onUpdate(float elapsedTime)
	{
		;
	}
	protected override void onExit(SceneProcedure nextProcedure)
	{
		LayoutTools.HIDE_LAYOUT(LAYOUT_TYPE.LT_BUTTOM_PROMPT);
	}
	protected override void onKeyProcess(float elapsedTime)
	{
		// 测试结束本局游戏
		if(mGameInputManager.getKeyCurrentDown(KeyCode.E))
		{
			CommandGameSceneChangeProcedure cmdProcedure = newCmd(out cmdProcedure);
			cmdProcedure.mProcedure = PROCEDURE_TYPE.PT_MAIN_FINISH;
			pushCommand(cmdProcedure, mGameScene);
		}
	}
}