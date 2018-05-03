using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Net;

public class Game : GameFramework
{
	public override void update(float elapsedTime)
	{
		base.update(elapsedTime);
	}
	public override void fixedUpdate(float elapsedTime)
	{
		base.fixedUpdate(elapsedTime);
	}
	public override void keyProcess()
	{
		base.keyProcess();
		if(FrameBase.mInputManager.getKeyCurrentDown(KeyCode.L))
		{
			GameLayout debugInfo = FrameBase.mLayoutManager.getGameLayout(LAYOUT_TYPE.LT_DEBUG_INFO);
			if (debugInfo != null)
			{
				if (debugInfo.isVisible())
				{
					LayoutTools.HIDE_LAYOUT(LAYOUT_TYPE.LT_DEBUG_INFO);
				}
				else
				{
					LayoutTools.SHOW_LAYOUT(LAYOUT_TYPE.LT_DEBUG_INFO);
				}
			}
		}
	}
	//------------------------------------------------------------------------------------------------------------------------------
	protected override void notifyBase()
	{
		base.notifyBase();
		// 所有类都构造完成后通知GameBase
		GameBase frameBase = new GameBase();
		frameBase.notifyConstructDone();
	}
	protected override void registe()
	{
		LayoutRegister.registeAllLayout();
		GameSceneRegister.registerAllGameScene();
		DataRegister.registeAllData();
		CharacterRegister.registeAllCharacter();
		SceneRegister.registeAllScene();
	}
	protected override void initComponent()
	{
		base.initComponent();
		registeComponent<SocketManager>();
		registeComponent<USBManager>();
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
	protected override void init()
	{
		base.init();
	}
	protected override void launch()
	{
		base.launch();
		CommandGameSceneManagerEnter cmd = GameBase.newCmd(out cmd, false, false);
		cmd.mSceneType = GAME_SCENE_TYPE.GST_LOGO;
		GameBase.pushCommand(cmd, GameBase.mGameSceneManager);
	}
}