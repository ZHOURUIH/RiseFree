using UnityEngine;
using System.Collections;
using RenderHeads.Media.AVProVideo;

public class ScriptStartVideo : LayoutScript
{
	protected txNGUIVideo mStartVideo;
	public ScriptStartVideo(string name, GameLayout layout)
		:
		base(name, layout)
	{ }
	public override void assignWindow()
	{
		newObject<txNGUIVideo>(ref mStartVideo, "StartVideo", 1);
	}
	public override void init()
	{
		;
	}
	public override void onShow(bool immediately, string param)
	{
		if(!mStartVideo.setFileName("LogoVideo.mp4"))
		{
			mGameFramework.stop();
			return;
		}
		mStartVideo.setPlayState(PLAY_STATE.PS_PLAY);
		mStartVideo.setVideoEndCallback(onVideoPlayEnd);
		mStartVideo.setVideoReadyCallback(onVideoReady);
	}
	public override void onHide(bool immediately, string param)
	{
		;
	}
	protected void onVideoPlayEnd(string fileName, bool isBreak)
	{
		// 视频播放完毕后通知场景
		LayoutTools.ACTIVE_WINDOW(mStartVideo, false);
		pushCommand<CommandLogoSceneVideoDone>(mGameSceneManager.getCurScene());
	}
	protected void onVideoReady(string fileName, bool isBreak)
	{
		pushDelayCommand<CommandLogoSceneVideoReady>(mGameSceneManager.getCurScene());
	}
}