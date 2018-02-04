using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 游戏主流程
public class MainSceneReady : SceneProcedure
{
	public MainSceneReady()
	{ }
	public MainSceneReady(PROCEDURE_TYPE type, GameScene gameScene)
		:
		 base(type, gameScene)
	{
		;
	}
	protected override void onInit(SceneProcedure lastProcedure, string intent)
	{
		// 创建比赛的所有角色
		mRoleSystem.createMyself("myself", 2);
		mRoleSystem.createAI("player0", "Role0", 0, 0);
		mRoleSystem.createAI("player1", "Role1", 1, 1);
		mRoleSystem.createAI("player2", "Role2", 3, 2);
		mRoleSystem.createAI("player3", "Role3", 4, 3);
		// 给所有角色添加准备状态
		mRoleSystem.notifyAllPlayerReady();
		// 显示游戏界面
		LayoutTools.LOAD_LAYOUT(LAYOUT_TYPE.LT_DIRECTION_TIPS, 20,false ,false ,"");
		LayoutTools.LOAD_LAYOUT_SHOW(LAYOUT_TYPE.LT_BUTTOM_PROMPT, 0);
		LayoutTools.LOAD_LAYOUT_SHOW(LAYOUT_TYPE.LT_TOP_TIME, 0);
		LayoutTools.LOAD_LAYOUT_SHOW(LAYOUT_TYPE.LT_TRACK, 0);
		LayoutTools.LOAD_LAYOUT_SHOW(LAYOUT_TYPE.LT_PROPS, 0);
		LayoutTools.LOAD_LAYOUT_SHOW(LAYOUT_TYPE.LT_COUNT_DOWN, 5);
		LayoutTools.HIDE_LAYOUT_DELAY(null, 10.0f, LAYOUT_TYPE.LT_BUTTOM_PROMPT);
	}
	protected override void onUpdate(float elapsedTime)
	{
		;
	}
	protected override void onExit(SceneProcedure nextProcedure)
	{
		LayoutTools.HIDE_LAYOUT(LAYOUT_TYPE.LT_COUNT_DOWN);
	}
	protected override void onKeyProcess(float elapsedTime)
	{
		;
	}
}