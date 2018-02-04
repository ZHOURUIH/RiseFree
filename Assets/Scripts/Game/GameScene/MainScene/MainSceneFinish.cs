using UnityEngine;

// 比赛结束流程
public class MainSceneFinish : SceneProcedure
{
	public MainSceneFinish()
	{ }
	public MainSceneFinish(PROCEDURE_TYPE type, GameScene gameScene)
		:
		 base(type, gameScene)
	{
		;
	}
	protected override void onInit(SceneProcedure lastProcedure, string intent)
	{
		// 给所有角色添加比赛结束状态,如果已经有比赛结束状态的则不会再次添加
		mRoleSystem.notifyAllPlayerFinish();
		// 2秒后显示比赛结束的布局
		// 开始后台异步加载角色展示场景
		mSceneSystem.loadSceneAsync(GameDefine.ROLE_DISPLAY, false, onSceneLoaded);
	}
	protected override void onUpdate(float elapsedTime)
	{
		;
	}
	protected override void onExit(SceneProcedure nextProcedure)
	{
		// 隐藏比赛结束的布局
		// 清空所有角色的所有状态
		mRoleSystem.clearAllPlayerState();
		// 隐藏除了主角以外的所有角色
		mRoleSystem.hideAllPlayerExceptMyself();
		LayoutTools.HIDE_LAYOUT(LAYOUT_TYPE.LT_CIRCLE_TIP);
		LayoutTools.HIDE_LAYOUT(LAYOUT_TYPE.LT_TRACK);
		LayoutTools.HIDE_LAYOUT(LAYOUT_TYPE.LT_PROPS);
		LayoutTools.HIDE_LAYOUT(LAYOUT_TYPE.LT_TOP_TIME);
	}
	protected override void onKeyProcess(float elapsedTime)
	{
		;
	}
	//------------------------------------------------------------------------------------------------------
	protected void onSceneLoaded(float progress, bool done, object userData)
	{
		if(done)
		{
			// 5秒后跳转到结算流程
			CommandGameSceneChangeProcedure cmdProcedure = newCmd(out cmdProcedure, true, true);
			cmdProcedure.mProcedure = PROCEDURE_TYPE.PT_MAIN_SETTLEMENT;
			pushDelayCommand(cmdProcedure, mGameScene, 5.0f);
		}
	}
}