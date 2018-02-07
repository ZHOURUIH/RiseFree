using UnityEngine;
using System.Collections;

// 游戏中状态,只要开始游戏角色就一直有这个状态,直到本局比赛结束
public class GamingState : PlayerState
{
	public GamingState(PLAYER_STATE type)
		:
		base(type)
	{
		;
	}
	public override void enter()
	{
		// 确认当前道具选择
		pushCommand<CommandCharacterSelectItem>(mPlayer);
		CommandCharacterAddState cmdState = newCmd(out cmdState, true, true);
		cmdState.mState = PLAYER_STATE.PS_SPRINT;
		pushDelayCommand(cmdState, mPlayer);
	}
	public override void update(float elapsedTime)
	{
		// 记录自己的运动时间
		mPlayer.getCharacterData().mRunTime += elapsedTime;
	}
	public override void keyProcess(float elapsedTime)
	{
		if (mGameInputManager.getKeyCurrentDown(KeyCode.A))
		{
			CommandCharacterUseItem cmd = newCmd(out cmd);
			cmd.mItemIndex = mPlayer.getPlayerPack().getSelectedIndex();
			pushCommand(cmd, mPlayer);
		}
		//切换道具
		if (mGameInputManager.getKeyCurrentDown(KeyCode.B))
		{
			if (mPlayer.getStateMachine().hasState(PLAYER_STATE.PS_AIM))
			{
				// 移除瞄准状态
				CommandCharacterRemoveState cmdState = newCmd(out cmdState);
				cmdState.mState = PLAYER_STATE.PS_AIM;
				pushCommand(cmdState, mPlayer);
			}
			if(mPlayer.getPlayerPack().canChangeSelection())
			{
				pushCommand<CommandCharacterSelectItem>(mPlayer);
			}
		}
		// 仅测试用
		if (mGameInputManager.getKeyCurrentDown(KeyCode.Alpha1))
		{
			CommandCharacterGetItem cmd = newCmd(out cmd);
			cmd.mItemType = PLAYER_ITEM.PI_MISSILE;
			pushCommand(cmd, mPlayer);
		}
		if (mGameInputManager.getKeyCurrentDown(KeyCode.Alpha2))
		{
			CommandCharacterGetItem cmd = newCmd(out cmd);
			cmd.mItemType = PLAYER_ITEM.PI_SHIELD;
			pushCommand(cmd, mPlayer);
		}
		if (mGameInputManager.getKeyCurrentDown(KeyCode.Alpha3))
		{
			CommandCharacterGetItem cmd = newCmd(out cmd);
			cmd.mItemType = PLAYER_ITEM.PI_TURBO;
			pushCommand(cmd, mPlayer);
		}
		if (mGameInputManager.getKeyCurrentDown(KeyCode.Alpha4))
		{
			CommandCharacterGetItem cmd = newCmd(out cmd);
			cmd.mItemType = PLAYER_ITEM.PI_LAND_MINE;
			pushCommand(cmd, mPlayer);
		}
		// 上方向键增加速度
		if (mGameInputManager.getKeyCurrentDown(KeyCode.UpArrow))
		{
			CommandCharacterHardwareSpeed cmdPassValue = newCmd(out cmdPassValue, false);
			cmdPassValue.mSpeed = mPlayer.getCharacterData().mSpeed + 1.0f;
			pushCommand(cmdPassValue, mPlayer);
		}
		// 下方向键降低速度
		if (mGameInputManager.getKeyCurrentDown(KeyCode.DownArrow))
		{
			CommandCharacterHardwareSpeed cmdPassValue = newCmd(out cmdPassValue, false);
			cmdPassValue.mSpeed = mPlayer.getCharacterData().mSpeed - 1.0f;
			pushCommand(cmdPassValue, mPlayer);
		}
		if (mGameInputManager.getKeyCurrentDown(KeyCode.Space))
		{
			pushCommand<CommandCharacterJump>(mPlayer);
		}
	}
	public override void leave()
	{
		// 比赛完成时,不再计算比赛里程数据
		mPlayer.activeFirstComponent<CharacterTrackMileage>(false);
	}
}