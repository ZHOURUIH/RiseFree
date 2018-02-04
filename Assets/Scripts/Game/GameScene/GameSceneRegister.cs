using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameSceneRegister : GameBase
{
	public void registerAllGameScene()
	{
		registeGameScene<LogoScene>(GAME_SCENE_TYPE.GST_LOGO);
		registeGameScene<StartScene>(GAME_SCENE_TYPE.GST_START);
		registeGameScene<MainScene>(GAME_SCENE_TYPE.GST_MAIN);
	}
	//-------------------------------------------------------------------------------------------------------------
	protected void registeGameScene<T>(GAME_SCENE_TYPE type) where T : GameScene
	{
		mGameSceneManager.registeGameScene<T>(type);
	}
}