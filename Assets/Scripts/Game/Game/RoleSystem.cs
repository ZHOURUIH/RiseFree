using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using UnityEngine;

public class RoleSystem : FrameComponent
{
	protected int mSelectedIndex;
	protected List<CharacterOther> mPlayerList;	// 比赛中的所有角色,包括主角
	protected List<CharacterOther> mSelectRoleList; // 用于角色选择的所有角色
	public RoleSystem(string name)
		:base(name)
	{
		mPlayerList = new List<CharacterOther>();
		mSelectRoleList = new List<CharacterOther>();
		mSelectedIndex = 0;
	}
	public override void init()
	{
		;
	}
	public void initSelectRole()
	{
		// 创建选择用的角色
		for(int i = 0; i < GameDefine.ROLE_COUNT; ++i)
		{
			mRoleSystem.createSelectRole(GameDefine.ROLE_MODEL_NAME[i], GameDefine.ROLE_MODEL_NAME[i], i);
		}
		// 清空所有角色的所有状态
		mRoleSystem.clearAllRoleState();
	}
	public override void destroy()
	{
		base.destroy();
	}
	public override void update(float elapsedTime)
	{
		;
	}
	public void createMyself(string name, int startIndex)
	{
		CommandCharacterManagerCreateCharacter cmdCreate = newCmd(out cmdCreate);
		cmdCreate.mCharacterType = CHARACTER_TYPE.CT_MYSELF;
		cmdCreate.mName = name;
		pushCommand(cmdCreate, mCharacterManager);
		CharacterOther player = mCharacterManager.getMyself();
		player.getCharacterData().mStartIndex = startIndex;
		player.initModel(mSelectRoleList[mSelectedIndex].getAvatar().getModelPath());
		// 将玩家加入比赛角色列表
		mPlayerList.Add(player);
	}
	// 创建用于比赛的玩家,number是玩家编号
	public void createAI(string name, string model, int startIndex, int number)
	{
		// 创建玩家
		CommandCharacterManagerCreateCharacter cmdCreate = newCmd(out cmdCreate);
		cmdCreate.mCharacterType = CHARACTER_TYPE.CT_AI;
		cmdCreate.mName = name;
		pushCommand(cmdCreate, mCharacterManager);
		CharacterOther player = mCharacterManager.getCharacter(name) as CharacterOther;
		player.getCharacterData().mStartIndex = startIndex;
		player.getCharacterData().mNumber = number;
		player.initModel(GameDefine.R_CHARACTER_PREFAB_PATH + model);
		// 将玩家加入比赛角色列表
		mPlayerList.Add(player);
	}
	protected void notifyPlayerCreated(CharacterOther player)
	{
		;
	}
	// 创建用于选择的角色
	public void createSelectRole(string name, string model, int id)
	{
		if(mCharacterManager.getCharacter(name) != null)
		{
			return;
		}
		CommandCharacterManagerCreateCharacter cmdCreate = newCmd(out cmdCreate);
		cmdCreate.mCharacterType = CHARACTER_TYPE.CT_OTHER;
		cmdCreate.mName = name;
		cmdCreate.mID = id;
		pushCommand(cmdCreate, mCharacterManager);
		CharacterOther character = mCharacterManager.getCharacter(id) as CharacterOther;
		character.initModel(GameDefine.R_CHARACTER_PREFAB_PATH + model);
		mCharacterManager.activeCharacter(character, false);
		mSelectRoleList.Add(character);
	}
	// 销毁所有比赛角色
	public void destroyAllPlayer()
	{
		int playerCount = mPlayerList.Count;
		for (int i = 0; i < playerCount; ++i)
		{
			CommandCharacterManagerDestroy cmd = newCmd(out cmd);
			cmd.mName = mPlayerList[i].getName();
			pushCommand(cmd, mCharacterManager);
		}
		mPlayerList.Clear();
	}
	// 销毁比赛角色
	public void destroyPlayer(string name)
	{
		// 不能销毁用于选择的角色
		int roleCount = mSelectRoleList.Count;
		for(int i = 0; i < roleCount; ++i)
		{
			if(mSelectRoleList[i].getName() == name)
			{
				UnityUtility.logError("can not destroy select role!");
				return;
			}
		}
		int playerCount = mPlayerList.Count;
		for(int i = 0; i < playerCount; ++i)
		{
			if(mPlayerList[i].getName() == name)
			{
				mPlayerList.RemoveAt(i);
				break;
			}
		}
		CommandCharacterManagerDestroy cmd = newCmd(out cmd);
		cmd.mName = name;
		pushCommand(cmd, mCharacterManager);
	}
	public int getLastIndex() { return (mSelectedIndex - 1 + mSelectRoleList.Count) % mSelectRoleList.Count; }
	public int getNextIndex() { return (mSelectedIndex + 1) % mSelectRoleList.Count; }
	public void setSelectedIndex(int index) { mSelectedIndex = index; }
	public int getSelectedIndex() { return mSelectedIndex; }
	public CharacterOther getSelectedRole(){return mSelectRoleList[mSelectedIndex]; }
	public List<CharacterOther> getSelectRoleList() { return mSelectRoleList; }
	// 隐藏所有除了主角以外的角色
	public void hideAllPlayerExceptMyself()
	{
		int count = mPlayerList.Count;
		for (int i = 0; i < count; ++i)
		{
			if(!mPlayerList[i].isType(CHARACTER_TYPE.CT_MYSELF))
			{
				mCharacterManager.activeCharacter(mPlayerList[i], false);
			}
		}
	}
	// 隐藏所有用于选择的角色
	public void hideAllSelectRole()
	{
		int count = mSelectRoleList.Count;
		for (int i = 0; i < count; ++i)
		{
			mCharacterManager.activeCharacter(mSelectRoleList[i], false);
		}
	}
	// 通知所有玩家准备
	public void notifyAllPlayerReady()
	{
		int count = mPlayerList.Count;
		for (int i = 0; i < count; ++i)
		{
			CommandCharacterAddState cmdState = newCmd(out cmdState);
			cmdState.mState = PLAYER_STATE.PS_READY;
			pushCommand(cmdState, mPlayerList[i]);
		}
	}
	// 通知所有玩家比赛开始
	public void notifyAllPlayerGame()
	{
		int count = mPlayerList.Count;
		for(int i = 0; i < count; ++i)
		{
			CommandCharacterAddState cmdState = newCmd(out cmdState);
			cmdState.mState = PLAYER_STATE.PS_GAMING;
			pushCommand(cmdState, mPlayerList[i]);
		}
	}
	// 通知所有玩家比赛结束
	public void notifyAllPlayerFinish()
	{
		int curTrackCircle = mRaceSystem.getCurGameTrack().mCircleCount;
		int count = mPlayerList.Count;
		for(int i = 0; i < count; ++i)
		{
			// 只通知未完成比赛的玩家
			if(mPlayerList[i].getCharacterData().mCircle < curTrackCircle)
			{
				CommandCharacterNotifyFinish cmdFinish = newCmd(out cmdFinish);
				cmdFinish.mFinish = false;
				pushCommand(cmdFinish, mPlayerList[i]);
			}
		}
	}
	// 清空所有角色的状态
	public void clearAllRoleState()
	{
		int count = mSelectRoleList.Count;
		for (int i = 0; i < count; ++i)
		{
			mSelectRoleList[i].getStateMachine().clearState();
		}
	}
	// 清空所有玩家的所有状态
	public void clearAllPlayerState()
	{
		int count = mPlayerList.Count;
		for (int i = 0; i < count; ++i)
		{
			mPlayerList[i].getStateMachine().clearState();
		}
	}
	public int getPlayerCount()
	{
		return mPlayerList.Count;
	}
	public List<CharacterOther> getAllCharacterList()
	{
		return mPlayerList;
	}
}