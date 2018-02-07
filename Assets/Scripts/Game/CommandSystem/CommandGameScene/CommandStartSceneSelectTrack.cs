using UnityEngine;
using System.Collections;

public class CommandStartSceneSelectTrack : Command
{
	public int mTrack;
	public override void init()
	{
		base.init();
		mTrack = 0;
	}
	public override void execute()
	{
		mRaceSystem.setTrackIndex(mTrack);
		// 通知布局
		mScriptSelectTrack.showIndex(mTrack);
	}
	public override string showDebugInfo()
	{
		return this.GetType().ToString() + " : track : " + mTrack;
	}
}
