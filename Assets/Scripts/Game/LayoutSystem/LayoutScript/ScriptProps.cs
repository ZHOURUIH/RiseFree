using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PropsItem
{
	public ScriptProps mScript;
	public txUIStaticSprite mProp;
	public txUIStaticSprite mBackground;		// 底图 不用管
	public txUIStaticSprite mSelect;			// 高亮的底图
	public txUIText mLabel;						// 道具的名称
	public txUIStaticSprite mIcon;				// 道具的图标	
	public Vector3 mOriginPosition;
	public Vector3 mOriginScale;
	public int mOriginPropDepth;
	public int mOriginBackgroundDepth;
	public int mOriginLabelDepth;
	public int mOriginIconDepth;
	public float mControlValueOffset;
	public PropsItem(ScriptProps script)
	{
		mScript = script;
	}
	public void assignWindow(txUIStaticSprite root, string trackName, int index)
	{
		mScript.newObject(ref mProp, root, trackName);
		mScript.newObject(ref mBackground, mProp, "Background");
		mScript.newObject(ref mSelect, mProp, "Select");
		mScript.newObject(ref mLabel, mProp, "Label", 0);
		mScript.newObject(ref mIcon, mProp, "Icon", 0);
	}
	public void init()
	{
		mOriginPosition = mProp.getPosition();
		mOriginScale = mProp.getScale();
		mOriginPropDepth = mProp.getDepth();
		mOriginBackgroundDepth = mBackground.getDepth();
		mOriginLabelDepth = mLabel.getDepth();
		mOriginIconDepth = mIcon.getDepth();
	}
	public void onRest()
	{
		LayoutTools.ACTIVE_WINDOW(mProp, false);
		LayoutTools.ACTIVE_WINDOW(mLabel, false);
		LayoutTools.ACTIVE_WINDOW(mIcon,false);
		setSelected(false);
	}
	public void onShow(float delayTime)
	{
		LayoutTools.ACTIVE_WINDOW_DELAY_EX(mScript, mProp, true, delayTime, onPropsShow);
	}
	public void addProp(PLAYER_ITEM type)
	{
		activeProp(true);
		ItemInfo itemInfo = mScript.mItemInfoList[type];
		mIcon.setSpriteName(itemInfo.mIconSpriteName);
		mIcon.setWindowSize(itemInfo.mSpriteSize);
		mLabel.setLabel(itemInfo.mPropsName);
	}
	public void removeProp()
	{
		activeProp(false);
	}
	public void setControlValueOffset(float offset)
	{
		mControlValueOffset = offset;
	}
	public void setSelected(bool select)
	{
		LayoutTools.ACTIVE_WINDOW(mSelect, select);
	}
	public void activeProp(bool flag)
	{
		mLabel.setActive(flag);
		mIcon.setActive(flag);
	}
	public void lerp(PropsItem curItem, PropsItem nextItem, float percent)
	{
		LayoutTools.MOVE_WINDOW(mProp, MathUtility.lerp(curItem.mOriginPosition, nextItem.mOriginPosition, percent));
		LayoutTools.SCALE_WINDOW(mProp, MathUtility.lerp(curItem.mOriginScale, nextItem.mOriginScale, percent));
		mProp.setDepth((int)MathUtility.lerp(curItem.mOriginPropDepth, nextItem.mOriginPropDepth, percent));
		mBackground.setDepth((int)MathUtility.lerp(curItem.mOriginBackgroundDepth, nextItem.mOriginBackgroundDepth, percent));
		mLabel.setDepth((int)MathUtility.lerp(curItem.mOriginLabelDepth, nextItem.mOriginLabelDepth, percent));
		mIcon.setDepth((int)MathUtility.lerp(curItem.mOriginIconDepth, nextItem.mOriginIconDepth, percent));
	}
	//-----------------------------------------------------------------------------------------------------------------------------------
	protected void onPropsShow(object user_data, Command cmd)
	{
		LayoutTools.SCALE_WINDOW(mProp, new Vector2(0.3f, 0.3f), Vector2.one, 0.5f);
		LayoutTools.ALPHA_WINDOW(mProp, 0.3f, 1.0f, 0.5f);
	}
}
public class ItemInfo
{
	public string mIconSpriteName;
	public string mPropsName;
	public Vector2 mSpriteSize;
	public ItemInfo(UIAtlas atlas, string iconSpriteName, string propsName)
	{
		mIconSpriteName = iconSpriteName;
		mPropsName = propsName;
		UISpriteData spriteData = atlas.GetSprite(iconSpriteName);
		mSpriteSize = new Vector2(spriteData.width, spriteData.height);
	}
}
public class ScriptProps : LayoutScript
{
	public Dictionary<PLAYER_ITEM, ItemInfo> mItemInfoList;
	protected txUIStaticSprite mPropsRoot;
	protected List<PropsItem> mPropsList;				//	道具
	protected txUIObject mControlHelper;
	protected float mStartOffsetValue;		// 本次移动的起始值
	protected float mTargetOffsetValue;		// 本次移动的目标值
	protected float mCurOffsetValue;		// 当前实际的偏移值
	protected int mShowIndex;				// 当前显示选中的下标
	public ScriptProps(string name, GameLayout layout)
		:
		base(name, layout)
	{
		mPropsList = new List<PropsItem>();
		mItemInfoList = new Dictionary<PLAYER_ITEM, ItemInfo>();
	}
	public override void assignWindow()
	{
		newObject(ref mControlHelper, "ControlHelper", 1);
		newObject(ref mPropsRoot, "PropsRoot");
		for (int i = 0; i < GameDefine.PACK_ITEM_COUNT; ++i)
		{
			PropsItem item = new PropsItem(this);
			item.assignWindow(mPropsRoot, "Props" + i, i);
			mPropsList.Add(item);
		}
	}
	public override void init()
	{
		int trackCount = mPropsList.Count;
		for (int i = 0; i < trackCount; ++i)
		{
			mPropsList[i].init();
		}
		trackCount = mPropsList.Count;
		for (int i = 0; i < trackCount; ++i)
		{
			mPropsList[i].setControlValueOffset((float)i / trackCount);
		}
		UIAtlas atlas = mPropsList[0].mIcon.getAtlas();
		mItemInfoList.Add(PLAYER_ITEM.PI_MISSILE, new ItemInfo(atlas, "PropsBarIcon_Missile", "导弹"));
		mItemInfoList.Add(PLAYER_ITEM.PI_SHIELD, new ItemInfo(atlas, "PropsBarIcon_Shield", "护盾"));
		mItemInfoList.Add(PLAYER_ITEM.PI_TURBO, new ItemInfo(atlas, "PropsBarIcon_Accelerate", "加速"));
		mItemInfoList.Add(PLAYER_ITEM.PI_LAND_MINE, new ItemInfo(atlas, "PropsBarIcon_Landmine", "地雷"));
	}
	public override void onReset()
	{
		int trackCount = mPropsList.Count;
		for (int i = 0; i < trackCount; ++i)
		{
			mPropsList[i].onRest();
		}
	}
	public override void onShow(bool immediately, string param)
	{
		float delayTime = 0.0f;
		int count = mPropsList.Count;
		for (int i = 0; i < count; ++i)
		{
			mPropsList[i].onShow(delayTime);
			delayTime += 0.2f;
		}
	}
	public override void onHide(bool immediately, string param)
	{
		;
	}
	public override void update(float elapsedTime)
	{
		;
	}
	public void addProps(PLAYER_ITEM itemType, int itemIndex)
	{
		if (itemIndex > GameDefine.PACK_ITEM_COUNT)
		{
			UnityUtility.logError("itemIndex " + itemIndex + " is out of range count : " + mPropsList.Count);
			return;
		}
		mPropsList[itemIndex].addProp(itemType);
	}
	public void removeProps(int itemIndex)
	{
		mPropsList[itemIndex].removeProp();
	}
	public void showIndex(int index, float time = 0.3f)
	{
		// 设置当前选中的下标,并显示图标
		mShowIndex = index;
		int count = mPropsList.Count;
		for(int i = 0; i < count; ++i)
		{
			mPropsList[i].setSelected(i == mShowIndex);
		}
		// 设置当前起始和目标值
		mStartOffsetValue = mCurOffsetValue;
		mTargetOffsetValue = -mPropsList[mShowIndex].mControlValueOffset;
		// 默认将范围限定到0-1之间
		MathUtility.clampValue(ref mStartOffsetValue, 0.0f, 1.0f, 1.0f);
		MathUtility.clampValue(ref mTargetOffsetValue, 0.0f, 1.0f, 1.0f);
		// 需要保证起始和目标之间不超过0.5,如果超过0.5则转换范围
		if (Mathf.Abs(mStartOffsetValue - mTargetOffsetValue) > 0.5f)
		{
			MathUtility.clampValue(ref mStartOffsetValue, 0.5f, 1.5f, 1.0f);
			MathUtility.clampValue(ref mTargetOffsetValue, 0.5f, 1.5f, 1.0f);
		}
		// 利用辅助物体开始渐变
		LayoutTools.SCALE_WINDOW_EX(mControlHelper, mControlHelper.getScale(), Vector2.one, time, onHelperScaling, null);
	}
	//------------------------------------------------------------------------------------------------------------------------------------------
	protected void onHelperScaling(ComponentKeyFrameBase component, object userData, bool breakTremling, bool done)
	{
		updateItem(MathUtility.lerp(mStartOffsetValue, mTargetOffsetValue, component.getTremblingPercent()));
	}
	protected void updateItem(float controlValue)
	{
		// 变化时需要随时更新当前值
		mCurOffsetValue = controlValue;
		int count = mPropsList.Count;
		for (int i = 0; i < count; ++i)
		{
			PropsItem item = mPropsList[i];
			float value = controlValue + item.mControlValueOffset;
			// 确保值在0-1范围内,并且不能等于1
			while (value >= 1.0f)
			{
				value -= 1.0f;
			}
			int index = (int)(value * count);
			float percent = (value - (float)index / count) / (1.0f / count);
			mPropsList[i].lerp(mPropsList[index], mPropsList[(index + 1) % count], percent);
		}
	}
}
