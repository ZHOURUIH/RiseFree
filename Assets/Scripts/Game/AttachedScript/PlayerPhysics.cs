using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class PlayerPhysics : MonoBehaviour
{
	public CharacterOther mPlayer;
	public void Awake()
	{
		mPlayer = GameBase.mCharacterManager.getCharacter(gameObject.name) as CharacterOther;
	}
	public void OnTriggerEnter(Collider other)
	{
		// 角色之间的碰撞
		if (other.gameObject.layer == GameUtility.mCharacterLayer)
		{
			CharacterOther otherPlayer = GameBase.mCharacterManager.getCharacter(other.name) as CharacterOther;
			GameBase.mRaceSystem.addCollidePair(mPlayer, otherPlayer);
		}
		// 接触起跳点
		else if(other.gameObject.layer == GameUtility.mJumpPointLayer && mPlayer.hasState(PLAYER_STATE.PS_GAMING))
		{
			CommandCharacterJump cmdJump = GameBase.newCmd(out cmdJump);
			cmdJump.mJumpSpeed = GameDefine.JUMP_SPEED;
			GameBase.pushCommand(cmdJump, mPlayer);
		}
	}
	//-------------------------------------------------------------------------------------------------------
}