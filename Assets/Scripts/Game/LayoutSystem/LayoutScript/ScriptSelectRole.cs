using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class RoleSelection
{
	public ScriptSelectRole mScript;
	public txUIObject mRoleRoot;
	public txUITextureAnim mRole;
	public txUIObject mEnd;
	public Vector3 mStartPosition;
	public Vector3 mEndPosition;
	public int mSelected;   // -1表示无效值,0表示未选中,1表示选中
	public bool mHideDone;
	public RoleSelection(ScriptSelectRole script)
	{
		mScript = script;
	}
	public void assignWindow(string name)
	{
		mScript.newObject(ref mRoleRoot, name, 1);
		mScript.newObject(ref mRole, mRoleRoot, "Role");
		mScript.newObject(ref mEnd, mRoleRoot, "End", 0);
	}
	public void init()
	{
		mStartPosition = mRole.getPosition();
		mEndPosition = mEnd.getPosition();
		mRole.setAutoHide(false);
	}
	public void onReset()
	{
		LayoutTools.MOVE_WINDOW(mRole, mStartPosition);
		LayoutTools.SCALE_WINDOW(mRole, Vector2.one);
		LayoutTools.ALPHA_WINDOW(mRole, 1.0f);
		LayoutTools.ACTIVE_WINDOW(mRole, false);
		mSelected = -1;
		mHideDone = true;
	}
	public void onShow(bool immediately, float delay)
	{
		if (immediately)
		{
			mHideDone = false;
			LayoutTools.ACTIVE_WINDOW(mRole);
			mRole.stop();
			mRole.setCurFrameIndex(mRole.getTextureFrameCount() - 1);
		}
		else
		{
			LayoutTools.ACTIVE_WINDOW_DELAY_EX(mScript, mRole, true, delay, onRoleShow);
		}
	}
	public void onHide()
	{
		// 当前选项已经显示,则执行正常隐藏逻辑
		if(mRole.isActive())
		{
			LayoutTools.MOVE_WINDOW(mRole, mRole.getPosition(), mEndPosition, 0.35f);
			LayoutTools.ALPHA_WINDOW_EX(mRole, 1.0f, 0.0f, 0.35f, onRoleHide);
		}
		// 如果当前选项没有显示,则表示其显示命令被中断了,则直接通知布局该选项隐藏完毕
		else
		{
			mHideDone = true;
		}
	}
	public void select(bool select, bool force = false)
	{
		int curSel = select ? 1 : 0;
		if(curSel == mSelected && !force)
		{
			return;
		}
		mSelected = curSel;
		// 只能在布局显示完毕后才能执行选中逻辑,否则会与显示逻辑发生冲突
		if(mScript.isShowDone())
		{
			if (mSelected == 1)
			{
				LayoutTools.ALPHA_WINDOW(mRole, 1.0f);
				LayoutTools.SCALE_WINDOW(mRole, mRole.getScale(), new Vector2(1.2f, 1.2f), 0.2f);
			}
			else
			{
				LayoutTools.ALPHA_WINDOW(mRole, 0.7f);
				LayoutTools.SCALE_WINDOW(mRole, Vector2.one);
			}
		}
	}
	public bool isSelected(){return mSelected == 1;}
	public void hideSelection()
	{
		LayoutTools.SCALE_WINDOW_EX(mRole, mRole.getScale(), new Vector2(0.8f, 0.8f), 0.15f, onSelectionHide);
	}
	//-------------------------------------------------------------------------------------------------------
	protected void onRoleShow(object user_data, Command cmd)
	{
		mHideDone = false;
		mRole.stop();
		mRole.play();
		mScript.notifyRoleShowDone(this);
	}
	protected void onRoleHide(ComponentKeyFrameBase component, object userData, bool breakTremling, bool done)
	{
		if(breakTremling)
		{
			return;
		}
		mHideDone = true;
		mScript.notifyRoleHideDone(this);
	}
	protected void onSelectionHide(ComponentKeyFrameBase component, object userData, bool breakTremling, bool done)
	{
		if (breakTremling)
		{
			return;
		}
		mScript.notifySelectionHideDone();
	}
}

public class ScriptSelectRole : LayoutScript
{
	protected txUISpriteAnim mSelectionRoleTitle;      // "角色选择"标题序列帧
	protected txUISpriteAnim mFemale;                  // 女角色按钮
	protected List<RoleSelection> mRoleSelectionList;
	protected bool mShowDone = false;
	public ScriptSelectRole(string name, GameLayout layout)
		:
		base(name, layout)
	{
		mRoleSelectionList = new List<RoleSelection>();
	}
	public override void assignWindow()
	{
		newObject(ref mSelectionRoleTitle, "SelectionRoleTitle", 0);
		for(int i = 0; i < GameDefine.ROLE_COUNT; ++i)
		{
			RoleSelection selection = new RoleSelection(this);
			selection.assignWindow("RoleRoot" + i);
			mRoleSelectionList.Add(selection);
		}
	}
	public override void init()
	{
		mSelectionRoleTitle.setAutoHide(false);
		mLayout.setScriptControlHide(true);
		int count = mRoleSelectionList.Count;
		for (int i = 0; i < count; ++i)
		{
			mRoleSelectionList[i].init();
		}
	}
	public override void onReset()
	{
		LayoutTools.ACTIVE_WINDOW(mSelectionRoleTitle, false);
		int count = mRoleSelectionList.Count;
		for (int i = 0; i < count; ++i)
		{
			mRoleSelectionList[i].onReset();
		}
		mShowDone = false;
	}
	public override void onGameState()
	{
		// 设置所有选项的选中状态,由于此时还未显示完毕
		// 所以只是在选项内保存了一个是否选中的状态,并没有执行选中逻辑
		int selectIndex = mRoleSystem.getSelectedIndex();
		int count = mRoleSelectionList.Count;
		for (int i = 0; i < count; ++i)
		{
			mRoleSelectionList[i].select(i == selectIndex);
		}
	}
	public override void onShow(bool immediately, string param)
	{
		LayoutTools.ACTIVE_WINDOW(mSelectionRoleTitle);
		mSelectionRoleTitle.stop();
		if (immediately)
		{	
			mSelectionRoleTitle.setCurFrameIndex(mSelectionRoleTitle.getTextureFrameCount() - 1);
			showDone();
		}
		else
		{
			mSelectionRoleTitle.play();
		}
		int count = mRoleSelectionList.Count;
		for (int i = 0; i < count; ++i)
		{
			mRoleSelectionList[i].onShow(immediately, immediately ? 0.0f : i * 0.2f);
		}
	}
	public override void onHide(bool immediately, string param)
	{
		if (immediately)
		{
			hideDone();
			return;
		}
		// 先隐藏选中的项
		RoleSelection selection = null;
		int count = mRoleSelectionList.Count;
		for (int i = 0; i < count; ++i)
		{
			if(mRoleSelectionList[i].mSelected == 1)
			{
				selection = mRoleSelectionList[i];
				break;
			}
		}
		if(selection != null)
		{
			selection.hideSelection();
		}
		// 如果没有选中的项,则直接开始隐藏布局
		else
		{
			notifySelectionHideDone();
		}
	}
	public override void update(float elapsedTime)
	{
		;
	}
	// 选择一个角色
	public void selectRole(int index)
	{
		int count = mRoleSelectionList.Count;
		for (int i = 0; i < count; ++i)
		{
			mRoleSelectionList[i].select(i == index);
		}
	}
	public void notifyRoleShowDone(RoleSelection role)
	{
		// 如果最后一个选项已经显示完毕,则设置布局显示完毕
		if(role == mRoleSelectionList[mRoleSelectionList.Count - 1])
		{
			showDone();
		}
	}
	public void notifyRoleHideDone(RoleSelection role)
	{
		// 如果所有选项都已经隐藏完毕,则隐藏布局
		bool allHideDone = true;
		int count = mRoleSelectionList.Count;
		for(int i = 0; i < count; ++i)
		{
			if(!mRoleSelectionList[i].mHideDone)
			{
				allHideDone = false;
				break;
			}
		}
		if(allHideDone)
		{
			hideDone();
		}
	}
	// 选中项已经隐藏完毕,开始执行真正地隐藏逻辑
	public void notifySelectionHideDone()
	{
		int count = mRoleSelectionList.Count;
		for (int i = 0; i < count; ++i)
		{
			mRoleSelectionList[i].onHide();
		}
	}
	public bool isShowDone() { return mShowDone; }
	//------------------------------------------------------------------------------------------------------------------------------
	protected void hideDone()
	{
		LayoutTools.HIDE_LAYOUT_FORCE(mType);
	}
	protected void showDone()
	{
		mShowDone = true;
		// 确定当前选中角色
		int count = mRoleSelectionList.Count;
		for (int i = 0; i < count; ++i)
		{
			mRoleSelectionList[i].select(mRoleSelectionList[i].isSelected(), true);
		}
	}
}
