﻿using UnityEngine;
using System.Collections;

public class CharacterRegister : GameBase
{
	public void registeAllCharacter()
	{
		registeCharacter<Character>(CHARACTER_TYPE.CT_NORMAL);
		registeCharacter<CharacterOther>(CHARACTER_TYPE.CT_OTHER);
		registeCharacter<CharacterAI>(CHARACTER_TYPE.CT_AI);
		registeCharacter<CharacterMyself>(CHARACTER_TYPE.CT_MYSELF);
	}
	public void registeCharacter<T>(CHARACTER_TYPE type) where T : Character
	{
		mCharacterManager.registeCharacter<T>(type);
	}
}