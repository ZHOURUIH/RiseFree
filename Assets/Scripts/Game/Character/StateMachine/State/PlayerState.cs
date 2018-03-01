using UnityEngine;
using System.Collections;

public class PlayerState : GameBase
{
	protected PLAYER_STATE mType;
	protected CharacterOther mPlayer;
	protected bool mActive;
	protected Animation mAnimation;
	protected float mStateTime;				// 该状态持续的时间,小于0表示无限制
	protected EnableStatus mEnableStatus;	// 该状态可使用的输入方式,按键,外部速度,转向
	public PlayerState(PLAYER_STATE type)
	{
		mType = type;
		mActive = true;
		mStateTime = -1.0f;
		mEnableStatus = new EnableStatus();
		mEnableStatus.enableAll(true);
	}
	public void setPlayer(CharacterOther player)
	{
		mPlayer = player;
		mAnimation = mPlayer.getAnimation();
	}
	// 当前是否可以进入该状态,状态的是否重复已经由状态机判断了
	public virtual bool canEnter(){return true;}
	public virtual void enter(){}
	public virtual void update(float elapsedTime)
	{
		if(mStateTime >= 0.0f)
		{
			mStateTime -= elapsedTime;
			if(mStateTime <= 0.0f)
			{
				mStateTime = -1.0f;
				CommandCharacterRemoveState cmd = newCmd(out cmd);
				cmd.mState = mType;
				pushCommand(cmd, mPlayer);
			}
		}
	}
	public virtual void fixedUpdate(float elapsedTime)
	{
		;
	}
	// 返回值表示退出状态是否成功
	public virtual void leave(){}
	public virtual void keyProcess(float elapsedTime){}
	public void setActive(bool active) { mActive = active; }
	public bool getActive() { return mActive; }
	public PLAYER_STATE getStateType() { return mType; }
	public void setStateTime(float time) { mStateTime = time; }
	public float getStateTime() { return mStateTime; }
	public EnableStatus getEnableStatus() { return mEnableStatus; }
}
