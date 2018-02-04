using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 当物体移动时产生的拖尾,适用于移动的物体
public class RibbonTrailDynamic : RibbonTrail
{
	protected Transform mEndPointTransform;
	public override void update(float elapsedTime)
	{
		if(mObject == null || !mObject.activeSelf)
		{
			return;
		}
		// 如果最新的一个点距离当前已经超过了最小距离,则添加一个点
		Transform transform = mObject.transform;
		addSection(transform.position, mEndPointTransform.position);
		base.update(elapsedTime);
	}
	public void setEndPointTransform(Transform endPoint)
	{
		mEndPointTransform = endPoint;
	}
}