using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ScriptGlobalAudio : LayoutScript
{
	protected List<txUIObject> mAudioWindowList;
	public ScriptGlobalAudio(string name, GameLayout layout)
		:
		base(name, layout)
	{
		mAudioWindowList = new List<txUIObject>();
	}
	public override void assignWindow(){}
	public override void init(){}
	public override void onReset(){}
	public override void onShow(bool immediately, string param){}
	public override void update(float elapsedTime){}
	public override void onHide(bool immediately, string param){}
	public txUIObject getAudioWindow()
	{
		txUIObject audioWindow = null;
		int count = mAudioWindowList.Count;
		for(int i = 0; i < count; ++i)
		{
			// 该窗口没有正在播放音效,则使用该窗口播放音效
			if(!mAudioWindowList[i].getAudioSource().isPlaying)
			{
				audioWindow = mAudioWindowList[i];
				break;
			}
		}
		// 如果找不到可以使用的窗口,则创建一个窗口
		if(audioWindow == null)
		{
			audioWindow = createObject<txUIObject>(mRoot, "AudioWindow" + count);
			mAudioWindowList.Add(audioWindow);
		}
		return audioWindow;
	}
}