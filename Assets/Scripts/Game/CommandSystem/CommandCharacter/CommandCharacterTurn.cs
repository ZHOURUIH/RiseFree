using UnityEngine;
using System.Collections;

public class CommandCharacterTurn : Command
{
	public float mAngle;
	public override void init()
	{
		base.init();
	}
	public override void execute()
	{
		Character character = mReceiver as Character;
		Animation animation = character.getAnimation();
		if(Mathf.Abs(mAngle) >= GameDefine.TURN_ANGLE && Mathf.Abs(mAngle) < GameDefine.TURN_SHARP_ANGLE)
		{
			animation.CrossFade(mAngle < 0.0f ? GameDefine.ANIM_TURN_LEFT : GameDefine.ANIM_TURN_RIGHT);
		}
		else if(Mathf.Abs(mAngle) >= GameDefine.TURN_SHARP_ANGLE)
		{
			animation.CrossFade(mAngle < 0.0f ? GameDefine.ANIM_TURN_LEFT_SHARP : GameDefine.ANIM_TURN_RIGHT_SHARP);
		}
		animation.CrossFadeQueued(GameDefine.ANIM_RIDE);
	}
}
