using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class PlayerPhysics : MonoBehaviour
{
	public CharacterOther mPlayer;
	protected int mJumpPointLayer;
	public void Awake()
	{
		mPlayer = GameBase.mCharacterManager.getCharacter(gameObject.name) as CharacterOther;
		mJumpPointLayer = LayerMask.NameToLayer(GameDefine.LAYER_JUMP_POINT);
	}
	public void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.layer == mJumpPointLayer && mPlayer.hasState(PLAYER_STATE.PS_GAMING))
		{
			CommandCharacterJump cmdJump = GameBase.newCmd(out cmdJump);
			cmdJump.mJumpSpeed = GameDefine.JUMP_SPEED;
			GameBase.pushCommand(cmdJump, mPlayer);
		}
	}
	//-------------------------------------------------------------------------------------------------------
}