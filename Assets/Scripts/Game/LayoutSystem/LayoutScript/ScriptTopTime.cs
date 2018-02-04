using UnityEngine;
using System.Collections;

public class ScriptTopTime : LayoutScript
{
	public txUIStaticSprite mTimeRoot;
	public txUIStaticSprite mTime;
	public txUINumber mMin;
	public txUINumber mSecond;
	public txUIStaticSprite mTimeStart;
	public Vector3 mTimeStartPos;
	public Vector3 mTimeEndPos;
	public ScriptTopTime(LAYOUT_TYPE type, string name, GameLayout layout)
		:
		base(type, name, layout)
	{
		;
	}
	public override void assignWindow()
	{
		mTimeRoot = newObject<txUIStaticSprite>("TimeRoot");
		mTime = newObject<txUIStaticSprite>(mTimeRoot, "Time");
		mMin = newObject<txUINumber>(mTime, "Min");
		mSecond = newObject<txUINumber>(mTime, "Second");
		mTimeStart = newObject<txUIStaticSprite>("TimeStartPos", 0);
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
