using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class StateGroup
{
	public STATE_GROUP mGroupID;
	public bool mCoexist;  // 该组中的状态是否可以共存
	public List<PLAYER_STATE> mStateList;
	public StateGroup(STATE_GROUP groupID, bool coexist)
	{
		mGroupID = groupID;
		mCoexist = coexist;
		mStateList = new List<PLAYER_STATE>();
	}
	public void addState(PLAYER_STATE state)
	{
		mStateList.Add(state);
	}
	public bool hasState(PLAYER_STATE state)
	{
		return mStateList.Contains(state);
	}
}