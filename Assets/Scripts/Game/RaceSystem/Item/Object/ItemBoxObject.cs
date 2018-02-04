using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ItemBoxObject : MonoBehaviour
{
	protected SceneItemBox mItemBox;
	protected bool mEffentive = true;
	public void Start()
	{
		gameObject.GetComponent<BoxCollider>().isTrigger = true;
	}
	public void OnTriggerEnter(Collider other)
	{
		if (mEffentive)
		{
			Character character = GameBase.mCharacterManager.getCharacter(other.name);
			mItemBox.onEffective(character);
			mEffentive = false;
		}	
	}
	public void setItem(SceneItemBox item) { mItemBox = item; }
}