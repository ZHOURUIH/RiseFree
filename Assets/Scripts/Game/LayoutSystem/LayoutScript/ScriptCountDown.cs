using UnityEngine;
using System.Collections;

public class ScriptCountDown : LayoutScript
{
	protected txUIStaticSprite mNumber;
	protected string[] mNumberNameList;
	protected float mMaxTime = 4.0f;
	public ScriptCountDown(string name, GameLayout layout)
		:
		base(name, layout)
	{
		mNumberNameList = new string[] { "uigo", "ui1", "ui2", "ui3" };
	}
	public override void assignWindow()
	{
		newObject(ref mNumber, "Number");
	}
	public override void init()
	{
		;
	}
	public override void onReset()
	{
		mMaxTime = 4.0f;
		LayoutTools.ACTIVE_WINDOW(mNumber, false);
		LayoutTools.SCALE_WINDOW(mNumber, Vector2.zero);		
	}
	public override void update(float elapsedTime)
	{
		// 大于0.1秒认为是无效更新
		if(mMaxTime >= 0.0f && elapsedTime < 0.1f)
		{
			int lastNumber = (int)mMaxTime;
			mMaxTime -= elapsedTime;
			if((int)mMaxTime != lastNumber)
			{
				setNumber((int)mMaxTime);
			}
			// 倒计时结束,跳转到Gaming流程
			// 需要延迟到下一帧,避免在流程退出中修改布局容器导致迭代器失效
			if(mMaxTime <= 0.0f)
			{
				CommandGameSceneChangeProcedure cmd = newCmd(out cmd, true, true);
				cmd.mProcedure = PROCEDURE_TYPE.PT_MAIN_GAMING;
				pushDelayCommand(cmd, mGameSceneManager.getCurScene());
				mMaxTime = -1.0f;
			}
		}
	}
	public override void onShow(bool immediately, string param)
	{
		;
	}
	public override void onHide(bool immediately, string param)
	{
		;
	}
	//--------------------------------------------------------------------------------------------------
	protected void setNumber(int number)
	{
		LayoutTools.ACTIVE_WINDOW(mNumber);
		mNumber.setSpriteName(mNumberNameList[number]);
		LayoutTools.SCALE_KEYFRAME_WINDOW(mNumber, "CountDown", Vector2.zero, Vector2.one, 0.95f);
	}
}