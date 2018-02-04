using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class SelecteState : PlayerState
{
	public SelecteState(PLAYER_STATE type)
		:base(type)
	{
		mEnableStatus.enableAll(false);
	}
}