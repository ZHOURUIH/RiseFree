using UnityEngine;
using System.Collections;

public class CommandStartSceneSelectTrack : Command
{
	public int mTrack;
	public bool mPlayAudio;
	public override void init()
	{
		base.init();
		mTrack = 0;
		mPlayAudio = true;
	}
	public override void execute()
	{
		mRaceSystem.setTrackIndex(mTrack);
		// 通知布局
		mScriptSelectTrack.showIndex(mTrack);
		if(mPlayAudio)
		{
			GameTools.PLAY_AUDIO_UI(mScriptGlobalAudio.getAudioWindow(), SOUND_DEFINE.SD_SELECTION_CHANGE);
		}
	}
	public override string showDebugInfo()
	{
		return this.GetType().ToString() + " : track : " + mTrack;
	}
}
