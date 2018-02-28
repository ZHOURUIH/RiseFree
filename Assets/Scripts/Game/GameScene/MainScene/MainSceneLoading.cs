using UnityEngine;

public class MainSceneLoading : SceneProcedure
{
	protected SceneInstance mSceneInstance;
	public MainSceneLoading()
	{ }
	public MainSceneLoading(PROCEDURE_TYPE type, GameScene gameScene)
		:
		 base(type, gameScene)
	{
		;
	}
	protected override void onInit(SceneProcedure lastProcedure, string intent)
	{
		LayoutTools.LOAD_NGUI_SHOW(LAYOUT_TYPE.LT_LOADING, 1);
		mSceneSystem.loadSceneAsync(mRaceSystem.getTrackName(), true, onSceneLoad);

		mSceneInstance = mSceneSystem.getScene<SceneInstance>(mRaceSystem.getTrackName());
	}
	protected override void onUpdate(float elapsedTime)
	{
		;
	}
	protected override void onExit(SceneProcedure nextProcedure)
	{
		LayoutTools.HIDE_LAYOUT(LAYOUT_TYPE.LT_LOADING);
    }
	protected override void onKeyProcess(float elapsedTime)
	{
		;
	}
	public void onSceneLoad(float progress, bool done, object userData)
	{
		mScriptLoading.setProgress(progress);
		if (done)
		{
			// 初始化场景
			mSceneSystem.initScene(mRaceSystem.getTrackName());
			// 加载结束后在下一帧跳转流程
			CommandGameSceneChangeProcedure cmd = newCmd(out cmd, true, true);
			cmd.mProcedure = PROCEDURE_TYPE.PT_MAIN_READY;
			pushDelayCommand(cmd ,mGameScene);
		}
	}
}