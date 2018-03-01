using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using UnityEngine;

public class GameTools : GameBase
{
	// 场景音效
	#region 播放场景音效
	public static void PLAY_AUDIO_SCENE(SOUND_DEFINE sound)
	{
		ObjectTools.PLAY_AUDIO_SCENE(sound, false, mGameSetting.getCurVolume());
	}
	public static void PLAY_AUDIO_SCENE(string sound)
	{
		ObjectTools.PLAY_AUDIO_SCENE(sound, false, mGameSetting.getCurVolume());
	}
	public static void PLAY_AUDIO_SCENE(SOUND_DEFINE sound, bool loop)
	{
		ObjectTools.PLAY_AUDIO_SCENE(sound, loop, mGameSetting.getCurVolume());
	}
	public static void PLAY_AUDIO_SCENE(string sound, bool loop)
	{
		ObjectTools.PLAY_AUDIO_SCENE(sound, loop, mGameSetting.getCurVolume());
	}
	#endregion
	//------------------------------------------------------------------------------------------------------------------
	// 物体音效
	#region 播放物体音效
	public static void PLAY_AUDIO_OBJECT(MovableObject obj, SOUND_DEFINE sound)
	{
		ObjectTools.PLAY_AUDIO_OBJECT(obj, sound, false, mGameSetting.getCurVolume());
	}
	public static void PLAY_AUDIO_OBJECT(MovableObject obj, string sound)
	{
		ObjectTools.PLAY_AUDIO_OBJECT(obj, sound, false, mGameSetting.getCurVolume());
	}
	public static void PLAY_AUDIO_OBJECT(MovableObject obj, SOUND_DEFINE sound, bool loop)
	{
		ObjectTools.PLAY_AUDIO_OBJECT(obj, sound, loop, mGameSetting.getCurVolume());
	}
	public static void PLAY_AUDIO_OBJECT(MovableObject obj, string sound, bool loop)
	{
		ObjectTools.PLAY_AUDIO_OBJECT(obj, sound, loop, mGameSetting.getCurVolume());
	}
	#endregion
	//------------------------------------------------------------------------------------------------------------------
	// 窗口音效
	#region 播放窗口音效
	public static void PLAY_AUDIO_UI(txUIObject obj, SOUND_DEFINE sound)
	{
		LayoutTools.PLAY_AUDIO(obj, sound, false, mGameSetting.getCurVolume());
	}
	// fileName为sound文件夹的相对路径,
	public static void PLAY_AUDIO_UI(txUIObject obj, string fileName)
	{
		LayoutTools.PLAY_AUDIO(obj, fileName, false, mGameSetting.getCurVolume());
	}
	public static void PLAY_AUDIO_UI(txUIObject obj, SOUND_DEFINE sound, bool loop)
	{
		LayoutTools.PLAY_AUDIO(obj, sound, loop, mGameSetting.getCurVolume());
	}
	// fileName为sound文件夹的相对路径,
	public static void PLAY_AUDIO_UI(txUIObject obj, string fileName, bool loop)
	{
		LayoutTools.PLAY_AUDIO(obj, fileName, loop, mGameSetting.getCurVolume());
	}
	#endregion
}