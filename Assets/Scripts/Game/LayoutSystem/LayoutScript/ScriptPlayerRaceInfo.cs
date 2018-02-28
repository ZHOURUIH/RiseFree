using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class RaceInfo : GameBase
{
	protected ScriptPlayerRaceInfo mScript;
	protected txUGUICanvas mCanvas;
	protected txUIObject mCircleRoot;
	protected txUGUIStaticImage mCircleLabel;
	protected txUGUINumber mCurCircleNumber;
	protected txUGUIStaticImage mSeperate;
	protected txUGUINumber mMaxCircleNumber;
	protected txUGUIStaticImage mCircleIcon;
	protected txUIObject mRankRoot;
	protected txUGUIStaticImage mPreLabel;
	protected txUGUINumber mRankNumber;
	protected txUGUIStaticImage mEndLabel;
	protected txUGUIStaticImage mRankIcon;
	protected txUIObject mSpeedRoot;
	protected txUGUINumber mSpeedNumber;
	protected txUGUIStaticImage mSpeedUnit;
	protected txUGUIStaticImage mSpeedIcon;
	protected txUGUIStaticImage mSpeedColorIcon;
	public RaceInfo(ScriptPlayerRaceInfo script)
	{
		mScript = script;
	}
	public void assignWindow(string rootName)
	{
		mScript.newObject(out mCanvas, rootName);
		mScript.newObject(out mCircleRoot, mCanvas, "CircleRoot");
		mScript.newObject(out mCircleLabel, mCircleRoot, "CircleLabel");
		mScript.newObject(out mCurCircleNumber, mCircleRoot, "CurCircleNumber");
		mScript.newObject(out mSeperate, mCircleRoot, "Seperate");
		mScript.newObject(out mMaxCircleNumber, mCircleRoot, "MaxCircleNumber");
		mScript.newObject(out mCircleIcon, mCircleRoot, "CircleIcon");
		mScript.newObject(out mRankRoot, mCanvas, "RankRoot");
		mScript.newObject(out mPreLabel, mRankRoot, "PreLabel");
		mScript.newObject(out mRankNumber, mRankRoot, "RankNumber");
		mScript.newObject(out mEndLabel, mRankRoot, "EndLabel");
		mScript.newObject(out mRankIcon, mRankRoot, "RankIcon");
		mScript.newObject(out mSpeedRoot, mCanvas, "SpeedRoot");
		mScript.newObject(out mSpeedNumber, mSpeedRoot, "SpeedNumber");
		mScript.newObject(out mSpeedUnit, mSpeedRoot, "SpeedUnit");
		mScript.newObject(out mSpeedIcon, mSpeedRoot, "SpeedIcon");
		mScript.newObject(out mSpeedColorIcon, mSpeedRoot, "SpeedColorIcon");
	}
	public void init()
	{
		mSpeedNumber.setDockingPosition(DOCKING_POSITION.DP_RIGHT);
	}
	public void onReset() { }
	public void setSpeedMS(float speed)
	{
		int speedKMH = (int)(MathUtility.MStoKMH(speed) * GameDefine.DISPLAY_MILEAGE_SCALE);
		mSpeedNumber.setNumber(speedKMH);
	}
	public void setRank(int rank)
	{
		// RankNumber图片中是从0开始
		mRankNumber.setNumber(rank);
	}
	public void setMaxCircle(int circle)
	{
		mMaxCircleNumber.setNumber(circle);
	}
	// circle是玩家已完成的圈数
	public void setCurCircle(int circle)
	{
		circle += 1;
		MathUtility.clamp(ref circle, 0, mRaceSystem.getCurGameTrack().mCircleCount);
		// 需要显示为玩家当前所在的圈数
		mCurCircleNumber.setNumber(circle);
	}
	public void setConnectPlayer(CharacterOther player)
	{
		if(player != null)
		{
			mCanvas.setConnectParent(player.getObject(), new Vector3(0.0f, 1.0f, 0.0f));
		}
		else
		{
			mCanvas.setConnectParent(mScript.getRoot().mObject);
		}
	}
}

public class ScriptPlayerRaceInfo : LayoutScript
{
	protected RaceInfo[] mRaceInfoList;
	public ScriptPlayerRaceInfo(string name, GameLayout layout)
		:
		base(name, layout)
	{
		mRaceInfoList = new RaceInfo[GameDefine.MAX_AI_COUNT + 1];
		int count = mRaceInfoList.Length;
		for (int i = 0; i < count; ++i)
		{
			mRaceInfoList[i] = new RaceInfo(this);
		}
	}
	public override void assignWindow()
	{
		int count = mRaceInfoList.Length;
		for (int i = 0; i < count; ++i)
		{
			mRaceInfoList[i].assignWindow("info" + i);
		}
	}
	public override void init()
	{
		int count = mRaceInfoList.Length;
		for (int i = 0; i < count; ++i)
		{
			mRaceInfoList[i].init();
		}
	}
	public override void onReset()
	{
		int count = mRaceInfoList.Length;
		for (int i = 0; i < count; ++i)
		{
			mRaceInfoList[i].onReset();
		}
	}
	public override void onGameState()
	{
		int count = mRaceInfoList.Length;
		for (int i = 0; i < count; ++i)
		{
			CharacterOther player = mRoleSystem.getPlayer(indexToNumber(i));
			CharacterData data = player.getCharacterData();
			mRaceInfoList[i].setCurCircle(data.mCircle);
			mRaceInfoList[i].setSpeedMS(data.mSpeed);
			mRaceInfoList[i].setRank(data.mRank);
			mRaceInfoList[i].setMaxCircle(mRaceSystem.getCurGameTrack().mCircleCount);
			mRaceInfoList[i].setConnectPlayer(player);
		}
	}
	public override void onShow(bool immediately, string param)
	{
		;
	}
	public override void update(float elapsedTime)
	{
		;
	}
	public override void onHide(bool immediately, string param)
	{
		int count = mRaceInfoList.Length;
		for (int i = 0; i < count; ++i)
		{
			mRaceInfoList[i].setConnectPlayer(null);
		}
	}
	public void notifySpeedMS(int number, float speedMS)
	{
		mRaceInfoList[numberToIndex(number)].setSpeedMS(speedMS);
	}
	public void notifyCurCircle(int number, int circle)
	{
		mRaceInfoList[numberToIndex(number)].setCurCircle(circle);
	}
	public void notifyRank(int number, int rank)
	{
		mRaceInfoList[numberToIndex(number)].setRank(rank);
	}
	//------------------------------------------------------------------------------------------------------------
	// 通过角色的编号获得在数组中的下标
	protected int numberToIndex(int number)
	{
		if(number < 0)
		{
			return 0;
		}
		return number + 1;
	}
	// 通过数组中的下标获得角色编号
	protected int indexToNumber(int index)
	{
		if(index == 0)
		{
			return -1;
		}
		return index - 1;
	}
}

