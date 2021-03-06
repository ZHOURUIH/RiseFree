﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

public class GameConfig : ConfigBase
{
	public GameConfig(string name)
		:base(name)
	{ }
	public override void writeConfig()
	{
		FileUtility.writeTxtFile(CommonDefine.F_CONFIG_PATH + "GameFloatConfig.txt", generateFloatFile());
		FileUtility.writeTxtFile(CommonDefine.F_CONFIG_PATH + "GameStringConfig.txt", generateStringFile());
	}
	//-----------------------------------------------------------------------------------------------------------------------
	protected override void addFloat()
	{
		addFloatParam(GAME_DEFINE_FLOAT.GDF_VOLUME);
		addFloatParam(GAME_DEFINE_FLOAT.GDF_KEYBOARD_ENABLE);
		addFloatParam(GAME_DEFINE_FLOAT.GDF_TURN_THRESHOLD);
		addFloatParam(GAME_DEFINE_FLOAT.GDF_TURN_ANGLE_OFFSET);
		addFloatParam(GAME_DEFINE_FLOAT.GDF_TURN_SENSITIVE);
		addFloatParam(GAME_DEFINE_FLOAT.GDF_READ_RPM);
		addFloatParam(GAME_DEFINE_FLOAT.GDF_NORMAL_FRICTION);
		addFloatParam(GAME_DEFINE_FLOAT.GDF_MIN_UPHILL_ANGLE);
		addFloatParam(GAME_DEFINE_FLOAT.GDF_MAX_UPHILL_ANGLE);
		addFloatParam(GAME_DEFINE_FLOAT.GDF_MIN_DOWNHILL_ANGLE);
		addFloatParam(GAME_DEFINE_FLOAT.GDF_MAX_DOWNHILL_ANGLE);
		addFloatParam(GAME_DEFINE_FLOAT.GDF_MIN_UPHILL_FRICTION);
		addFloatParam(GAME_DEFINE_FLOAT.GDF_MAX_UPHILL_FRICTION);
		addFloatParam(GAME_DEFINE_FLOAT.GDF_MIN_DOWNHILL_FRICTION);
		addFloatParam(GAME_DEFINE_FLOAT.GDF_MAX_DOWNHILL_FRICTION);
		addFloatParam(GAME_DEFINE_FLOAT.GDF_AI_BASE_SPEED);
		if (mFloatNameToDefine.Count != (int)GAME_DEFINE_FLOAT.GDF_GAME_MAX - (int)GAME_DEFINE_FLOAT.GDF_GAME_MIN - 1)
		{
			UnityUtility.logError("not all float parameter added!");
		}
	}
	protected override void addString()
	{
		addStringParam(GAME_DEFINE_STRING.GDS_REGISTE_CODE);
		if (mStringNameToDefine.Count != (int)GAME_DEFINE_STRING.GDS_GAME_MAX - (int)GAME_DEFINE_STRING.GDS_GAME_MIN - 1)
		{
			UnityUtility.logError("not all string parameter added!");
		}
	}
	protected override void readConfig()
	{
		readFile(CommonDefine.F_CONFIG_PATH + "GameFloatConfig.txt", true);
		readFile(CommonDefine.F_CONFIG_PATH + "GameStringConfig.txt", false);
	}
}