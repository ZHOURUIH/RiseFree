﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

public class ThreadLock
{
	protected int mLockCount = 0;         // 是否锁定
	protected bool mTraceStack = false;
	protected string mFileName;
	protected int mLine;
	public ThreadLock()
	{
		mLockCount = 0;
		mTraceStack = false;
	}
	public void setTrackStack(bool trace)
	{
		mTraceStack = trace;
	}
	public void waitForUnlock()
	{
		while (Interlocked.Exchange(ref mLockCount, 1) != 0){}
		if(mTraceStack)
		{
			mFileName = UnityUtility.getCurSourceFileName(2);
			mLine = UnityUtility.getLineNum(2);
		}
	}
	public void unlock()
	{
		Interlocked.Exchange(ref mLockCount, 0);
		if (mTraceStack)
		{
			mFileName = "";
			mLine = 0;
		}
	}
};