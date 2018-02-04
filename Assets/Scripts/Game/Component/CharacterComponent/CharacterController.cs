using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

// 自由角色控制器
public class CharacterController : GameComponent
{
	public CharacterController(Type type, string name)
		:base(type, name)
	{
		;
	}
	//-----------------------------------------------------------------------------------------------------------------------------------
	protected override void setBaseType()
	{
		mBaseType = typeof(CharacterController);
	}
	protected override bool isType(Type type)
	{
		return base.isType(type) || type == typeof(CharacterController);
	}
}