using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

// 角色里程
public class CharacterTrackMileage : GameComponent
{
	protected CharacterOther mCharacter;
	protected CharacterData mData;
	protected List<int> mReverseindex;
	protected List<int> mPassedPointList;           // 经过的点
	protected float mWrongDirectionTime;
	public CharacterTrackMileage(Type type, string name)
		: base(type, name)
	{
		mReverseindex = new List<int>();
		mPassedPointList = new List<int>();
		mWrongDirectionTime = 0.0f;
	}
	public override void init(ComponentOwner owner)
	{
		base.init(owner);
		mCharacter = owner as CharacterOther;
		mData = mCharacter.getCharacterData();
	}
	public override void update(float elapsedTime)
	{
		if (mData.mSpeed > 0.0f)
		{
			// 判断当前方向是否合法
			bool wrongDirection = directionCheck();
			// 圈数检测,更新当前移动距离,反向检测,完成比赛检测	
			if (!wrongDirection)
			{
				int curPointIndex = mData.mCurWayPoint;
				float runDistance = mWayPointManager.getRunDistance(mCharacter.getPosition(), ref curPointIndex, curPointIndex);
				if (!mPassedPointList.Contains(curPointIndex))
				{
					mPassedPointList.Add(curPointIndex);
				}
				// 防止在游戏开始后 后退到终点线之前 导致已过路点添加错误 最后圈数检测错误
				if (runDistance >= mWayPointManager.getTotalLength()/2 && mPassedPointList.Count <= mWayPointManager.getPointCount() / 3)
				{
					mPassedPointList.Clear();
				}
				// 已经进入下一圈,并且上一次经过的是当前圈的最后一个路段,当前正处于第0个路段,并且已经完成了当前圈一半以上的路段
				if (runDistance > 0.0f &&
					curPointIndex == 0 && mPassedPointList[mPassedPointList.Count - 1] == mWayPointManager.getPointCount() - 1 && 
					mPassedPointList.Count >= mWayPointManager.getPointCount() / 2)
				{
					// 圈数改变
					mPassedPointList.Clear();
					CommandCharacterCircleChanged cmdCircle = newCmd(out cmdCircle);
					cmdCircle.mCircle = mData.mCircle + 1;
					pushCommand(cmdCircle, mCharacter);
				}
				// 如果已经跑完比赛了,则将距离和路点清零
				if (mCharacter.hasState(PLAYER_STATE.PS_FINISH))
				{
					curPointIndex = 0;
					runDistance = 0.0f;
				}
				// 里程改变,必须在更新圈数后才能更新里程,因为计算里程会使用圈数
				CommandCharacterDistanceChanged cmdDistance = newCmd(out cmdDistance, false);
				cmdDistance.mWayPoint = curPointIndex;
				cmdDistance.mDistance = runDistance;
				pushCommand(cmdDistance, mCharacter);
				mWrongDirectionTime = 0.0f;
				//方向正确后退出游戏中的错误方向状态
				if (mCharacter.hasState(PLAYER_STATE.PT_WRONG_DIRECTION))
				{
					CommandCharacterRemoveState cmdRemoveState = newCmd(out cmdRemoveState);
					cmdRemoveState.mState = PLAYER_STATE.PT_WRONG_DIRECTION;
					pushCommand(cmdRemoveState, mCharacter);
				}
			}
			else
			{
				if(!mCharacter.hasState(PLAYER_STATE.PT_WRONG_DIRECTION))
				{
					mWrongDirectionTime += elapsedTime;
					if (mWrongDirectionTime >= GameDefine.WRONG_DIRECTION_TIPS_TIME)
					{
						CommandCharacterAddState cmdState = newCmd(out cmdState);
						cmdState.mState = PLAYER_STATE.PT_WRONG_DIRECTION;
						pushCommand(cmdState, mCharacter);
						mWrongDirectionTime = 0.0f;
					}
				}
			}
		}
		base.update(elapsedTime);
	}
	//-----------------------------------------------------------------------------------------------------------------------------------
	protected bool directionCheck()
	{
		int pointIndex = mData.mCurWayPoint;
		MathUtility.clamp(ref pointIndex, 0, mWayPointManager.getPointCount());
		Vector3 firstPoint = mWayPointManager.getPoint(pointIndex);
		Vector3 nextPoint = mWayPointManager.getPoint((pointIndex + 1) % mWayPointManager.getPointCount());
		Vector3 wayDirection = MathUtility.normalize(nextPoint - firstPoint);
		Vector3 speedDirection = MathUtility.getVectorFromAngle(mData.mSpeedRotation.y * Mathf.Deg2Rad);
		float angle = MathUtility.getAngleBetweenVector(wayDirection, speedDirection);
		return angle > Mathf.PI / 2.0f;
	}
	protected override void setBaseType()
	{
		mBaseType = typeof(CharacterTrackMileage);
	}
	protected override bool isType(Type type)
	{
		return base.isType(type) || type == typeof(CharacterTrackMileage);
	}
}