using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class CommandResetCharacterPositionAndDirection :Command
{	
	public override void init()
	{
		base.init();
	}
	public override void execute()
	{
		Character character  = mReceiver as Character;
		CharacterData data = character.getCharacterData();
		int pointIndex = mWayPointManager.getPointIndexFromDistance(data.mRunDistance , data.mCurWayPoint);
		float direction = mWayPointManager.getPointDirection(pointIndex);
		data.mSpeedRotation.y = direction;
		Vector3 point = mWayPointManager.getPoint(pointIndex);
		character.setWorldPosition(point);
		CharacterBikePhysics bikePhysics = character.getFirstComponent<CharacterBikePhysics>();
		bikePhysics.correctTransform();
	}
	public override string showDebugInfo()
	{
		return this.GetType().ToString();
	}
}

