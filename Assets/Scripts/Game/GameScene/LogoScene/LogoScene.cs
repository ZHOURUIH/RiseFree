using UnityEngine;
using System.Collections;

public class LogoScene : GameScene
{
	public LogoScene(GAME_SCENE_TYPE type, string name)
		:
		base(type, name)
	{
		mDestroyEngineScene = false;
	}
	public override void assignStartExitProcedure()
	{
		mStartProcedure = PROCEDURE_TYPE.PT_LOGO_REGISTER_CHECK;
		mExitProcedure = PROCEDURE_TYPE.PT_LOGO_EXIT;
	}
	public override void createSceneProcedure()
	{
		addProcedure<LogoSceneRegisterCheck>(PROCEDURE_TYPE.PT_LOGO_REGISTER_CHECK);
		addProcedure<LogoSceneLogo>(PROCEDURE_TYPE.PT_LOGO_LOGO);
		addProcedure<LogoSceneStartVideo>(PROCEDURE_TYPE.PT_LOGO_START_VIDEO);
		addProcedure<LogoSceneExit>(PROCEDURE_TYPE.PT_LOGO_EXIT);
		if (mSceneProcedureList.Count != (int)PROCEDURE_TYPE.PT_LOGO_MAX - (int)PROCEDURE_TYPE.PT_LOGO_MIN - 1)
		{
			UnityUtility.logError("error : not all procedure added! : " + typeof(LogoScene).ToString());
		}
	}
}
