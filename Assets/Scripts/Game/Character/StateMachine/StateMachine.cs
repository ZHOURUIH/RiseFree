using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class StateMachine
{
	protected Dictionary<PLAYER_STATE, Type> mStateTypeList;
	protected Dictionary<PLAYER_STATE, PlayerState> mCurStateList;
	protected Dictionary<PLAYER_STATE, List<STATE_GROUP>> mStateGroupList;		// 查找状态所在的所有组
	protected Dictionary<STATE_GROUP, StateGroup> mGroupStateList;				// 查找该组中的所有状态
	protected CharacterOther mPlayer;
	public StateMachine()
	{
		mStateTypeList = new Dictionary<PLAYER_STATE, Type>();
		mCurStateList = new Dictionary<PLAYER_STATE, PlayerState>();
		mStateGroupList = new Dictionary<PLAYER_STATE, List<STATE_GROUP>>();
		mGroupStateList = new Dictionary<STATE_GROUP, StateGroup>();
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
		registeGroup(STATE_GROUP.SG_SELECT, false);
		registeGroup(STATE_GROUP.SG_GAME, false);
		registeGroup(STATE_GROUP.SG_RIDE, false);
		registeGroup(STATE_GROUP.SG_BUFF, true);
		// 为状态分组
		assignGroup(STATE_GROUP.SG_SELECT, PLAYER_STATE.PS_ON_SELECT_ROLE);
		assignGroup(STATE_GROUP.SG_SELECT, PLAYER_STATE.PS_SELECTED_ROLE);
		assignGroup(STATE_GROUP.SG_SELECT, PLAYER_STATE.PS_UN_SELECT_ROLE);
		assignGroup(STATE_GROUP.SG_GAME, PLAYER_STATE.PS_READY);
		assignGroup(STATE_GROUP.SG_GAME, PLAYER_STATE.PS_FINISH);
		assignGroup(STATE_GROUP.SG_GAME, PLAYER_STATE.PS_GAMING);
		assignGroup(STATE_GROUP.SG_RIDE, PLAYER_STATE.PS_RIDING);
		assignGroup(STATE_GROUP.SG_RIDE, PLAYER_STATE.PS_JUMP);
		assignGroup(STATE_GROUP.SG_RIDE, PLAYER_STATE.PS_IDLE);
		assignGroup(STATE_GROUP.SG_BUFF, PLAYER_STATE.PS_ATTACKED);
		assignGroup(STATE_GROUP.SG_BUFF, PLAYER_STATE.PS_SPRINT);
		assignGroup(STATE_GROUP.SG_BUFF, PLAYER_STATE.PS_PROTECTED);
		assignGroup(STATE_GROUP.SG_BUFF, PLAYER_STATE.PS_AIM);
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
	public bool hasStateGroup(STATE_GROUP group)
	{
		if(!mGroupStateList.ContainsKey(group))
		{
			return false;
		}
		int count0 = mGroupStateList[group].mStateList.Count;
		for(int i = 0; i < count0; ++i)
		{
			if(hasState(mGroupStateList[group].mStateList[i]))
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
			if (!canCoExist(item.Key, type))
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
	protected void assignGroup(STATE_GROUP group, PLAYER_STATE state)
	{
		if (!mStateGroupList.ContainsKey(state))
		{
			mStateGroupList.Add(state, new List<STATE_GROUP>());
		}
		mStateGroupList[state].Add(group);
		if (mGroupStateList.ContainsKey(group))
		{
			mGroupStateList[group].addState(state);
		}
	}
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
	protected void registeGroup(STATE_GROUP group, bool coexist)
	{
		mGroupStateList.Add(group, new StateGroup(group, coexist));
	}
	// 两个状态是否可以共存
	protected bool canCoExist(PLAYER_STATE state0, PLAYER_STATE state1)
	{
		// 任意一个状态没有所属组,则不在同一组
		if(!mStateGroupList.ContainsKey(state0) || !mStateGroupList.ContainsKey(state1))
		{
			return true;
		}
		List<STATE_GROUP> group0 = mStateGroupList[state0];
		List<STATE_GROUP> group1 = mStateGroupList[state1];
		int count0 = group0.Count;
		int count1 = group1.Count;
		for(int i = 0; i < count0; ++i)
		{
			for(int j = 0; j < count1; ++j)
			{
				// 属于同一状态组,并且该状态组中的所有状态都不能共存
				if(group0[i] == group1[j] && !mGroupStateList[group0[i]].mCoexist)
				{
					return false;
				}
			}
		}
		return true;
	}
}
