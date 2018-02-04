using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class LogoSceneExit : SceneProcedure
{
	public LogoSceneExit()
	{ }
	public LogoSceneExit(PROCEDURE_TYPE type, GameScene gameScene)
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
		LayoutTools.UNLOAD_LAYOUT(LAYOUT_TYPE.LT_LOGO);
		LayoutTools.UNLOAD_LAYOUT(LAYOUT_TYPE.LT_START_VIDEO);
	}
	protected override void onKeyProcess(float elapsedTime)
	{
		;
	}
}