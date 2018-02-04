using UnityEngine;
using System.Collections;

public class StartScene : GameScene
{
	public StartScene(GAME_SCENE_TYPE type, string name)
		:
		base(type, name)
	{
		mDestroyEngineScene = false;
	}
	public override void assignStartExitProcedure()
	{
		mStartProcedure = PROCEDURE_TYPE.PT_START_LOADING;
		mExitProcedure = PROCEDURE_TYPE.PT_START_EXIT;
	}
	public override void createSceneProcedure()
	{
		addProcedure<StartSceneLoading>(PROCEDURE_TYPE.PT_START_LOADING);
		addProcedure<StartSceneStandBy>(PROCEDURE_TYPE.PT_START_STAND_BY);
		addProcedure<StartSceneSelectRole>(PROCEDURE_TYPE.PT_START_SELECT_ROLE);
		addProcedure<StartSceneSetting>(PROCEDURE_TYPE.PT_START_SETTING);
		addProcedure<StartSceneSelectTrack>(PROCEDURE_TYPE.PT_START_SELECT_TRACK);
		addProcedure<StartSceneConfirmSelection>(PROCEDURE_TYPE.PT_START_CONFIRM_SELECTION);
		addProcedure<StartSceneExit>(PROCEDURE_TYPE.PT_START_EXIT);
		if (mSceneProcedureList.Count != (int)PROCEDURE_TYPE.PT_START_MAX - (int)PROCEDURE_TYPE.PT_START_MIN - 1)
		{
			UnityUtility.logError("error : not all procedure added! : " + typeof(StartScene).ToString());
		}
	}
}
