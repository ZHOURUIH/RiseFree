using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MainScene : GameScene
{
	public MainScene(GAME_SCENE_TYPE type, string name)
		:
		base(type, name)
	{ }
	public override void assignStartExitProcedure()
	{
		mStartProcedure = PROCEDURE_TYPE.PT_MAIN_LOADING;
		mExitProcedure = PROCEDURE_TYPE.PT_MAIN_EXIT;
	}
	public override void createSceneProcedure()
	{
		addProcedure<MainSceneLoading>(PROCEDURE_TYPE.PT_MAIN_LOADING);
		addProcedure<MainSceneReady>(PROCEDURE_TYPE.PT_MAIN_READY);
		MainSceneGaming gaming = addProcedure<MainSceneGaming>(PROCEDURE_TYPE.PT_MAIN_GAMING);
		addProcedure<MainSceneGamingFinish>(PROCEDURE_TYPE.PT_MAIN_GAMING_FINISH, gaming);
		addProcedure<MainSceneFinish>(PROCEDURE_TYPE.PT_MAIN_FINISH);
		addProcedure<MainSceneSettlement>(PROCEDURE_TYPE.PT_MAIN_SETTLEMENT);
		addProcedure<MainSceneExit>(PROCEDURE_TYPE.PT_MAIN_EXIT);
		if (mSceneProcedureList.Count != (int)PROCEDURE_TYPE.PT_MAIN_MAX - (int)PROCEDURE_TYPE.PT_MAIN_MIN - 1)
		{
			UnityUtility.logError("error : not all procedure added! : " + typeof(MainScene).ToString());
		}
	}
	public override void update(float elapsedTime)
	{ 
		base.update(elapsedTime); 
	}
	protected void activeLayout(LAYOUT_TYPE layout)
	{
		LayoutScriptAutoHide script = mLayoutManager.getScript<LayoutScriptAutoHide>(layout);
		if (!script.getLayout().isVisible() && script.getHideDone())
		{
			LayoutTools.SHOW_LAYOUT(layout);
		}
		else if (script.getShowDone())
		{
			script.resetHideTime();
		}
	}
}