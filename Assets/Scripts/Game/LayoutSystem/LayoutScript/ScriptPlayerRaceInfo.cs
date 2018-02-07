using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
public class ScriptPlayerRaceInfo : LayoutScript
{
	protected txUIObject mSpeedRoot;
	protected txUINumber mSpeedNumber;
	protected txUIObject mRankRoot;
	protected txUINumber mRankNumber;
	protected txUIObject mCircleRoot;
	protected txUINumber mCurCircleNumber;
	protected txUINumber mMaxCircleNumber;
	public ScriptPlayerRaceInfo(string name, GameLayout layout)
		:
		base(name, layout)
	{
		;
	}
	public override void assignWindow()
	{
		newObject(ref mSpeedRoot, "SpeedRoot");
		newObject(ref mSpeedNumber, mSpeedRoot, "SpeedNumber");
		newObject(ref mRankRoot, "RankRoot");
		newObject(ref mRankNumber, mRankRoot, "RankNumber");
		newObject(ref mCircleRoot, "CircleRoot");
		newObject(ref mCurCircleNumber, mCircleRoot, "CurCircleNumber");
		newObject(ref mMaxCircleNumber, mCircleRoot, "MaxCircleNumber");
	}
	public override void init()
	{
		mSpeedNumber.setDockingPosition(DOCKING_POSITION.DP_RIGHT);
	}
	public override void onReset()
	{
		;
	}
	public override void onGameState()
	{
		setMaxCircle(mRaceSystem.getCurGameTrack().mCircleCount);
		CharacterMyself myself = mCharacterManager.getMyself();
		CharacterData myselfData = myself.getCharacterData();
		setCurCircle(myselfData.mCircle);
		setSpeedMS(myselfData.mSpeed);
		setRank(myselfData.mRank);
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
		;
	}
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
		// 需要显示为玩家当前所在的圈数
		mCurCircleNumber.setNumber(circle + 1);
	}
}

