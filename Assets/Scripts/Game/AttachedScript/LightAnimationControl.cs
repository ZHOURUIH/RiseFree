using UnityEngine;
using System.Collections;

public class LightAnimationControl : MonoBehaviour
{
	protected Animation mAnim;	// 灯光的动画控制
	void Awake ()
	{
		mAnim = this.GetComponent<Animation>();
		mAnim["Take 001"].speed = 0.08f; // 设置播放速度
	}
	void Start()
	{
		if (!mAnim.isPlaying)
		{
			mAnim.Play("Take 001");
		}
	}
	void Update () 
	{
		;
	}
}
