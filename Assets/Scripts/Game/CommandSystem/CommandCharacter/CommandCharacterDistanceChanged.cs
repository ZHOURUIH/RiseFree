using UnityEngine;
using System.Collections;

public class CommandCharacterDistanceChanged : Command
{
	public int mWayPoint;
	public float mDistance;
	public override void init()
	{
		base.init();
		mWayPoint = 0;
		mDistance = 0.0f;
	}
	public override void execute()
	{
		Character character = mReceiver as Character;
		CharacterData data = character.getCharacterData();
		data.mCurWayPoint = mWayPoint;
		data.mRunDistance = mDistance;
		data.mTotalDistance = data.mCircle * mWayPointManager.getTotalLength() + data.mRunDistance;
		// 通知布局
		int totalCircleCount = mRaceSystem.getCurGameTrack().mCircleCount;
		float progress = data.mTotalDistance / (mWayPointManager.getTotalLength() * totalCircleCount);
		mScriptTrack.setPlayerProgress(progress, data.mNumber);
	}
	public override string showDebugInfo()
	{
		return this.GetType().ToString() + " : way point : " + mWayPoint + ", distance : " + mDistance;
	}
}