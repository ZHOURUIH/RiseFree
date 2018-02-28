using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class StartSceneSelectRole : SceneProcedure
{
	public StartSceneSelectRole()
	{ }
	public StartSceneSelectRole(PROCEDURE_TYPE type, GameScene gameScene)
		:
		base(type, gameScene)
	{
		;
	}
	protected override void onInit(SceneProcedure lastProcedure, string intent)
	{
		// 从待机流程和选择赛道流程跳转过来,正常显示布局
		if (!lastProcedure.isThisOrParent(PROCEDURE_TYPE.PT_START_SETTING))
		{
			// 设置摄像机的变换
			RoleDisplay roleDisplay = mSceneSystem.getScene<RoleDisplay>(GameDefine.ROLE_DISPLAY);
			GameCamera mainCamera = mCameraManager.getMainCamera();
			if (!lastProcedure.isThisOrParent(PROCEDURE_TYPE.PT_START_SELECT_TRACK))
			{
				mainCamera.copyCamera(roleDisplay.mCameraPositionObject0);
				LayoutTools.LOAD_NGUI_SHOW(LAYOUT_TYPE.LT_BUTTOM_PROMPT, 9);
			}
			else
			{
				Transform cameraPos0 = roleDisplay.mCameraTransform0;
				ObjectTools.MOVE_OBJECT(mainCamera, mainCamera.getPosition(), cameraPos0.localPosition, 0.5f);
				ObjectTools.ROTATE_OBJECT(mainCamera, mainCamera.getRotation(), cameraPos0.localEulerAngles, 0.5f);
			}
			LayoutTools.LOAD_NGUI_SHOW(LAYOUT_TYPE.LT_SELECT_ROLE, 9);
		}
		// 从设置流程跳转过来,立即显示布局
		else
		{
			LayoutTools.LOAD_NGUI_SHOW(LAYOUT_TYPE.LT_BUTTOM_PROMPT, 9);
		}
		LayoutTools.LOAD_NGUI_SHOW(LAYOUT_TYPE.LT_RETURN, 0);
		// 设置当前选中角色
		CommandStartSceneSelectRole cmd = newCmd(out cmd);
		cmd.mIndex = mRoleSystem.getSelectedIndex();
		pushCommand(cmd, mGameScene);
	}
	protected override void onUpdate(float elapsedTime)
	{
		;
	}
	protected override void onExit(SceneProcedure nextProcedure)
	{
		;
	}
	protected override void onKeyProcess(float elapsedTime)
	{
		// 返回到待机流程
		if(mGameInputManager.getKeyCurrentDown(KeyCode.Y))
		{
			LayoutTools.HIDE_LAYOUT(LAYOUT_TYPE.LT_SELECT_ROLE, true);
			LayoutTools.HIDE_LAYOUT(LAYOUT_TYPE.LT_BUTTOM_PROMPT, true);
			CommandGameSceneChangeProcedure cmd = newCmd(out cmd);
			cmd.mProcedure = PROCEDURE_TYPE.PT_START_STAND_BY;
			pushCommand(cmd, mGameScene);
			return;
		}
		// 进入选择赛道流程
		if (mGameInputManager.getKeyCurrentDown(KeyCode.A))
		{
			LayoutTools.HIDE_LAYOUT(LAYOUT_TYPE.LT_SELECT_ROLE);
			CommandGameScenePrepareChangeProcedure cmd = newCmd(out cmd);
			cmd.mProcedure = PROCEDURE_TYPE.PT_START_SELECT_TRACK;
			cmd.mPrepareTime = 0.5f;
			pushCommand(cmd, mGameScene);
			return;
		}
		// 进入设置流程
		if (mGameInputManager.getKeyCurrentDown(KeyCode.X))
		{
			LayoutTools.HIDE_LAYOUT(LAYOUT_TYPE.LT_BUTTOM_PROMPT);
			CommandGameScenePrepareChangeProcedure cmd = newCmd(out cmd);
			cmd.mProcedure = PROCEDURE_TYPE.PT_START_SETTING;
			cmd.mPrepareTime = 0.5f;
			pushCommand(cmd, mGameScene);
			return;
		}
		// 选择上一个角色
		if (mGameInputManager.turnLeft())
		{
			CommandStartSceneSelectRole cmd = newCmd(out cmd);
			cmd.mIndex = mRoleSystem.getLastIndex();
			pushCommand(cmd, mGameScene);
		}
		// 选择下一个角色
		if (mGameInputManager.turnRight())
		{
			CommandStartSceneSelectRole cmd = newCmd(out cmd);
			cmd.mIndex = mRoleSystem.getNextIndex();
			pushCommand(cmd, mGameScene);
		}	
	}
}