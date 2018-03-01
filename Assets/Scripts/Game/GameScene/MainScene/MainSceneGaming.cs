using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 游戏主流程
public class MainSceneGaming : SceneProcedure
{
	protected SOUND_DEFINE[] mRaceMusicList;
	protected int mCount = 0;
	public MainSceneGaming()
	{ }
	public MainSceneGaming(PROCEDURE_TYPE type, GameScene gameScene)
		:
		 base(type, gameScene)
	{
		mRaceMusicList = new SOUND_DEFINE[] 
		{
			SOUND_DEFINE.SD_RACE_BACKGROUND0,
			SOUND_DEFINE.SD_RACE_BACKGROUND1,
			SOUND_DEFINE.SD_RACE_BACKGROUND2,
			SOUND_DEFINE.SD_RACE_BACKGROUND3,
		};
	}
	protected override void onInit(SceneProcedure lastProcedure, string intent)
	{
		// 给所有角色添加游戏中状态
		mRoleSystem.notifyAllPlayerGame();
		mRaceSystem.notifyGameStart();
		LayoutTools.LOAD_NGUI_SHOW(LAYOUT_TYPE.LT_CIRCLE_TIP, 10);
		LayoutTools.LOAD_NGUI_HIDE(LAYOUT_TYPE.LT_END_COUNT_DOWN, 10);
		// 随机播放一首背景音乐
		GameTools.PLAY_AUDIO_SCENE(mRaceMusicList[MathUtility.randomInt(0, mRaceMusicList.Length - 1)], true);
	}
	protected override void onUpdate(float elapsedTime)
	{
		;
	}
	protected override void onExit(SceneProcedure nextProcedure)
	{
		LayoutTools.HIDE_LAYOUT(LAYOUT_TYPE.LT_BUTTOM_PROMPT);
	}
	protected override void onKeyProcess(float elapsedTime)
	{
		// 测试结束本局游戏
		if(mGameInputManager.getKeyCurrentDown(KeyCode.E))
		{
			CommandGameSceneChangeProcedure cmdProcedure = newCmd(out cmdProcedure);
			cmdProcedure.mProcedure = PROCEDURE_TYPE.PT_MAIN_FINISH;
			pushCommand(cmdProcedure, mGameScene);
		}
		//按L键显示和隐藏游戏键位的使用说明
		if (Input.GetKeyDown(KeyCode.L))
		{
			mCount++;
			if (mCount % 2 == 0)
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