using UnityEngine;
using System.Collections;

// 管理类初始化完成调用
// 这个父类的添加是方便代码的书写
public class GameBase : FrameBase
{
	public static Game mGame;
	public static GameConfig mGameConfig;
	public static HardwareInfo mHardwareInfo;
	public static SocketManager mSocketNetManager;
	public static SerialPortManager mSerialPortManager;
	public static GameInputManager mGameInputManager;
	public static GameSetting mGameSetting;
	public static RoleSystem mRoleSystem;
	public static RaceSystem mRaceSystem;
	public static SceneItemManager mItemManager;
	public static LogSystem mLogSystem;
	public static RegisterTool mRegisterTool;
	public static WayPointManager mWayPointManager;
	public static GameUtility mGameUtility;
	// 以下是用于快速访问的布局脚本
	public static ScriptLogo mScriptLogo;
	public static ScriptStartVideo mScriptStartVideo;
	public static ScriptStandBy mScriptStandBy;
	public static ScriptSelectRole mScriptSelectRole;
	public static ScriptVolumeSetting mScriptVolumeSetting;
	public static ScriptButtomPrompt mScriptButtomPrompt;
	public static ScriptReturn mScriptReturn;
	public static ScriptSelectTrack mScriptSelectTrack;
	public static ScriptLoading mScriptLoading;
	public static ScriptConfirmSelection mScriptConfirmSelection;
	public static ScriptTopTime mScriptTopTime;
	public static ScriptTrack mScriptTrack;
	public static ScriptProps mScriptProps;
	public static ScriptAiming mScriptAiming;
	public static ScriptCountDown mScriptCountDown;
	public static ScriptSettlement mScriptSettlement;
	public static ScriptCircleTip mScriptCircleTip;
	public static ScriptPlayerRaceInfo mScriptPlayerRaceInfo;
	public static ScriptAttackTip mScriptAttackTip;
	public static ScriptDebugInfo mScriptDebugInfo;
	public static ScriptGlobalAudio mScriptGlobalAudio;
	public override void notifyConstructDone()
	{
		base.notifyConstructDone();
		if (mGame == null)
		{
			mGame = Game.instance as Game;
			mGameConfig = mGame.getSystem<GameConfig>();
			mHardwareInfo = mGame.getSystem<HardwareInfo>();
			mSocketNetManager = mGame.getSystem<SocketManager>();
			mGameInputManager = mGame.getSystem<GameInputManager>();
			mSerialPortManager = mGame.getSystem<SerialPortManager>();
			mGameSetting = mGame.getSystem<GameSetting>();
			mRoleSystem = mGame.getSystem<RoleSystem>();
			mRaceSystem = mGame.getSystem<RaceSystem>();
			mItemManager = mGame.getSystem<SceneItemManager>();
			mLogSystem = mGame.getSystem<LogSystem>();
			mRegisterTool = mGame.getSystem<RegisterTool>();
			mWayPointManager = mGame.getSystem<WayPointManager>();
			mGameUtility = mGame.getSystem<GameUtility>();
		}
	}
}