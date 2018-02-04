using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using UnityEngine;

public class RaceSystem : FrameComponent
{
	protected float mSystemTime;
	protected int mTrack;
	protected List<string> mTrackNameList;
	public RaceSystem(string name)
		:
		base(name)
	{
		mTrackNameList = new List<string>();
		mSystemTime = 0.0f;
	}
	public override void init()
	{
		mTrackNameList.Add(GameDefine.PRIMARY_TRACK);
		mTrackNameList.Add(GameDefine.SNOW_MOUNTAIN);
		mTrackNameList.Add(GameDefine.DESERT);
		mTrackNameList.Add(GameDefine.ANCIENT_CITY);
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
		}
	}
	public void notifyGameStart()
	{
		mSystemTime = 0.0f;
	}
	public void setTrack(int track)
	{
		mTrack = track;
	}
	public int getTrackIndex()
	{
		return mTrack;
	}
	public int getTrackCount()
	{
		return mTrackNameList.Count;
	}
	public int getLastTrackIndex()
	{
		return (mTrack - 1 + getTrackCount()) % getTrackCount();
	}
	public int getNextTrackIndex()
	{
		return (mTrack + 1) % getTrackCount();
	}
	public GameTrackBase getCurGameTrack()
	{
		return mSceneSystem.getScene<GameTrackBase>(getTrackName());
	}
	public string getTrackName()
	{
		return mTrackNameList[mTrack];
	}
}