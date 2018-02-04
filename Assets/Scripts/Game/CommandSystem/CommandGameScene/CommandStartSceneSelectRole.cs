using UnityEngine;
using System.Collections;

public class CommandStartSceneSelectRole : Command
{
	public int mIndex;
	public override void init()
	{
		base.init();
		mIndex = 0;
	}
	public override void execute()
	{
		StartScene gameScene = (mReceiver) as StartScene;
		if (gameScene.atProcedure(PROCEDURE_TYPE.PT_START_SELECT_ROLE))
		{
			CharacterOther curRole = mRoleSystem.getSelectedRole();
			// 当前角色有SELECT_ROLE状态组,如果没有,则说明是第一次进入选人流程
			if (curRole != null && curRole.hasStateGroup(0))
			{
				// 当前角色显示完毕时才能切换角色,也不能选择相同的角色
				if (!curRole.hasState(PLAYER_STATE.PS_SELECTED_ROLE) || mRoleSystem.getSelectedIndex() == mIndex)
				{
					return;
				}
				// 取消当前角色的选中
				CommandCharacterAddState cmdUnSelect = newCmd(out cmdUnSelect);
				cmdUnSelect.mState = PLAYER_STATE.PS_UN_SELECT_ROLE;
				pushCommand(cmdUnSelect, curRole);
			}
			// 启动选择的角色
			mRoleSystem.setSelectedIndex(mIndex);
			CharacterOther character = mRoleSystem.getSelectedRole();
			if (character != null)
			{
				CommandCharacterAddState cmdSelect = newCmd(out cmdSelect);
				cmdSelect.mState = PLAYER_STATE.PS_ON_SELECT_ROLE;
				pushCommand(cmdSelect, character);
				// 通知布局
				mScriptSelectRole.selectRole(mIndex);
			}
		}
	}
	public override string showDebugInfo()
	{
		return this.GetType().ToString() + " : index : " + mIndex;
	}
}
