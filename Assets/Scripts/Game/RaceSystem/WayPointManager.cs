using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using UnityEngine;

public class WayPoint
{
	public Vector3 mPoint;			// 当前路点坐标
	public float mDisToLastPoint;	// 到上一个路点的距离
	public float mDisToNextPoint;	// 到下一个路点的距离
	public float mDisToStart;		// 到起点的距离
}
public class WayPointManager : FrameComponent
{
	protected List<WayPoint> mPointList;
	protected float			 mTotalLength;		// 总距离
	public WayPointManager(string name)
		:
		base(name)
	{
		mPointList = new List<WayPoint>();
	}
	public override void init()
	{
		;
	}
	public override void destroy()
	{
		clear();
		base.destroy();
	}
	public void clear()
	{
		mPointList.Clear();
		mTotalLength = 0.0f;
	}
	public override void update(float elapsedTime)
	{
		;
	}
	public void initPoints(GameObject pointRoot)
	{
		mPointList.Clear();
		int count = pointRoot.transform.childCount;
		for (int i = 0; i < count; ++i)
		{
			Transform obj = pointRoot.transform.GetChild(i);
			WayPoint wayPoint = new WayPoint();
			wayPoint.mPoint = obj.position;
			wayPoint.mDisToLastPoint = 0.0f;
			wayPoint.mDisToStart = 0.0f;
			wayPoint.mDisToNextPoint = 0.0f;
			mPointList.Add(wayPoint);
		}
		// 计算路点距离,从下标1开始,因为第0个距离为0
		count = mPointList.Count;
		for (int i = 0; i < count; ++i)
		{
			int lastIndex = i > 0 ? i - 1 : 0;
			int nextIndex = (i + 1) % count;
			mPointList[i].mDisToLastPoint = MathUtility.getLength(mPointList[i].mPoint - mPointList[lastIndex].mPoint);
			mPointList[i].mDisToNextPoint = MathUtility.getLength(mPointList[i].mPoint - mPointList[nextIndex].mPoint);
			mPointList[i].mDisToStart = mPointList[i].mDisToLastPoint + mPointList[lastIndex].mDisToStart;
		}
		// 计算赛道总长度
		mTotalLength = mPointList[count - 1].mDisToNextPoint + mPointList[count - 1].mDisToStart;
	}
	public Vector3 getPoint(int index)
	{
		if (index < 0 || index >= mPointList.Count)
		{
			UnityUtility.logError("can not find point at index : " + index);
			return Vector3.zero;
		}
		return mPointList[index].mPoint;
	}
	// 根据一个点,计算出对应的到起点的距离
	public float getRunDistance(Vector3 pos, ref int curIndex, int lastPointIndex = -1)
	{
		Vector3 lastPoint = Vector3.zero;
		Vector3 nextPoint = Vector3.zero;
		float curDis = 0.0f;
		// 如果当前点投影在路段内,则计算在当前路段中走过的距离
		// 投影超出了路段范围,则当前路段中走过的距离为0
		if (getPointBetween(pos, ref lastPoint, ref nextPoint, ref curIndex, lastPointIndex))
		{
			Vector3 posOnLine = MathUtility.getProjectPoint(pos, lastPoint, nextPoint);
			curDis = MathUtility.getLength(posOnLine - lastPoint);
		}
		float totalDis = curDis + mPointList[curIndex].mDisToStart;
		if (totalDis > mTotalLength)
		{
			UnityUtility.logError("totalDis can not over mTotalLength, totalDis : " + totalDis + ", mTotalLength : " + mTotalLength);
		}
		return totalDis;
	}
	// 得到路段的朝向
	public float getPointDirection(int pointIndex)
	{
		int count = mPointList.Count;
		Vector3 curPoint = mPointList[pointIndex].mPoint;
		Vector3 nextPoint = mPointList[(pointIndex + 1) % count].mPoint;
		return MathUtility.getAngleFromVector(nextPoint - curPoint) * Mathf.Rad2Deg;
	}
	// 得到路线上distance所在路段的下标,表示distance的所在路段的下标,用于提高计算效率
	public int getPointIndexFromDistance(float distance, int lastPointIndex = -1)
	{
		// distance不能超过一圈的长度
		while(distance >= mTotalLength)
		{
			distance -= mTotalLength;
		}
		int startIndex = lastPointIndex;
		if (lastPointIndex >= 0)
		{
			// 如果传入的距离还没有到传入的上一次下标的点到起点的距离,则从头开始查找
			if(mPointList[startIndex].mDisToStart >= distance)
			{
				startIndex = 0;
			}
		}
		else
		{
			startIndex = 0;
		}
		int pointIndex = 0;
		int count = mPointList.Count;
		for (int i = startIndex; i < count; ++i)
		{
			if(mPointList[i].mDisToStart <= distance && mPointList[(i + 1) % count].mDisToStart >= distance)
			{
				pointIndex = i;
				break;
			}
		}
		return pointIndex;
	}
	//--------------------------------------------------------------------------------------------------------------------------------------------------
	// 根据一个点计算出对应路段的两个点和第一个点的下标
	protected bool getPointBetween(Vector3 pos, ref Vector3 lastPoint, ref Vector3 nextPoint, ref int curIndex, int lastPointIndex = -1)
	{
		int startIndex = 0;
		int count = 0;
		int pointCount = mPointList.Count;
		// 如果上一次已经查找过了,则从上次的地方继续查找,并且只查找最近的5个点
		if (lastPointIndex >= 0)
		{
			startIndex = lastPointIndex;
			count = Mathf.Min(mPointList.Count, 5);
		}
		else
		{
			count = pointCount;
		}
		float minDis = -1.0f;
		int minDisIndex = 0;
		for (int i = 0; i < count; ++i)
		{
			float curDis = MathUtility.getLength(pos - mPointList[(i + startIndex) % pointCount].mPoint);
			if (minDis < 0.0f || minDis > curDis)
			{
				minDis = curDis;
				minDisIndex = (i + startIndex) % pointCount;
			}
		}
		Vector3 point0 = mPointList[(minDisIndex - 1 + pointCount) % pointCount].mPoint;
		Vector3 point1 = mPointList[minDisIndex].mPoint;
		Vector3 point2 = mPointList[(minDisIndex + 1) % pointCount].mPoint;
		// 判断在哪段路上面根据位置到路点的连线与路段的夹角决定,与哪段路段的夹角小就在哪条路段上
		Vector3 line = pos - point1;
		float angle0 = MathUtility.getAngleBetweenVector(point0 - point1, line);
		float angle1 = MathUtility.getAngleBetweenVector(point2 - point1, line);
		// 在前一段路上
		if (angle0 < angle1)
		{
			lastPoint = point0;
			nextPoint = point1;
			curIndex = (minDisIndex - 1 + pointCount) % pointCount;
		}
		// 后一段路上
		else
		{
			lastPoint = point1;
			nextPoint = point2;
			curIndex = minDisIndex;
		}
		// 返回投影点是否在路段上
		return MathUtility.isPointProjectOnLine(pos, lastPoint, nextPoint);
	}
	//--------------------------------------------------------------------------------------------------------------------------------------------------
	// 获取赛道一圈的总距离
	public float getTotalLength()
	{
		return mTotalLength;
	}
	// 获取现在检查点的数量
	public int getPointCount()
	{
		return mPointList.Count;
	}
}