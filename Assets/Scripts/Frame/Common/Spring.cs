﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Spring
{
	protected float mNormalLength;
	protected float mCurLength;
	protected float mSpringK;
	protected float mObjectMass;
	protected float mMinLength;
	protected float mForce;// 力和速度 只有正负没有方向,正的是沿着拉伸弹簧的方向,负值压缩弹簧的方向
	protected float mObjectSpeed;
	protected float mPreAcce;
	public Spring()
	{
		mNormalLength = 0.0f;
		mCurLength = 0.0f;
		mSpringK = 1.0f;
		mObjectMass = 1.0f;
		mForce = 0.0f;
		mObjectSpeed = 0.0f;
		mMinLength = 0.5f;
	}
	public void update(float fElaspedTime)
	{
		// 计算拉力
		float elasticForce = calculateElasticForce() * -1.0f;

		// 加速度
		float acceleration = (mForce + elasticForce) / mObjectMass;
		if (MathUtility.isFloatZero(acceleration) || (acceleration < 0.0f && mPreAcce > 0.0f) || (acceleration > 0.0f && mPreAcce < 0.0f))
		{
			mObjectSpeed = 0.0f;
			acceleration = 0.0f;
		}
		else
		{
			// 速度
			mObjectSpeed += acceleration * fElaspedTime;
		}

		// 长度
		mCurLength += mObjectSpeed * fElaspedTime;
		if (mCurLength <= mMinLength)
		{
			mCurLength = mMinLength;
			mObjectSpeed = 0.0f;
			acceleration = 0.0f;
		}
		mPreAcce = acceleration;
	}
	// 计算拉力 如果为正则是压缩弹簧的方向,为负拉伸弹簧的方向
	public float calculateElasticForce()
	{
		return (mCurLength - mNormalLength) * mSpringK;
	}
	public void setNormaLength(float length) { mNormalLength = length; }
	public void setMass(float mass) { mObjectMass = mass; }
	public void setSpringk(float k) { mSpringK = k; }
	public void setSpeed(float speed) { mObjectSpeed = speed; }
	public void setForce(float force) { mForce = force; }
	public void setCurLength(float length) { mCurLength = length; }
	public float getSpeed() { return mObjectSpeed; }
	public float getLength() { return mCurLength; }
	public float getNomalLength() { return mNormalLength; }
};