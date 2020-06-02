using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor.Experimental.AssetImporters;

public class AseAtlasGenerator : MonoBehaviour{


	public static Texture2D GenerateAtlas(Texture2D[] sprites, out SpriteImportData[] spriteData, Vector2Int spriteSize, AseTextureSettings textureSettings) {
		var cols = sprites.Length;
		var rows = 1;

		float spriteCount = sprites.Length;

		var divider = 2;

		var width = cols * spriteSize.x;
		var height = rows * spriteSize.y;


		while (width > height) {
			cols = (int)Math.Ceiling(spriteCount / divider);
			rows = (int)Math.Ceiling(spriteCount / cols);

			width = cols * spriteSize.x;
			height = rows * spriteSize.y;

			if (cols <= 1) {
				break;
			}

			divider++;
		}

		if (height > width)
			divider -= 2;
		else
			divider -= 1;

		if (divider < 1)
			divider = 1;

		cols = (int)Math.Ceiling(spriteCount / divider);
		rows = (int)Math.Ceiling(spriteCount / cols);

		return GenerateAtlas(sprites, out spriteData, spriteSize, textureSettings, cols, rows);
	}

	static Texture2D GenerateAtlas(Texture2D[] sprites, out SpriteImportData[] spriteData, Vector2Int spriteSize, AseTextureSettings textureSettings, int cols, int rows) {
		var spriteImportData = new List<SpriteImportData>();

		var width = cols * spriteSize.x;
		var height = rows * spriteSize.y;

		var atlas = CreateTransparentTexture(width, height);
		var index = 0;

		for (var row = 0; row < rows; row++) {
			for (var col = 0; col < cols; col++) {
				Rect spriteRect = new Rect(col * spriteSize.x, atlas.height - ((row + 1) * spriteSize.y), spriteSize.x, spriteSize.y);
				atlas.SetPixels((int)spriteRect.x, (int)spriteRect.y, (int)spriteRect.width, (int)spriteRect.height, sprites[index].GetPixels());
				atlas.Apply();

				var importData = new SpriteImportData {
					rect = spriteRect,
					pivot = textureSettings.spritePivot,
					border = Vector4.zero,
					name = index.ToString()
				};

				spriteImportData.Add(importData);

				index++;
				if (index >= sprites.Length)
					break;
			}
			if (index >= sprites.Length)
				break;
		}

		spriteData = spriteImportData.ToArray();
		return atlas;
	}

	//创建透明贴图
	static Texture2D CreateTransparentTexture(int width, int height) {
		Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
		Color[] pixels = new Color[width * height];

		for (int i = 0; i < pixels.Length; i++)
			pixels[i] = Color.clear;

		texture.SetPixels(pixels);
		texture.Apply();

		return texture;
	}

}
