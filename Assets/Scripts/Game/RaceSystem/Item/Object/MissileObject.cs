using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
public class MissileObject : MonoBehaviour
{
	protected SceneMissile mMissileObject;
	protected bool mEffentive = true;
	public void Start()
	{
		gameObject.GetComponent<BoxCollider>().isTrigger = true;
	}
	public void OnTriggerEnter(Collider other)
	{
		if (mEffentive && LayerMask.NameToLayer(GameDefine.LAYER_CHARACTER) == other.gameObject.layer)
		{
			Character character = GameBase.mCharacterManager.getCharacter(other.name);
			mMissileObject.onEffective(character);
			mEffentive = false;
		}
		
	}
	public void setItem(SceneMissile item) { mMissileObject = item; }
}