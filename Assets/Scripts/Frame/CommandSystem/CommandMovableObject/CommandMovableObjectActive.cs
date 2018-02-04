﻿using UnityEngine;
using System.Collections;

public class CommandMovableObjectActive : Command
{
	public bool mActive;
	public override void init()
	{
		base.init();
		mActive = true;
	}
	public override void execute()
	{
		MovableObject obj = (mReceiver) as MovableObject;
		obj.setActive(mActive);
	}
	public override string showDebugInfo()
	{
		return this.GetType().ToString() + " : active : " + mActive;
	}
}