using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ItemBoxObject : MonoBehaviour
{
	protected SceneItemBox mItemBox;
	protected bool mEffective = true;
	public void Start()
	{
		gameObject.GetComponent<BoxCollider>().isTrigger = true;
	}
	public void OnTriggerEnter(Collider other)
	{
		if (mEffective && LayerMask.NameToLayer(GameDefine.LAYER_CHARACTER) == other.gameObject.layer)
		{
			Character character = GameBase.mCharacterManager.getCharacter(other.name);
			mItemBox.onEffective(character);
			mEffective = false;
		}
	}
	public void setItem(SceneItemBox item)
	{
		mItemBox = item;
		mEffective = true;
	}
}