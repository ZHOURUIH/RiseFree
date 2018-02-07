using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class StateMachine
{
	protected Dictionary<PLAYER_STATE, Type> mStateTypeList;
	protected Dictionary<PLAYER_STATE, PlayerState> mCurStateList;
	protected Dictionary<PLAYER_STATE, List<int>> mStateGroupList;		// 查找状态所在的所有组
	protected Dictionary<int, List<PLAYER_STATE>> mGroupStateList;		// 查找该组中的所有状态
	protected CharacterOther mPlayer;
	public StateMachine()
	{
		mStateTypeList = new Dictionary<PLAYER_STATE, Type>();
		mCurStateList = new Dictionary<PLAYER_STATE, PlayerState>();
		mStateGroupList = new Dictionary<PLAYER_STATE, List<int>>();
		mGroupStateList = new Dictionary<int, List<PLAYER_STATE>>();
	}
	public void init(CharacterOther player)
	{
		mPlayer = player;
		registeState<JumpState>(PLAYER_STATE.PS_JUMP);
		registeState<RidingState>(PLAYER_STATE.PS_RIDING);
		registeState<IdleState>(PLAYER_STATE.PS_IDLE);
		registeState<AimState>(PLAYER_STATE.PS_AIM);
		registeState<AttackedState>(PLAYER_STATE.PS_ATTACKED);
		registeState<ProtectedState>(PLAYER_STATE.PS_PROTECTED);
		registeState<SprintState>(PLAYER_STATE.PS_SPRINT);
		registeState<WrongDirectionState>(PLAYER_STATE.PT_WRONG_DIRECTION);
		registeState<ReadyState>(PLAYER_STATE.PS_READY);
		registeState<GamingState>(PLAYER_STATE.PS_GAMING);
		registeState<FinishState>(PLAYER_STATE.PS_FINISH);
		registeState<OnSelectRoleState>(PLAYER_STATE.PS_ON_SELECT_ROLE);
		registeState<SelectedRoleState>(PLAYER_STATE.PS_SELECTED_ROLE);
		registeState<UnSelectRoleState>(PLAYER_STATE.PS_UN_SELECT_ROLE);
		if(mStateTypeList.Count != (int)PLAYER_STATE.PS_MAX)
		{
			UnityUtility.logError("not all player state registed!");
		}
		// 为状态分组
		// 选中状态
		assignGroup(0, PLAYER_STATE.PS_ON_SELECT_ROLE);
		assignGroup(0, PLAYER_STATE.PS_SELECTED_ROLE);
		assignGroup(0, PLAYER_STATE.PS_UN_SELECT_ROLE);
		// 游戏状态
		assignGroup(1, PLAYER_STATE.PS_READY);
		assignGroup(1, PLAYER_STATE.PS_FINISH);
		assignGroup(1, PLAYER_STATE.PS_GAMING);
		// 骑行状态
		assignGroup(2, PLAYER_STATE.PS_RIDING);
		assignGroup(2, PLAYER_STATE.PS_JUMP);
		assignGroup(2, PLAYER_STATE.PS_IDLE);
		// 增益状态和负面状态
		assignGroup(3, PLAYER_STATE.PS_ATTACKED);
		assignGroup(3, PLAYER_STATE.PS_SPRINT);
		assignGroup(3, PLAYER_STATE.PS_PROTECTED);
		assignGroup(3, PLAYER_STATE.PS_AIM);
	}
	public void assignGroup(int group, PLAYER_STATE state)
	{
		if(mStateGroupList.ContainsKey(state))
		{
			mStateGroupList[state].Add(group);
		}
		else
		{
			List<int> list = new List<int>();
			list.Add(group);
			mStateGroupList.Add(state, list);
		}
		if(mGroupStateList.ContainsKey(group))
		{
			mGroupStateList[group].Add(state);
		}
		else
		{
			List<PLAYER_STATE> list = new List<PLAYER_STATE>();
			list.Add(state);
			mGroupStateList.Add(group, list);
		}
	}
	public void update(float elapsedTime)
	{
		// 只有myself才能响应按键
		bool processKey = mPlayer.isType(CHARACTER_TYPE.CT_MYSELF) && (mPlayer as CharacterOther).getProcessKey();
		// 更新所有状态,复制一份列表,避免在更新中修改mCurStateList时报错
		Dictionary<PLAYER_STATE, PlayerState> tempList = new Dictionary<PLAYER_STATE, PlayerState>(mCurStateList);
		foreach (var item in tempList)
		{
			if (item.Value.getActive())
			{
				item.Value.update(elapsedTime);
				if(processKey)
				{
					item.Value.keyProcess(elapsedTime);
				}
			}
		}
	}
	public void fixedUpdate(float elapsedTime)
	{
		foreach (var item in mCurStateList)
		{
			if(item.Value.getActive())
			{
				item.Value.fixedUpdate(elapsedTime);
			}
		}
	}
	// 是否拥有该组的任意一个状态
	public bool hasStateGroup(int group)
	{
		if(!mGroupStateList.ContainsKey(group))
		{
			return false;
		}
		int count0 = mGroupStateList[group].Count;
		for(int i = 0; i < count0; ++i)
		{
			if(hasState(mGroupStateList[group][i]))
			{
				return true;
			}
		}
		return false;
	}
	// 是否拥有指定状态
	public bool hasState(PLAYER_STATE type)
	{
		return mCurStateList.ContainsKey(type) && mCurStateList[type].getActive();
	}
	public bool addState(PLAYER_STATE type)
	{
		if(mCurStateList.ContainsKey(type))
		{
			return false;
		}
		// 创建状态,并且判断是否可以进入该状态,不能进入则直接返回
		PlayerState state = createState(type);
		state.setPlayer(mPlayer);
		if(!state.canEnter())
		{
			return false;
		}
		// 先移除不能共存的状态
		Dictionary<PLAYER_STATE, PlayerState> tempList = new Dictionary<PLAYER_STATE, PlayerState>(mCurStateList);
		foreach (var item in tempList)
		{
			if (isSameGroup(item.Key, type))
			{
				removeState(item.Key);
			}
		}
		// 进入状态,并添加到状态列表
		state.enter();
		mCurStateList.Add(type, state);
		// 通知角色有状态添加
		mPlayer.notifyStateChanged();
		return true;
	}
	public bool removeState(PLAYER_STATE type)
	{
		if (!mCurStateList.ContainsKey(type))
		{
			return false;
		}
		mCurStateList[type].leave();
		mCurStateList[type].setActive(false);
		mCurStateList.Remove(type);
		// 通知角色有状态移除
		mPlayer.notifyStateChanged();
		return true;
	}
	public void clearState()
	{
		Dictionary<PLAYER_STATE, PlayerState> tempList = new Dictionary<PLAYER_STATE, PlayerState>(mCurStateList);
		foreach (var item in tempList)
		{
			removeState(item.Key);
		}
	}
	public Dictionary<PLAYER_STATE, PlayerState> getStateList() { return mCurStateList; }
	//----------------------------------------------------------------------------------------------------------------
	protected PlayerState createState(PLAYER_STATE type)
	{
		if(!mStateTypeList.ContainsKey(type))
		{
			UnityUtility.logError("state not registed! type : " + type);
			return null;
		}
		PlayerState state = UnityUtility.createInstance<PlayerState>(mStateTypeList[type], type);
		return state;
	}
	protected void registeState<T>(PLAYER_STATE state) where T : PlayerState
	{
		mStateTypeList.Add(state, typeof(T));
	}
	protected bool isSameGroup(PLAYER_STATE state0, PLAYER_STATE state1)
	{
		// 任意一个状态没有所属组,则不在同一组
		if(!mStateGroupList.ContainsKey(state0) || !mStateGroupList.ContainsKey(state1))
		{
			return false;
		}
		List<int> group0 = mStateGroupList[state0];
		List<int> group1 = mStateGroupList[state1];
		int count0 = group0.Count;
		int count1 = group1.Count;
		for(int i = 0; i < count0; ++i)
		{
			for(int j = 0; j < count1; ++j)
			{
				if(group0[i] == group1[j])
				{
					return true;
				}
			}
		}
		return false;
	}
}
