using UnityEngine;

[System.Serializable]
public class AseTextureSettings {
	[SerializeField] public int pixelsPerUnit = 100;
	[SerializeField] public Vector2 spritePivot = new Vector2(0.5f, 0f);

	[SerializeField] public bool flipTexture = false;
}
