using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Net;

public class Game : GameFramework
{
	public override void notifyBase()
	{
		base.notifyBase();
		// 所有类都构造完成后通知GameBase
		GameBase frameBase = new GameBase();
		frameBase.notifyConstructDone();
	}
	public override void registe()
	{
		LayoutRegister layoutRegister = new LayoutRegister();
		layoutRegister.registeAllLayout();
		GameSceneRegister gameSceneRegister = new GameSceneRegister();
		gameSceneRegister.registerAllGameScene();
		DataRegister dataRegister = new DataRegister();
		dataRegister.registeAllData();
		CharacterRegister characterRegiste = new CharacterRegister();
		characterRegiste.registeAllCharacter();
		SceneRegister sceneRegister = new SceneRegister();
		sceneRegister.registeAllScene();
	}
	public override void initComponent()
	{
		base.initComponent();
		registeComponent<SocketManager>();
		registeComponent<SerialPortManager>();
		registeComponent<HardwareInfo>();
		registeComponent<GameConfig>();
		registeComponent<GameUtility>();
		registeComponent<GameInputManager>();
		registeComponent<GameSetting>();
		registeComponent<RoleSystem>();
		registeComponent<RaceSystem>();
		registeComponent<SceneItemManager>();
		registeComponent<LogSystem>();
		registeComponent<RegisterTool>();
		registeComponent<WayPointManager>();
	}
	public override void init()
	{
		base.init();
	}
	public override void launch()
	{
		base.launch();
		CommandGameSceneManagerEnter cmd = GameBase.newCmd(out cmd, false, false);
		cmd.mSceneType = GAME_SCENE_TYPE.GST_LOGO;
		GameBase.pushCommand(cmd, GameBase.mGameSceneManager);
	}
	public override void update(float elapsedTime)
	{
		base.update(elapsedTime);
	}
	public override void fixedUpdate(float elapsedTime)
	{
		base.fixedUpdate(elapsedTime);
	}
}