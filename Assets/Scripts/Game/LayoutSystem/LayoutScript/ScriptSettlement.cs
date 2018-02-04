using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ScriptSettlement : LayoutScript
{
	protected txUISpriteAnim mDataRanking;			// 数据排行序列帧
	protected txUISpriteAnim mNext;					// 再战一局序列帧
	protected txUISpriteAnim mRinking;				// 名次
	protected txUIStaticSprite mRinkingLabelRoot;
	protected txUINumber mRinkingNumber;
	protected txUISpriteAnim mKcal;					// 能量
	protected txUIStaticSprite mKcalLabelRoot;
	protected txUINumber mKcalNumber;
	protected txUISpriteAnim mAverageSpeed;			// 平均速度
	protected txUIStaticSprite mAverageSpeedLabelRoot;
	protected txUINumber mAverageSpeedNumber;
	protected txUISpriteAnim mMileage;				// 里程
	protected txUIStaticSprite mMileageLabelRoot;
	protected txUINumber mMileageNumber;
	protected txUISpriteAnim mMaxSpeed;				// 最大速度
	protected txUIStaticSprite mMaxSpeedLabelRoot;
	protected txUINumber mMaxSpeedNumber;
	protected txUIStaticSprite mAIcon;					// A图标
	protected List<txUISpriteAnim> mAminList;
	protected List<txUIStaticSprite> mLabelList;
	protected List<txUINumber> mNumberList;

	public ScriptSettlement(LAYOUT_TYPE type, string name, GameLayout layout)
		:
		base(type, name, layout)
	{
		mAminList = new List<txUISpriteAnim>();
		mLabelList = new List<txUIStaticSprite>();
		mNumberList = new List<txUINumber>();
	}
	public override void assignWindow()
	{
		mDataRanking = newObject<txUISpriteAnim>("DataRanking");
		mNext = newObject<txUISpriteAnim>("Next", 0);
		mRinking = newObject<txUISpriteAnim>("Rinking", 0);
		mRinkingLabelRoot = newObject<txUIStaticSprite>(mRinking, "RinkingLabelRoot", 0);
		mRinkingNumber = newObject<txUINumber>(mRinkingLabelRoot, "Number");
		mKcal = newObject<txUISpriteAnim>("Kcal", 0);
		mKcalLabelRoot = newObject<txUIStaticSprite>(mKcal, "KcalLabelRoot", 0);
		mKcalNumber = newObject<txUINumber>(mKcalLabelRoot, "Number");
		mAverageSpeed = newObject<txUISpriteAnim>("AverageSpeed", 0);
		mAverageSpeedLabelRoot = newObject<txUIStaticSprite>(mAverageSpeed, "AverageLabelRoot", 0);
		mAverageSpeedNumber = newObject<txUINumber>(mAverageSpeedLabelRoot, "Number");
		mMileage = newObject<txUISpriteAnim>("Mileage", 0);
		mMileageLabelRoot = newObject<txUIStaticSprite>(mMileage, "MileageLabelRoot", 0);
		mMileageNumber = newObject<txUINumber>(mMileageLabelRoot, "Number");
		mMaxSpeed = newObject<txUISpriteAnim>("MaxSpeed", 0);
		mMaxSpeedLabelRoot = newObject<txUIStaticSprite>(mMaxSpeed, "MaxSpeedLabelRoot", 0);
		mMaxSpeedNumber = newObject<txUINumber>(mMaxSpeedLabelRoot, "Number");
		mAIcon = newObject<txUIStaticSprite>("AIcon", 0);
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
		mKcalNumber.setNumber(StringUtility.floatToString(data.mKcal,1));
		mAverageSpeedNumber.setNumber(StringUtility.floatToString(MathUtility.MStoKMH(data.mAverageSpeed),1));
		mMileageNumber.setNumber(StringUtility.floatToString(MathUtility.MtoKM(data.mTotalDistance),1));
		mMaxSpeedNumber.setNumber(StringUtility.floatToString(MathUtility.MStoKMH(data.mMaxSpeed),1));
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
