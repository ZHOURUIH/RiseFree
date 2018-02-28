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
	protected SortedDictionary<int, CharacterOther> mPlayerList;	// key是角色的编号,比赛中的所有角色,包括主角
	protected List<CharacterOther> mSelectRoleList;					// 用于角色选择的所有角色
	public RoleSystem(string name)
		:base(name)
	{
		mPlayerList = new SortedDictionary<int, CharacterOther>();
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
		// 将玩家加入比赛角色列表,并确保位于第一个
		mPlayerList.Add(player.getCharacterData().mNumber, player);
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
		CharacterData data = player.getCharacterData();
		data.mStartIndex = startIndex;
		data.mNumber = number;
		player.initModel(GameDefine.R_CHARACTER_PREFAB_PATH + model);
		// 将玩家加入比赛角色列表
		mPlayerList.Add(data.mNumber, player);
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
		foreach (var item in mPlayerList)
		{
			CommandCharacterManagerDestroy cmd = newCmd(out cmd);
			cmd.mName = item.Value.getName();
			pushCommand(cmd, mCharacterManager);
		}
		mPlayerList.Clear();
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
		foreach (var item in mPlayerList)
		{
			if(!item.Value.isType(CHARACTER_TYPE.CT_MYSELF))
			{
				mCharacterManager.activeCharacter(item.Value, false);
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
		foreach (var item in mPlayerList)
		{
			CommandCharacterAddState cmdState = newCmd(out cmdState);
			cmdState.mState = PLAYER_STATE.PS_READY;
			pushCommand(cmdState, item.Value);
		}
	}
	// 通知所有玩家比赛开始
	public void notifyAllPlayerGame()
	{
		foreach (var item in mPlayerList)
		{
			CommandCharacterAddState cmdState = newCmd(out cmdState);
			cmdState.mState = PLAYER_STATE.PS_GAMING;
			pushCommand(cmdState, item.Value);
		}
	}
	// 通知所有玩家比赛结束
	public void notifyAllPlayerFinish()
	{
		foreach (var item in mPlayerList)
		{
			// 只通知未完成比赛的玩家
			if (!item.Value.hasState(PLAYER_STATE.PS_FINISH))
			{
				CommandCharacterNotifyFinish cmdFinish = newCmd(out cmdFinish);
				cmdFinish.mFinish = false;
				pushCommand(cmdFinish, item.Value);
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
		foreach (var item in mPlayerList)
		{
			item.Value.getStateMachine().clearState();
		}
	}
	public int getPlayerCount(){return mPlayerList.Count;}
	public SortedDictionary<int, CharacterOther> getAllPlayer(){return mPlayerList;}
	public CharacterOther getPlayer(int number)
	{
		if(mPlayerList.ContainsKey(number))
		{
			return mPlayerList[number];
		}
		return null;
	}
}