using UnityEngine;
using System.Collections;

public class GameSetting : FrameComponent
{
	protected float mCurVolume;                 // 当前音量
	public GameSetting(string name)
		:base(name)
	{}
	public override void init()
	{
		mCurVolume = mGameConfig.getFloatParam(GAME_DEFINE_FLOAT.GDF_VOLUME);
	}
	public void applyToConfig()
	{
		mGameConfig.setFloatParam(GAME_DEFINE_FLOAT.GDF_VOLUME, mCurVolume);
		mGameConfig.writeConfig();
	}
	//获取成员变量
	public float getCurVolume() { return mCurVolume; }

	//设置成员变量
	public void setCurVolume(float v) { mCurVolume = v; }

}
