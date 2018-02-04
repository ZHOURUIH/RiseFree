using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
public class LandmineObject : MonoBehaviour
{
	protected SceneLandMine mSceneLandMine;
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
			mSceneLandMine.onEffective(character);
			mEffentive = false;
		}
		
	}
	public void setItem(SceneLandMine item) { mSceneLandMine = item; }
}