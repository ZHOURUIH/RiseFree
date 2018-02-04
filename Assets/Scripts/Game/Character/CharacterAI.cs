using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterAI : CharacterOther
{
	public CharacterAI(CHARACTER_TYPE type, string name)
		:
		base(type, name)
	{}
	public override void initComponents()
	{
		base.initComponents();
		addComponent<CharacterControllerAI>("ControllerAI");
	}
	public override void update(float elapsedTime)
	{
		base.update(elapsedTime);
	}
	public override bool isType(CHARACTER_TYPE type) { return type == CHARACTER_TYPE.CT_AI || base.isType(type); }
}