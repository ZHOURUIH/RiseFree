using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnableStatus
{
	public void enableAll(bool enable)
	{
		mProcessKey = enable;
		mProcessTurn = enable;
		mProcessExternalSpeed = enable;
	}
	public void and(EnableStatus that)
	{
		mProcessKey = mProcessKey && that.mProcessKey;
		mProcessTurn = mProcessTurn && that.mProcessTurn;
		mProcessExternalSpeed = mProcessExternalSpeed && that.mProcessExternalSpeed;
	}
	public bool mProcessKey;
	public bool mProcessTurn;
	public bool mProcessExternalSpeed;
}

public class CharacterOther : Character
{
	protected RibbonTrailStatic mRibbonTrailStatic;
	protected RibbonTrailDynamic mRibbonTrailDynamic;
	protected PlayerPack mPlayerPack;
	protected EnableStatus mEnableStatus;
	protected StateMachine mStateMachine;
	protected float mCurTimeCount;
	protected const float TIME_INTERVAL = 1.0f;
	protected float mLastMileage;
	protected List<string> mSelectAnimation;
	protected List<SceneMissile> mMissileLockList;	// 锁定玩家的导弹列表
	public CharacterOther(CHARACTER_TYPE type, string name)
		:
		base(type, name)
	{
		mPlayerPack = new PlayerPack();
		mRibbonTrailStatic = new RibbonTrailStatic();
		mRibbonTrailDynamic = new RibbonTrailDynamic();
		mStateMachine = new StateMachine();
		mEnableStatus = new EnableStatus();
		mSelectAnimation = new List<string>();
		mMissileLockList = new List<SceneMissile>();
	}
	public override void init()
	{
		base.init();
		mPlayerPack.init(this);
		mStateMachine.init(this);
		mEnableStatus.enableAll(false);
	}
	public override void destroy()
	{
		mPlayerPack.destroy();
		destroyRibbonTrailStatic();
		destroyRibbonTrailDynamic();
		base.destroy();
	}
	public override void initComponents()
	{
		base.initComponents();
		addComponent<CharacterBikePhysics>("BikePhysics");
		addComponent<CharacterTrackMileage>("TrackMileage");
		addComponent<CharacterSpeedHardware>("SpeedHardware");
	}
	public override void update(float elapsedTime)
	{
		base.update(elapsedTime);
		mStateMachine.update(elapsedTime);
		// 随时更新运动方向
		if (mRibbonTrailStatic != null)
		{
			mRibbonTrailStatic.update(elapsedTime);
		}
		if (mRibbonTrailDynamic != null)
		{
			mRibbonTrailDynamic.update(elapsedTime);
		}
		// 每隔一段时间计算一次卡路里，然后进行累加
		mCurTimeCount += elapsedTime;
		if (mCurTimeCount >= TIME_INTERVAL)
		{
			float averageSpeed = (mCharacterData.mTotalDistance - mLastMileage) / mCurTimeCount;
			mCharacterData.mKcal += 65.0f * GameUtility.generateSpeedCoe(averageSpeed) * (mCurTimeCount / 3600.0f);
			mLastMileage = mCharacterData.mTotalDistance;
			// 记录平均速度
			mCharacterData.mAverageSpeed = averageSpeed;
			mCurTimeCount = 0.0f;
		}
	}
	public override void fixedUpdate(float elapsedTime)
	{
		base.fixedUpdate(elapsedTime);
		mStateMachine.fixedUpdate(elapsedTime);
	}
	public override void initModel(string modelPath, string animationControllerPath = "")
	{
		base.initModel(modelPath, animationControllerPath);
#if UNITY_EDITOR
		mObject.AddComponent<PlayerInfo>();
#endif
		mObject.AddComponent<PlayerPhysics>();
		getFirstComponent<CharacterBikePhysics>().notifyModelInited();
		Color color = GameUtility.getTrailColorByModelName(StringUtility.getFileName(modelPath));
		initDynamicTrail(color);
		initStaticTrail(color);
		activeTrail(false, false);
	}
	public void activeTrail(bool staticTrail, bool active = true)
	{
		// 隐藏时只能都隐藏
		// 显示时只能显示一个
		mRibbonTrailStatic.setActive(active && staticTrail);
		mRibbonTrailDynamic.setActive(active && !staticTrail);
	}
	public RibbonTrailDynamic getRibbonTrailDynamic() { return mRibbonTrailDynamic; }
	public RibbonTrailStatic getRibbonTrailStatic(){return mRibbonTrailStatic; }
	public PlayerPack getPlayerPack() { return mPlayerPack; }
	public void notifyStateChanged()
	{
		mEnableStatus.enableAll(true);
		Dictionary<PLAYER_STATE, PlayerState>  stateList = mStateMachine.getStateList();
		foreach(var item in stateList)
		{
			mEnableStatus.and(item.Value.getEnableStatus());
		}
	}
	public bool isLockedByMissile() { return mMissileLockList.Count > 0; }
	public void notifyMissileLocked(SceneMissile missile, bool locked)
	{
		if(locked)
		{
			if(!mMissileLockList.Contains(missile))
			{
				mMissileLockList.Add(missile);
			}
		}
		else
		{
			mMissileLockList.Remove(missile);
		}
	}
	public List<string> getSelectAnimation() { return mSelectAnimation; }
	public bool getProcessKey() { return mEnableStatus.mProcessKey; }
	public bool getProcessTurn(){ return mEnableStatus.mProcessTurn; }
	public bool getProcessExternalSpeed(){ return mEnableStatus.mProcessExternalSpeed; }
	public StateMachine getStateMachine() { return mStateMachine; }
	public bool hasState(PLAYER_STATE state) { return mStateMachine.hasState(state); }
	public bool hasStateGroup(STATE_GROUP group) { return mStateMachine.hasStateGroup(group); }
	public override bool isType(CHARACTER_TYPE type) { return type == CHARACTER_TYPE.CT_OTHER || base.isType(type); }
	//---------------------------------------------------------------------------------------------------------------------------------
	protected void initStaticTrail(Color color)
	{
		mRibbonTrailStatic.init(UnityUtility.getGameObject(mObject, "TrailStatic", true));
		mRibbonTrailStatic.setTrialLifeTime(1.0f);
		mRibbonTrailStatic.setSpeed(15.0f);
		mRibbonTrailStatic.setTrailColor(color);
	}
	protected void initDynamicTrail(Color color)
	{
		mRibbonTrailDynamic.init(UnityUtility.getGameObject(mObject, "TrailDynamic", true));
		mRibbonTrailDynamic.setTrialLifeTime(1.0f);
		mRibbonTrailDynamic.setEndPointTransform(UnityUtility.getGameObject(mObject, "Bip001 Pelvis", true).transform);
		mRibbonTrailDynamic.setTrailColor(color);
	}
	protected void destroyRibbonTrailDynamic()
	{
		if (mRibbonTrailDynamic != null)
		{
			mRibbonTrailDynamic.destroy();
			mRibbonTrailDynamic = null;
		}
	}
	protected void destroyRibbonTrailStatic()
	{
		if (mRibbonTrailStatic != null)
		{
			mRibbonTrailStatic.destroy();
			mRibbonTrailStatic = null;
		}
	}
}