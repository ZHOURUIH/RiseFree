using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class StartSceneExit : SceneProcedure
{
	public StartSceneExit()
	{ }
	public StartSceneExit(PROCEDURE_TYPE type, GameScene gameScene)
		:
		base(type, gameScene)
	{
		;
	}
	protected override void onInit(SceneProcedure lastProcedure, string intent)
	{
		;
	}
	protected override void onUpdate(float elapsedTime)
	{
		;
	}
	protected override void onExit(SceneProcedure nextProcedure)
	{
		LayoutTools.UNLOAD_LAYOUT(LAYOUT_TYPE.LT_STAND_BY);
		LayoutTools.UNLOAD_LAYOUT(LAYOUT_TYPE.LT_BUTTOM_PROMPT);
		LayoutTools.UNLOAD_LAYOUT(LAYOUT_TYPE.LT_RETURN);
		LayoutTools.UNLOAD_LAYOUT(LAYOUT_TYPE.LT_SELECT_ROLE);
		LayoutTools.UNLOAD_LAYOUT(LAYOUT_TYPE.LT_VOLUME_SETTING);
		mSceneSystem.unloadScene(GameDefine.ROLE_DISPLAY);
		// 清空所有角色的所有状态
		mRoleSystem.clearAllRoleState();
		// 隐藏所有选择用的角色
		mRoleSystem.hideAllSelectRole();
	}
	protected override void onKeyProcess(float elapsedTime)
	{
		;
	}
}