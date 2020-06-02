using UnityEngine;

[System.Serializable]
public class AseAnimationSettings {
	[SerializeField]
	public string animationName;
	[SerializeField]
	public bool loop;

	//生成动画
	//[SerializeField]
	//public bool generateAnimationClip;

	public AseAnimationSettings(string animationName, bool loop = false) {
		this.animationName = animationName;
		this.loop = loop;
	}
}
