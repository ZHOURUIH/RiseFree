using UnityEngine;
using System.Collections;

public class CharacterData : CharacterBaseData
{
	public CharacterData()
	{
		mSpeed = 0.0f;
		mVerticalSpeed = 0.0f;
		mLastSpeed = 0.0f;
		mTurnAngle = 0.0f;
		mStartIndex = 0;
		mNumber = -1;
		mRunDistance = 0.0f;
		mCircle = 0;
		mCurWayPoint = -1;
		mTotalDistance = 0.0f;
		mSpeedRotation = Vector3.zero;
		mKcal = 0.0f;
		mAverageSpeed = 0.0f;
		mMaxSpeed = 0.0f;
		mRank = 0;
		mRunTime = 0.0f;
		mTurnSensitive = 1.0f;
	}
	public float mSpeed;			// 水平方向上的速度
	public float mVerticalSpeed;	// 竖直方向上的速度
	public float mLastSpeed;        // 上一帧的速度
	public Vector3 mSpeedRotation;	// 速度的旋转值,记录了速度的方向,角色的旋转值会逐渐与该值靠近
	public float mTurnAngle;        // 当前转向轴的角度
	public int   mStartIndex;		// 玩家起点编号,用于寻找起点位置
	public int   mNumber;			// 玩家编号,玩家自己的编号为-1,AI编号为大于等于0
	public float mRunDistance;		// 当前到起点的距离
	public int	 mCircle;			// 当前已经完成的圈数
	public int	 mCurWayPoint;		// 当前所在路段的下标
	public float mTotalDistance;	// 累计总里程
	public float mKcal;				// 卡路里
	public float mAverageSpeed;		// 平均速度
	public float mMaxSpeed;			// 最大速度
	public int	 mRank;				// 名次
	public float mRunTime;          // 骑行时间
	public float mTurnSensitive;	// 转向灵敏度
}
