using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

// 复位角色
public class CommandCharacterReset :Command
{	
	public override void init()
	{
		base.init();
	}
	public override void execute()
	{
		Character character  = mReceiver as Character;
		CharacterData data = character.getCharacterData();
		// 查找当前所在路点,将角色重置到当前路段的起点处
		int pointIndex = mWayPointManager.getPointIndexFromDistance(data.mRunDistance, data.mCurWayPoint);
		data.mSpeedRotation.y = mWayPointManager.getPointDirection(pointIndex);
		character.setWorldPosition(mWayPointManager.getPoint(pointIndex));
		character.getFirstComponent<CharacterBikePhysics>().correctTransform();
	}
	public override string showDebugInfo()
	{
		return base.showDebugInfo();
	}
}

