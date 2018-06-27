using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Win32;

public class LogoSceneRegisterCheck : SceneProcedure
{
	public LogoSceneRegisterCheck()
	{ }
	public LogoSceneRegisterCheck(PROCEDURE_TYPE type, GameScene gameScene)
		:
		base(type, gameScene)
	{
		;
	}
	protected override void onInit(SceneProcedure lastProcedure, string intent)
	{
		// 不进行注册检测
		//#if UNITY_EDITOR
		CommandGameSceneChangeProcedure cmd = newCmd(out cmd, true, true);
		cmd.mProcedure = PROCEDURE_TYPE.PT_LOGO_LOGO;
		pushDelayCommand(cmd, mGameScene);
		//#else
		//		string requestCode = mRegisterTool.generateRequestCode();
		//		string newRegisteCode = mRegisterTool.generateRegisteCode(requestCode, GameDefine.REGISTE_KEY);
		//		string codeInRegistry = readRegistryValue(GameDefine.COMPANY_NAME, GameDefine.GAME_NAME);
		//		// 判断与注册表中的值是否一致
		//		bool inRegistry = codeInRegistry == newRegisteCode;
		//		bool inConfig = false;
		//		// 判断与配置文件中的值是否一致
		//		// 注册表里面没有注册码
		//		if(!inRegistry)
		//		{
		//			string codeInConfig = mGameConfig.getStringParam(GAME_DEFINE_STRING.GDS_REGISTE_CODE);
		//			inConfig = codeInConfig == newRegisteCode;
		//			// 配置文件中有,并且注册表中没有,则需要写入注册表
		//			if (inConfig)
		//			{
		//				writeRegistryValue(GameDefine.COMPANY_NAME, GameDefine.GAME_NAME, newRegisteCode);
		//			}
		//		}
		//		// 注册表里面有注册码
		//		else
		//		{
		//			string codeInConfig = mGameConfig.getStringParam(GAME_DEFINE_STRING.GDS_REGISTE_CODE);
		//			inConfig = codeInConfig == newRegisteCode;
		//			// 配置文件中没有,并且注册表中有,则需要同步到配置文件中
		//			if (!inConfig)
		//			{
		//				GameUtility.replaceConfigValue("GameStringConfig.txt","GDS_REGISTE_CODE", newRegisteCode);
		//			}
		//		}
		//		// 验证通过,则进入游戏
		//		if (inRegistry || inConfig)
		//		{
		//			CommandGameSceneChangeProcedure cmd = newCmd(out cmd, true, true);
		//			cmd.mProcedure = PROCEDURE_TYPE.PT_LOGO_LOGO;
		//			pushDelayCommand(cmd, mGameScene);
		//		}
		//		// 验证失败提示未注册
		//		else
		//		{
		//			UnityUtility.copyTextToClipbord(requestCode);
		//			UnityUtility.logError("该电脑未注册,请联系客服进行注册! 设备码已复制到剪贴板,设备码 : " + requestCode);
		//			mGameFramework.stop();
		//		}
		//#endif
	}
	protected override void onUpdate(float elapsedTime)
	{
		;
	}
	protected override void onExit(SceneProcedure nextProcedure)
	{

	}
	protected override void onKeyProcess(float elapsedTime)
	{
		;
	}
	//--------------------------------------------------------------------------------------------------------------------------------------------------
	// 从注册表中获取注册码,如果注册码不存在,则自动创建
	protected string readRegistryValue(string companyName, string gameName)
	{
		string value = "";
		RegistryKey key = Registry.CurrentUser.CreateSubKey("SOFTWARE\\" + companyName + "\\" + gameName);
		string codeValue = key.GetValue("REGISTE_CODE") as string;
		if (codeValue != null)
		{
			value = key.GetValue("REGISTE_CODE") as string;
		}
		key.Close();
		return value;
	}
	protected void writeRegistryValue(string companyName, string gameName, string registeCode)
	{
		RegistryKey key = Registry.CurrentUser.CreateSubKey("SOFTWARE\\" + companyName + "\\" + gameName);
		key.SetValue("REGISTE_CODE", registeCode);
		key.Close();
	}
}