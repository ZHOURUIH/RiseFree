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
		int[] startIndex = new int[GameDefine.MAX_AI_COUNT] { 0, 1, 3, 4};
		for(int i = 0; i < GameDefine.MAX_AI_COUNT; ++i)
		{
			mRoleSystem.createAI("player" + i, GameDefine.ROLE_MODEL_NAME[i], startIndex[i], i);
		}
		// 给所有角色添加准备状态
		mRoleSystem.notifyAllPlayerReady();
		// 显示游戏界面
		LayoutTools.LOAD_NGUI_HIDE(LAYOUT_TYPE.LT_DIRECTION_TIPS, 20);
		LayoutTools.LOAD_NGUI_SHOW(LAYOUT_TYPE.LT_BUTTOM_PROMPT, 0);
		LayoutTools.LOAD_NGUI_SHOW(LAYOUT_TYPE.LT_TOP_TIME, 0);
		LayoutTools.LOAD_NGUI_SHOW(LAYOUT_TYPE.LT_TRACK, 0);
		LayoutTools.LOAD_NGUI_SHOW(LAYOUT_TYPE.LT_PROPS, 0);
		LayoutTools.LOAD_NGUI_SHOW(LAYOUT_TYPE.LT_COUNT_DOWN, 5);
		LayoutTools.LOAD_UGUI_SHOW(LAYOUT_TYPE.LT_PLAYER_RACE_INFO, 10);
		LayoutTools.LOAD_NGUI_SHOW(LAYOUT_TYPE.LT_ATTACK_TIP, 10);
		LayoutTools.HIDE_LAYOUT_DELAY(null, 10.0f, LAYOUT_TYPE.LT_BUTTOM_PROMPT);
		LayoutTools.LOAD_NGUI_HIDE(LAYOUT_TYPE.LT_DEBUG_INFO, 25);
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