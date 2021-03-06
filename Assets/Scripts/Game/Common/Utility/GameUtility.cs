﻿using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Win32;

public class GameUtility : FrameComponent
{
	protected static float mSpeedRatio = 1.0f;
	public static bool mReadRPM = true;
	public static int mGroundLayer;
	public static int mWallLayer;
	public static int mCharacterLayer;
	public static int mJumpPointLayer;
	public GameUtility(string name)
		:base(name)
	{}
	public override void init()
	{
		mGroundLayer = LayerMask.NameToLayer(GameDefine.LAYER_GROUND);
		mWallLayer = LayerMask.NameToLayer(GameDefine.LAYER_WALL);
		mCharacterLayer = LayerMask.NameToLayer(GameDefine.LAYER_CHARACTER);
		mJumpPointLayer = LayerMask.NameToLayer(GameDefine.LAYER_JUMP_POINT);
		mReadRPM = (int)mGameConfig.getFloatParam(GAME_DEFINE_FLOAT.GDF_READ_RPM) != 0;
	}
	public static Color getTrailColorByModelName(string modelName)
	{
		int count = GameDefine.ROLE_MODEL_NAME.Length;
		for(int i = 0; i < count; ++i)
		{
			if(modelName == GameDefine.ROLE_MODEL_NAME[i])
			{
				return GameDefine.PLAYER_TRAIL_COLOR[i];
			}
		}
		return Color.white;
	}
	public static float calcuteConfigExpression(GAME_DEFINE_STRING expDefine, float variableValue)
	{
		string variableStr = "(" + variableValue.ToString("F2") + ")";
		string expression = mGameConfig.getStringParam(expDefine);
		expression = expression.Replace("i", variableStr);
		float expressionValue = MathUtility.calculateFloat(expression);
		return expressionValue;
	}
	// 硬件值转换为踏频(踏频单位为次/分钟)
	public static float HWSToStepFrequency(float hardwareValue)
	{
		if(MathUtility.isFloatZero(hardwareValue))
		{
			return 0.0f;
		}
		return hardwareValue / mSpeedRatio;
	}
	public static float stepFrequencyToHWS(float frequency)
	{
		if (frequency < 0.0f || MathUtility.isFloatZero(frequency))
		{
			return 0.0f;
		}
		return frequency * mSpeedRatio;
	}
	public static float stepFrequencyToMS(float frequency)
	{
		if(frequency <= 0.0f)
		{
			return 0.0f;
		}
		float speed = frequency / 60.0f * 4.0f;
		MathUtility.clamp(ref speed, 0.0f, 20.0f);
		// 假设每踩踏一圈,前进4米
		return speed;
	}
	public static float MSToStepFrequency(float speedMS)
	{
		if (speedMS < 0.0f || MathUtility.isFloatZero(speedMS))
		{
			return 0.0f;
		}
		return speedMS / 4.0f * 60.0f;
	}
	public static float MSToHWS(float speed)
	{
		return stepFrequencyToHWS(MSToStepFrequency(speed));
	}
	public static float HWSToMS(float hwsSpeed)
	{
		return stepFrequencyToMS(HWSToStepFrequency(hwsSpeed));
	}
	// 计算speedCoe
	public static float generateSpeedCoe(float averageSpeed)
	{
		if (averageSpeed < 4.5f)
		{
			return 4.0f;
		}
		else if (averageSpeed >= 4.5f && averageSpeed < 5.4f)
		{
			return 5.0f;
		}
		else if (averageSpeed >= 5.4f && averageSpeed < 6.3f)
		{
			return 6.0f;
		}
		else
		{
			return 7.0f;
		}
	}
	public static void replaceConfigValue(string fileName, string paramName, string paramValue)
	{
		string text = FileUtility.openTxtFile(CommonDefine.F_CONFIG_PATH + fileName);
		int keyWord = text.IndexOf(paramName);
		int pos1 = text.IndexOf('=', keyWord);
		int pos2 = text.IndexOf("\r\n", pos1);
		string str0 = text.Substring(0, pos1 + 1);
		string str2 = text.Substring(pos2, text.Length - pos2);
		string newtext = str0 + paramValue + str2;
		FileUtility.writeTxtFile(CommonDefine.F_CONFIG_PATH + fileName, newtext);
	}
	public static float getSprintIncreaseSpeed(float speed)
	{
		float min = 10.0f;
		float max = 20.0f;
		float increaseSpeed = speed * 0.2f;
		MathUtility.clamp(ref increaseSpeed, min, max);
		return increaseSpeed;
	}
}