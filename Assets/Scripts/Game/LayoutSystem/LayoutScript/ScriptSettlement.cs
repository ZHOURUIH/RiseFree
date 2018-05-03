using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ScriptSettlement : LayoutScript
{
	protected txNGUITextureAnim mDataRanking;			// 数据排行序列帧
	protected txNGUITextureAnim mNext;					// 再战一局序列帧
	protected txNGUITextureAnim mRinking;				// 名次
	protected txNGUIStaticSprite mRinkingLabelRoot;
	protected txNGUINumber mRinkingNumber;
	protected txNGUITextureAnim mKcal;					// 能量
	protected txNGUIStaticSprite mKcalLabelRoot;
	protected txNGUINumber mKcalNumber;
	protected txNGUITextureAnim mAverageSpeed;			// 平均速度
	protected txNGUIStaticSprite mAverageSpeedLabelRoot;
	protected txNGUINumber mAverageSpeedNumber;
	protected txNGUITextureAnim mMileage;				// 里程
	protected txNGUIStaticSprite mMileageLabelRoot;
	protected txNGUINumber mMileageNumber;
	protected txNGUITextureAnim mMaxSpeed;				// 最大速度
	protected txNGUIStaticSprite mMaxSpeedLabelRoot;
	protected txNGUINumber mMaxSpeedNumber;
	protected txNGUIStaticSprite mAIcon;					// A图标
	protected List<txNGUITextureAnim> mAminList;
	protected List<txNGUIStaticSprite> mLabelList;
	protected List<txNGUINumber> mNumberList;

	public ScriptSettlement(string name, GameLayout layout)
		:
		base(name, layout)
	{
		mAminList = new List<txNGUITextureAnim>();
		mLabelList = new List<txNGUIStaticSprite>();
		mNumberList = new List<txNGUINumber>();
	}
	public override void assignWindow()
	{
		newObject(out mDataRanking, "DataRanking");
		newObject(out mNext, "Next", 0);
		newObject(out mRinking, "Rinking", 0);
		newObject(out mRinkingLabelRoot, mRinking, "RinkingLabelRoot", 0);
		newObject(out mRinkingNumber, mRinkingLabelRoot, "Number");
		newObject(out mKcal, "Kcal", 0);
		newObject(out mKcalLabelRoot, mKcal, "KcalLabelRoot", 0);
		newObject(out mKcalNumber, mKcalLabelRoot, "Number");
		newObject(out mAverageSpeed, "AverageSpeed", 0);
		newObject(out mAverageSpeedLabelRoot, mAverageSpeed, "AverageLabelRoot", 0);
		newObject(out mAverageSpeedNumber, mAverageSpeedLabelRoot, "Number");
		newObject(out mMileage, "Mileage", 0);
		newObject(out mMileageLabelRoot, mMileage, "MileageLabelRoot", 0);
		newObject(out mMileageNumber, mMileageLabelRoot, "Number");
		newObject(out mMaxSpeed, "MaxSpeed", 0);
		newObject(out mMaxSpeedLabelRoot, mMaxSpeed, "MaxSpeedLabelRoot", 0);
		newObject(out mMaxSpeedNumber, mMaxSpeedLabelRoot, "Number");
		newObject(out mAIcon, "AIcon", 0);
	}
	public override void init()
	{
		mAminList.Add(mRinking);
		mAminList.Add(mKcal);
		mAminList.Add(mAverageSpeed);
		mAminList.Add(mMileage);
		mAminList.Add(mMaxSpeed);

		mLabelList.Add(mRinkingLabelRoot);
		mLabelList.Add(mKcalLabelRoot);
		mLabelList.Add(mAverageSpeedLabelRoot);
		mLabelList.Add(mMileageLabelRoot);
		mLabelList.Add(mMaxSpeedLabelRoot);

		mNumberList.Add(mRinkingNumber);
		mNumberList.Add(mKcalNumber);
		mNumberList.Add(mAverageSpeedNumber);
		mNumberList.Add(mMileageNumber);
		mNumberList.Add(mMaxSpeedNumber);

		for (int i = 0; i < mNumberList.Count; i++)
		{
			mNumberList[i].setDockingPosition(DOCKING_POSITION.DP_RIGHT);
		}
	}
	public override void onReset()
	{
		for (int i = 0; i < mAminList.Count; i++)
		{
			LayoutTools.ACTIVE_WINDOW(mAminList[i], false);
		};
		for (int i = 0; i < mLabelList.Count; i++)
		{
			LayoutTools.ACTIVE_WINDOW(mLabelList[i], false);
		};
		LayoutTools.ACTIVE_WINDOW(mNext, false);
		LayoutTools.ACTIVE_WINDOW(mAIcon, false);
	}
	public override void onGameState()
	{
		Character character = mCharacterManager.getMyself();
		CharacterData data = character.getCharacterData();
		data.mAverageSpeed = data.mTotalDistance / data.mRunTime;
		setPlayerDataNumber(data);
	}
	public override void update(float elapsedTime)
	{
		;
	}
	public override void onShow(bool immediately, string param)
	{
		mDataRanking.setLoop(LOOP_MODE.LM_ONCE);
		mDataRanking.setAutoHide(false);
		mDataRanking.stop(true, false);
		mDataRanking.play();
		float delaytime = 0.0f;
		int count = mAminList.Count;
		for (int i = 0; i < count; i++)
		{
			LayoutTools.ACTIVE_WINDOW_DELAY_EX(this, mAminList[i], true, delaytime, onLabelShow, null);
			delaytime += 0.5f;
			mAminList[i].setLoop(LOOP_MODE.LM_ONCE);
			mAminList[i].setAutoHide(false);
			mAminList[i].stop(true, false);
			mAminList[i].play();
		}
	}
	public override void onHide(bool immediately, string param)
	{
		for (int i = 0; i < mAminList.Count; i++)
		{
			LayoutTools.ACTIVE_WINDOW(mAminList[i], false);
		};
		for (int i = 0; i < mLabelList.Count; i++)
		{
			LayoutTools.ACTIVE_WINDOW(mLabelList[i], false);
		};
		LayoutTools.ACTIVE_WINDOW(mNext, false);
		LayoutTools.ACTIVE_WINDOW(mAIcon, false);
	}
	// 点击的一个效果
	public void AClickEffect()
	{
		LayoutTools.SCALE_WINDOW(mKcal, new Vector2(0.85f, 0.85f), new Vector2(1.0f, 1.0f), 0.8f);
	}
	public void setPlayerDataNumber(CharacterData data)
	{
		// 名次下标是从0开始 这里显示要加1
		int rank = data.mRank + 1;
		mRinkingNumber.setNumber(rank);
		mKcalNumber.setNumber(StringUtility.floatToString(data.mKcal, 1));
		mAverageSpeedNumber.setNumber(StringUtility.floatToString(MathUtility.MStoKMH(data.mAverageSpeed) * GameDefine.DISPLAY_MILEAGE_SCALE, 1));
		mMileageNumber.setNumber(StringUtility.floatToString(MathUtility.MtoKM(data.mTotalDistance) * GameDefine.DISPLAY_MILEAGE_SCALE, 1));
		mMaxSpeedNumber.setNumber(StringUtility.floatToString(MathUtility.MStoKMH(data.mMaxSpeed) * GameDefine.DISPLAY_MILEAGE_SCALE, 1));
	}
	protected void onLabelShow(object user_data, Command cmd)
	{
		int count = mLabelList.Count;
		float delaytime = 0.5f;
		for (int i = 0; i < count; i++)
		{
			LayoutTools.ACTIVE_WINDOW_DELAY(this, mLabelList[i], true, delaytime);
			delaytime += 0.5f;
		}

		LayoutTools.ACTIVE_WINDOW_DELAY(this, mNext, true, 2.5f);
		LayoutTools.ACTIVE_WINDOW_DELAY(this, mAIcon, true, 3.5f);
		mNext.setLoop(LOOP_MODE.LM_ONCE);
		mNext.setAutoHide(false);
		mNext.stop(true, false);
		mNext.play();
	}
}
