using UnityEngine;
using System.Collections;

public class ScriptTopTime : LayoutScript
{
	public txNGUIStaticSprite mTimeRoot;
	public txNGUIStaticSprite mTime;
	public txNGUINumber mMin;
	public txNGUINumber mSecond;
	public txNGUIStaticSprite mTimeStart;
	public Vector3 mTimeStartPos;
	public Vector3 mTimeEndPos;
	public ScriptTopTime(string name, GameLayout layout)
		:
		base(name, layout)
	{
		;
	}
	public override void assignWindow()
	{
		newObject(ref mTimeRoot, "TimeRoot");
		newObject(ref mTime, mTimeRoot, "Time");
		newObject(ref mMin, mTime, "Min");
		newObject(ref mSecond, mTime, "Second");
		newObject(ref mTimeStart, "TimeStartPos", 0);
	}
	public override void init()
	{
		mTimeStartPos = mTimeStart.getPosition();
		mTimeEndPos = mTimeRoot.getPosition();
		mMin.setDockingPosition(DOCKING_POSITION.DP_LEFT);
		mMin.setDockingPosition(DOCKING_POSITION.DP_RIGHT);
	}
	public override void onReset()
	{
		LayoutTools.MOVE_WINDOW(mTimeRoot, mTimeStartPos);
		setTime(0);
	}
	public override void onShow(bool immediately, string param)
	{
		LayoutTools.MOVE_WINDOW(mTimeRoot, mTimeStartPos, mTimeEndPos, 0.8f);
		LayoutTools.ALPHA_WINDOW(mTimeRoot, 0.3f, 1.0f, 0.8f);
	}
	public override void onHide(bool immediately, string param)
	{
		LayoutTools.MOVE_WINDOW(mTimeRoot, mTimeStartPos);
	}
	public  void setTime(int time)
	{
		int minutes = 0;
		int seconds = 0;
		if (time >= 0)
		{
			MathUtility.secondsToMinutesSeconds(time, ref minutes, ref seconds);
			mMin.setNumber(minutes, 2);
			mSecond.setNumber(seconds, 2);
		}
	}
}
