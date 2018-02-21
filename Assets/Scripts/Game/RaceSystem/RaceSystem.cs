using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using UnityEngine;

public class TrackInfo
{
	public string mName;
	public int mCircleCount;
	public int mDifficultyStart;
	public TrackInfo(string name, int circle, int star)
	{
		mName = name;
		mCircleCount = circle;
		mDifficultyStart = star;
	}
}

public class CollidePair
{
	public CharacterOther mPlayer0;
	public CharacterOther mPlayer1;
	public CollidePair(CharacterOther player0, CharacterOther player1)
	{
		mPlayer0 = player0;
		mPlayer1 = player1;
	}
	public bool isPair(CharacterOther player0, CharacterOther player1)
	{
		return (mPlayer0 == player0 && mPlayer1 == player1) || (mPlayer0 == player1 && mPlayer1 == player0);
	}
}

public class RaceSystem : FrameComponent
{
	protected float mSystemTime;
	protected int mTrackIndex;
	protected List<TrackInfo> mTrackInfoList;
	protected List<CollidePair> mCollidePairList;
	public RaceSystem(string name)
		:
		base(name)
	{
		mTrackInfoList = new List<TrackInfo>();
		mSystemTime = 0.0f;
		mCollidePairList = new List<CollidePair>();
	}
	public override void init()
	{
		mTrackInfoList.Add(new TrackInfo(GameDefine.PRIMARY_TRACK, 3, 1));
		mTrackInfoList.Add(new TrackInfo(GameDefine.SNOW_MOUNTAIN, 3, 2));
		mTrackInfoList.Add(new TrackInfo(GameDefine.DESERT, 3, 3));
		mTrackInfoList.Add(new TrackInfo(GameDefine.ANCIENT_CITY, 2, 4));
	}
	public override void destroy()
	{
		base.destroy();
	}
	public override void update(float elapsedTime)
	{
		if (mGameSceneManager.getCurScene().atProcedure(PROCEDURE_TYPE.PT_MAIN_GAMING))
		{
			// 系统计时
			int oldTime = (int)mSystemTime;
			mSystemTime += elapsedTime;
			if ((int)mSystemTime != oldTime)
			{
				mScriptTopTime.setTime((int)mSystemTime);
			}
			// 计算名次
			SortedDictionary<float, List<CharacterOther>> distanceList = new SortedDictionary<float, List<CharacterOther>>();
			List<CharacterOther> playerList = mRoleSystem.getAllCharacterList();
			int playerCount = playerList.Count;
			for (int i = 0; i < playerCount; ++i)
			{
				float dis = playerList[i].getCharacterData().mTotalDistance;
				if (!distanceList.ContainsKey(dis))
				{
					distanceList.Add(dis, new List<CharacterOther>());
				}
				distanceList[dis].Add(playerList[i]);
			}
			int rank = 0;
			foreach (var item in distanceList)
			{
				int listCount = item.Value.Count;
				for (int i = 0; i < listCount; ++i)
				{
					CharacterOther character = item.Value[i];
					int curRank = playerCount - 1 - rank;
					if (curRank != character.getCharacterData().mRank)
					{
						CommandCharacterChangeRank cmdRank = newCmd(out cmdRank);
						cmdRank.mRank = curRank;
						pushCommand(cmdRank, item.Value[i]);
					}
					++rank;
				}
			}
			int count = mCollidePairList.Count;
			if (count > 0)
			{
				// 计算每个碰撞对的碰撞效果
				for(int i = 0; i < count; ++i)
				{
					CollidePair pair = mCollidePairList[i];
					CharacterData data0 = pair.mPlayer0.getCharacterData();
					CharacterData data1 = pair.mPlayer1.getCharacterData();
					Vector3 playerSpeed0 = MathUtility.rotateVector3(Vector3.forward * data0.mSpeed, data0.mSpeedRotation.y * Mathf.Deg2Rad);
					Vector3 playerSpeed1 = MathUtility.rotateVector3(Vector3.forward * data1.mSpeed, data1.mSpeedRotation.y * Mathf.Deg2Rad);
					// 将速度分解为沿角色连线方向和连线的法线方向
					Vector3 line = pair.mPlayer0.getPosition() - pair.mPlayer1.getPosition();
					float lineYaw = MathUtility.getVectorYaw(line);
					playerSpeed0 = MathUtility.rotateVector3(playerSpeed0, -lineYaw);
					playerSpeed1 = MathUtility.rotateVector3(playerSpeed1, -lineYaw);
					// 将沿连线方向的速度交换
					MathUtility.swap(ref playerSpeed0.z, ref playerSpeed1.z);
					playerSpeed0 = MathUtility.rotateVector3(playerSpeed0, lineYaw);
					playerSpeed1 = MathUtility.rotateVector3(playerSpeed1, lineYaw);
					data0.mSpeedRotation.y = MathUtility.getVectorYaw(playerSpeed0) * Mathf.Rad2Deg;
					data1.mSpeedRotation.y = MathUtility.getVectorYaw(playerSpeed1) * Mathf.Rad2Deg;
					data0.mSpeed = MathUtility.getLength(playerSpeed0);
					data1.mSpeed = MathUtility.getLength(playerSpeed1);
				}
				mCollidePairList.Clear();
			}
		}
	}
	public void addCollidePair(CharacterOther player0, CharacterOther player1)
	{
		bool hasPair = false;
		int count = mCollidePairList.Count;
		for(int i = 0; i < count; ++i)
		{
			if(mCollidePairList[i].isPair(player0, player1))
			{
				hasPair = true;
				break;
			}
		}
		if(!hasPair)
		{
			UnityUtility.logInfo(player0.getName() + "碰撞了" + player1.getName(), LOG_LEVEL.LL_FORCE);
			mCollidePairList.Add(new CollidePair(player0, player1));
		}
	}
	public void notifyGameStart()
	{
		mSystemTime = 0.0f;
	}
	public void setTrackIndex(int track)
	{
		mTrackIndex = track;
	}
	public int getTrackIndex()
	{
		return mTrackIndex;
	}
	public int getTrackCount()
	{
		return mTrackInfoList.Count;
	}
	public int getLastTrackIndex()
	{
		return (mTrackIndex - 1 + getTrackCount()) % getTrackCount();
	}
	public int getNextTrackIndex()
	{
		return (mTrackIndex + 1) % getTrackCount();
	}
	public GameTrackBase getCurGameTrack()
	{
		return mSceneSystem.getScene<GameTrackBase>(getTrackName());
	}
	public string getTrackName()
	{
		return mTrackInfoList[mTrackIndex].mName;
	}
	public int getTrackDifficultyStart(int index)
	{
		return mTrackInfoList[index].mDifficultyStart;
	}
	public TrackInfo getTrackInfo(string name)
	{
		int count = mTrackInfoList.Count;
		for(int i = 0; i < count; ++i)
		{
			if(mTrackInfoList[i].mName == name)
			{
				return mTrackInfoList[i];
			}
		}
		return null;
	}
}