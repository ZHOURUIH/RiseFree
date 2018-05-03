using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class LayoutRegister : GameBase
{
	public static void registeAllLayout()
	{
		registeLayout<ScriptLogo>(LAYOUT_TYPE.LT_LOGO, "UILogo");											// 公司logo
		registeLayout<ScriptStartVideo>(LAYOUT_TYPE.LT_START_VIDEO, "UIStartVideo");						// 启动视频
		registeLayout<ScriptStandBy>(LAYOUT_TYPE.LT_STAND_BY, "UIStandBy");									// 待机布局
		registeLayout<ScriptSelectRole>(LAYOUT_TYPE.LT_SELECT_ROLE, "UISelectRole");						// 角色选择布局
        registeLayout<ScriptVolumeSetting>(LAYOUT_TYPE.LT_VOLUME_SETTING, "UIVolumeSetting");				// 音量设置
		registeLayout<ScriptButtomPrompt>(LAYOUT_TYPE.LT_BUTTOM_PROMPT, "UIBottomPrompt");					// 底部提示布局
		registeLayout<ScriptReturn>(LAYOUT_TYPE.LT_RETURN, "UIReturn");										// 返回按钮
		registeLayout<ScriptSelectTrack>(LAYOUT_TYPE.LT_SELECT_TRACK, "UISlectTrack");						// 赛道选择
		registeLayout<ScriptLoading>(LAYOUT_TYPE.LT_LOADING, "UILoading");									// 加载布局
		registeLayout<ScriptConfirmSelection>(LAYOUT_TYPE.LT_CONFIRM_SELECTION, "UIConfirmSelection");		// 确定开始游戏布局
		registeLayout<ScriptTopTime>(LAYOUT_TYPE.LT_TOP_TIME, "UITopTime");									// 游戏开始时的顶部时间
		registeLayout<ScriptTrack>(LAYOUT_TYPE.LT_TRACK, "UITrack");										// 游戏开始时的左侧赛道UI
		registeLayout<ScriptProps>(LAYOUT_TYPE.LT_PROPS, "UIProps");										// 游戏开始时右下角的道具布局
		registeLayout<ScriptAiming>(LAYOUT_TYPE.LT_AIMING, "UIAiming");										// 瞄准图标
		registeLayout<ScriptCountDown>(LAYOUT_TYPE.LT_COUNT_DOWN, "UICountDown");							// 倒计时
		registeLayout<ScriptSettlement>(LAYOUT_TYPE.LT_SETTLEMENT, "UISettlement");							// 结算布局	
		registeLayout<ScriptCircleTip>(LAYOUT_TYPE.LT_CIRCLE_TIP, "UICircleTip");							// 圈数提示
		registeLayout<ScriptEndCountDown>(LAYOUT_TYPE.LT_END_COUNT_DOWN, "UIEndCountDown");					// 比赛结束的倒计时
		registeLayout<ScriptDirectionTips>(LAYOUT_TYPE.LT_DIRECTION_TIPS, "UIDirectionTips");				// 方向错误提示
		registeLayout<ScriptPlayerRaceInfo>(LAYOUT_TYPE.LT_PLAYER_RACE_INFO, "UIPlayerRaceInfo");			// 比赛中显示的当前比赛信息
		registeLayout<ScriptAttackTip>(LAYOUT_TYPE.LT_ATTACK_TIP, "UIAttackTip");							// 攻击相关信息
		registeLayout<ScriptDebugInfo>(LAYOUT_TYPE.LT_DEBUG_INFO, "UIDebugInfo");							// 游戏测试的日志
		registeLayout<ScriptGlobalAudio>(LAYOUT_TYPE.LT_GLOBAL_AUDIO, "UIGlobalAudio");						// 游戏测试的日志
		if (mLayoutManager.getLayoutCount() < (int)LAYOUT_TYPE.LT_MAX)
		{
			UnityUtility.logError("error : not all script added! max count : " + (int)LAYOUT_TYPE.LT_MAX + ", added count :" + mLayoutManager.getLayoutCount());
		}
	}
	public static void onScriptChanged(LayoutScript script, bool created = true)
	{
		// 只有布局与脚本唯一对应的才能使用变量快速访问
		if (mLayoutManager.getScriptMappingCount(script.GetType()) > 1)
		{
			return;
		}
		if (assign(ref mScriptLogo, script, created)) return;
		if (assign(ref mScriptStartVideo, script, created)) return;
		if (assign(ref mScriptStandBy, script, created)) return;
		if (assign(ref mScriptSelectRole, script, created)) return;
		if (assign(ref mScriptVolumeSetting, script, created)) return;
		if (assign(ref mScriptButtomPrompt, script, created)) return;
		if (assign(ref mScriptReturn, script, created)) return;
		if (assign(ref mScriptSelectTrack, script, created)) return;
		if (assign(ref mScriptLoading, script, created)) return;
		if (assign(ref mScriptConfirmSelection, script, created)) return;
		if (assign(ref mScriptTopTime, script, created)) return;
		if (assign(ref mScriptTrack, script, created)) return;
		if (assign(ref mScriptProps, script, created)) return;
		if (assign(ref mScriptAiming, script, created)) return;
		if (assign(ref mScriptCountDown, script, created)) return;
		if (assign(ref mScriptSettlement, script, created)) return;
		if (assign(ref mScriptCircleTip, script, created)) return;
		if (assign(ref mScriptPlayerRaceInfo, script, created)) return;
		if (assign(ref mScriptAttackTip, script, created)) return;
		if (assign(ref mScriptDebugInfo, script, created)) return;
		if (assign(ref mScriptGlobalAudio, script, created)) return;
	}
	//----------------------------------------------------------------------------------------------------------------------------------------------------------------
	protected static void registeLayout<T>(LAYOUT_TYPE layout, string name) where T : LayoutScript
	{
		mLayoutManager.registeLayout(typeof(T), layout, name);
	}
	protected static bool assign<T>(ref T thisScript, LayoutScript value, bool created = true) where T : LayoutScript
	{
		if (typeof(T) == value.GetType())
		{
			thisScript = created ? value as T : null;
			return true;
		}
		else
		{
			return false;
		}
	}
}