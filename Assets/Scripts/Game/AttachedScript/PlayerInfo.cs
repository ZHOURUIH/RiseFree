using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
	protected Animation mAnimation;
	protected CharacterData mCharacterData;
	public List<PLAYER_STATE> mState;
	public CharacterOther mPlayer;
	public Vector3 mCenterOfMass;
	public float mSpeed;
	public float mTargetSpeed;
	public int mCircle;			// 当前已经完成的圈数
	public int mCurWayPoint;    // 当前所在路段的下标
	public float mRunDistance;  // 当前圈跑过的距离
	public float mTotalDistance;// 该赛道跑过的总距离
	public bool mProcessKey;
	public bool mProcessTurn;
	public bool mProcessExternalSpeed;
	public List<string> mActiveComponentList;
	public List<string> mActivedAnimationStateList;
	public List<string> mAllAnimationStateList;
	public void Awake()
	{
		mActiveComponentList = new List<string>();
		mActivedAnimationStateList = new List<string>();
		mAllAnimationStateList = new List<string>();
	}
	public void Update()
	{
		updateInfo();
	}
	private void OnDisable()
	{
		updateInfo(false);
	}
	public void setPlayer(CharacterOther player)
	{
		mPlayer = player;
		mCharacterData = mPlayer.getCharacterData();
		mAnimation = mPlayer.getAnimation();
	}
	//-------------------------------------------------------------------------------------------------------
	protected void updateInfo(bool enable = true)
	{
		if(mPlayer == null)
		{
			return;
		}
		mState = new List<PLAYER_STATE>(mPlayer.getStateMachine().getStateList().Keys);
		mActiveComponentList.Clear();
		Dictionary<string, GameComponent> allComponent = mPlayer.getAllComponent();
		foreach (var item in allComponent)
		{
			if (item.Value.isActive())
			{
				mActiveComponentList.Add(item.Value.getType().ToString());
			}
		}
		mCenterOfMass = mPlayer.getRigidBody().centerOfMass;
		mSpeed = mCharacterData.mSpeed;
		mCircle = mCharacterData.mCircle;
		mCurWayPoint = mCharacterData.mCurWayPoint;
		mRunDistance = mCharacterData.mRunDistance;
		mTotalDistance = mCharacterData.mTotalDistance;
		mProcessKey = mPlayer.getProcessKey();
		mProcessTurn = mPlayer.getProcessTurn();
		mProcessExternalSpeed = mPlayer.getProcessExternalSpeed();
		CharacterSpeedHardware hardwareSpeed = mPlayer.getFirstActiveComponent<CharacterSpeedHardware>();
		if (hardwareSpeed != null)
		{
			mTargetSpeed = hardwareSpeed.getTargetSpeed();
		}
		mActivedAnimationStateList.Clear();
		mAllAnimationStateList.Clear();
		if(enable)
		{
			foreach (var item in mAnimation)
			{
				AnimationState state = item as AnimationState;
				string info = "|" + state.name + "| |";
				info += (state.enabled ? "enabled" : "disabled") + "| |";
				info += "len:" + StringUtility.floatToString(state.length, 2) + "| |";
				info += "spd:" + StringUtility.floatToString(state.speed, 2) + "| |";
				info += "time:" + StringUtility.floatToString(state.time, 2) + "| |";
				info += "weight:" + StringUtility.floatToString(state.weight, 2) + "| |";
				info += state.wrapMode + "|";
				mAllAnimationStateList.Add(info);
				if (state.enabled)
				{
					mActivedAnimationStateList.Add(info);
				}
			}
		}
	}
}