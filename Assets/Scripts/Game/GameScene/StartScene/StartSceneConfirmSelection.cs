using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class StartSceneConfirmSelection : SceneProcedure
{
	public StartSceneConfirmSelection()
	{ }
	public StartSceneConfirmSelection(PROCEDURE_TYPE type, GameScene gameScene)
		:
	base(type, gameScene)
	{
		;
	}
	protected override void onInit(SceneProcedure lastProcedure, string intent)
	{
		LayoutTools.LOAD_LAYOUT_SHOW(LAYOUT_TYPE.LT_CONFIRM_SELECTION,10);
	}
	protected override void onUpdate(float elapsedTime)
	{
		;
	}
	protected override void onExit(SceneProcedure nextProcedure)
	{
		LayoutTools.HIDE_LAYOUT(LAYOUT_TYPE.LT_CONFIRM_SELECTION);
	}
	protected override void onKeyProcess(float elapsedTime)
	{
		if (mGameInputManager.getKeyCurrentDown(KeyCode.A))
		{
			LayoutTools.UNLOAD_LAYOUT(LAYOUT_TYPE.LT_BUTTOM_PROMPT);
			CommandGameSceneManagerEnter cmd = newCmd(out cmd);
			cmd.mSceneType = GAME_SCENE_TYPE.GST_MAIN;
			pushCommand(cmd, mGameSceneManager);
			return;
		}
		if (mGameInputManager.getKeyCurrentDown(KeyCode.Y))
		{
			CommandGameSceneChangeProcedure cmd = newCmd(out cmd);
			cmd.mProcedure = PROCEDURE_TYPE.PT_START_SELECT_TRACK;
			pushCommand(cmd, mGameScene);
			return;
		}
	}
}