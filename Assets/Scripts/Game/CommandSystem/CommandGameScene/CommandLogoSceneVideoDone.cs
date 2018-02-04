using UnityEngine;
using System.Collections;

public class CommandLogoSceneVideoDone : Command
{
	public override void init()
	{
		base.init();
	}
	public override void execute()
	{
		LogoScene gameScene = (mReceiver) as LogoScene;
		if(gameScene.atProcedure(PROCEDURE_TYPE.PT_LOGO_START_VIDEO))
		{
			LogoSceneStartVideo startVideo = gameScene.getCurOrParentProcedure<LogoSceneStartVideo>(PROCEDURE_TYPE.PT_LOGO_START_VIDEO);
			startVideo.notifyStartVideoDone();
		}
	}
	public override string showDebugInfo()
	{
		return this.GetType().ToString();
	}
}
