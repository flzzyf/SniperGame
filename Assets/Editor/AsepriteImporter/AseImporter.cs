using UnityEngine;
using UnityEditor.Experimental.AssetImporters;
using Aseprite;
using Aseprite.Chunks;
using System.IO;
using UnityEditor;
using System.Collections.Generic;

[ScriptedImporter(1, new[] { "ase", "aseprite" })]
public class AseImporter : ScriptedImporter {
	Texture2D atlas;
	[SerializeField]
	public AseTextureSettings textureSettings = new AseTextureSettings();
	[SerializeField]
	public AseAnimationSettings[] animationSettings;

	//是否生成动画文件
	[SerializeField]
	public bool generateAnimation = false;

	//是否自动添加前缀
	public bool addSuffix = true;

	struct AseFrame {
		public Sprite sprite;
		//帧时长（毫秒）
		public float duration;

		public AseFrame(Sprite sprite, float duration) {
			this.sprite = sprite;
			this.duration = duration;
		}
	}

	public void Reimport() {
		AssetDatabase.Refresh();
	}

	//导入
	public override void OnImportAsset(AssetImportContext ctx) {
		name = GetFileName(ctx.assetPath);

        AseFile aseFile = ReadAseFile(ctx.assetPath);

		Texture2D[] frames = aseFile.GetFrames();

		//翻转贴图
		//if (textureSettings.flipTexture) {
		//	for (int i = 0; i < frames.Length; i++) {
		//		frames[i] = FlipTexture(frames[i]);
		//	}
		//}

		SpriteImportData[] spriteImportData = new SpriteImportData[0];
		Vector2Int size = new Vector2Int(aseFile.Header.Width, aseFile.Header.Height);
		atlas = AseAtlasGenerator.GenerateAtlas(frames, out spriteImportData, size, textureSettings);
		atlas.filterMode = FilterMode.Point;
		ctx.AddObjectToAsset("Texture", atlas);

		//生成贴图文件
		GenerateSprites(ctx, aseFile, spriteImportData);

        ctx.SetMainObject(atlas);
	}

	//生成贴图文件
	void GenerateSprites(AssetImportContext ctx, AseFile aseFile, SpriteImportData[] spriteImportData) {
		int spriteCount = spriteImportData.Length;
		AseFrame[] frames = new AseFrame[spriteImportData.Length];

		FrameTag[] tags = aseFile.GetAnimations();

		for (int i = 0; i < spriteCount; i++) {
			Sprite sprite = Sprite.Create(atlas, spriteImportData[i].rect, spriteImportData[i].pivot,
			textureSettings.pixelsPerUnit, 1, SpriteMeshType.Tight, spriteImportData[i].border);

			sprite.name = string.Format("{0}_{1}", name, i);

			ctx.AddObjectToAsset(sprite.name, sprite);

			frames[i] = new AseFrame(sprite, aseFile.Frames[i].FrameDuration / 1000f);
		}

		//生成动画文件
		GenerateAnimations(ctx, aseFile, frames);
	}

	//生成动画
	AseAnimationSettings GenerateAnimation(AssetImportContext ctx, string name, AseFrame[] frames, int startFrame = 0, int endFrame = 0) {
		AseAnimationSettings animationSetting = GetAnimationSetting(name);

		if (generateAnimation) {
			AnimationClip clip = new AnimationClip();
			clip.name = name;
			clip.frameRate = 25;

			if (endFrame == 0) {
				endFrame = frames.Length - 1;
			}

			EditorCurveBinding spriteBinding = new EditorCurveBinding();
			spriteBinding.type = typeof(SpriteRenderer);
			spriteBinding.path = "";
			spriteBinding.propertyName = "m_Sprite";

			int length = endFrame - startFrame + 1;
			ObjectReferenceKeyframe[] spriteKeyFrames = new ObjectReferenceKeyframe[length + 1];

			float time = 0;
			for (int i = startFrame; i <= endFrame; i++) {
				ObjectReferenceKeyframe frame = new ObjectReferenceKeyframe();
				frame.time = time;
				frame.value = frames[i].sprite;
				time += frames[i].duration;

				spriteKeyFrames[i - startFrame] = frame;
			}

			//单独设置最后一帧
			float frameTime = 1f / clip.frameRate;
			ObjectReferenceKeyframe lastFrame = new ObjectReferenceKeyframe();
			lastFrame.time = time - frameTime;
			lastFrame.value = frames[endFrame].sprite;
			spriteKeyFrames[spriteKeyFrames.Length - 1] = lastFrame;

			AnimationUtility.SetObjectReferenceCurve(clip, spriteBinding, spriteKeyFrames);
			AnimationClipSettings settings = AnimationUtility.GetAnimationClipSettings(clip);

			//循环设置
			if (animationSetting.loop) {
				settings.loopTime = true;
				clip.wrapMode = WrapMode.Loop;
			}
			else {
				settings.loopTime = false;
				clip.wrapMode = WrapMode.Once;
			}

			AnimationUtility.SetAnimationClipSettings(clip, settings);
			ctx.AddObjectToAsset(name, clip);

			//string filePath = string.Format("{0}/{1}.anim", GetFileDic(ctx.assetPath), name);
			//AssetDatabase.CreateAsset(clip, filePath);
			//clip = AssetDatabase.LoadAssetAtPath(filePath, typeof(AnimationClip)) as AnimationClip;
			//Debug.Log(AssetDatabase.CopyAsset();
			//AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(clip), filePath);
		}

		return animationSetting;
	}

	//生成动画组
	void GenerateAnimations(AssetImportContext ctx, AseFile aseFile, AseFrame[] frames) {
		FrameTag[] tags = aseFile.GetAnimations();
		List<AseAnimationSettings> settingList;

		//如果当前没有保存的动画设置
		if (animationSettings == null || animationSettings.Length == 0) {
			settingList = new List<AseAnimationSettings>();
		}
		//已有保存的动画设置
		else {
			//去除其中已失效的动画
			RemoveUnusedAnimationSettings(tags);

			settingList = new List<AseAnimationSettings>(animationSettings);
		}

		//如果一个Tag都没有
		if (tags.Length == 0) {
			//直接以文件命名
			//生成动画
			AseAnimationSettings animationSetting = GenerateAnimation(ctx, name, frames);
			if (!settingList.Contains(animationSetting)) {
				settingList.Add(animationSetting);
			}
		}
		//有Tag
		else {
			for (int i = 0; i < tags.Length; i++) {
				FrameTag tag = tags[i];

				string animationName = GetAnimationName(tag.TagName);

				//生成动画
				AseAnimationSettings animationSetting = GenerateAnimation(ctx, animationName, frames, tag.FrameFrom, tag.FrameTo);

				if (!settingList.Contains(animationSetting)) {
					settingList.Add(animationSetting);
				}
			}
		}

		animationSettings = settingList.ToArray();
	}

	//获取之前的动画设定
	AseAnimationSettings GetAnimationSetting(string animationName) {
		if (animationSettings != null) {
			foreach (var setting in animationSettings) {
				if (setting.animationName == animationName) {
					return setting;
				}
			}
		}

		return new AseAnimationSettings(animationName);
	}

	//移除失效的动画设定
	void RemoveUnusedAnimationSettings(FrameTag[] tags) {
		List<AseAnimationSettings> settingList = new List<AseAnimationSettings>(animationSettings);

		for (int i = settingList.Count - 1; i >= 0; i--) {
			string animName = settingList[i].animationName;
			bool found = false;

			if (tags.Length > 0) {
				foreach (var tag in tags) {
					if (GetAnimationName(tag.TagName) == animName) {
						found = true;
						break;
					}
				}
			}
			else {
				if (name == animName) {
					found = true;
				}
			}

			if (!found) {
				settingList.RemoveAt(i);
			}
		}

		animationSettings = settingList.ToArray();
	}


	public void Generate() {
		//AnimationClip clip = clips[0].
		//AssetDatabase.CreateAsset(clips[0], "Assets/" + name + ".anim");
		AssetDatabase.Refresh();
	}


	#region 帮助方法

	//从Tag获取动画名
	string GetAnimationName(string tagName) {
		string animationName = tagName;

		//自动添加前缀
		if (addSuffix) {
			//文件名前缀
			string prefix = name.Split('_')[0];
			animationName = string.Format("{0}_{1}", prefix, animationName);
		}

		return animationName;
	}

	//从路径获取文件名称
	string GetFileName(string assetPath) {
		string[] parts = assetPath.Split('/');
		string filename = parts[parts.Length - 1];

		return filename.Substring(0, filename.LastIndexOf('.'));
	}
	//从路径获取文件路径
	string GetFileDic(string assetPath) {
		string[] parts = assetPath.Split('/');
		string filename = parts[parts.Length - 1];

		return assetPath.Substring(0, assetPath.Length - filename.Length - 1);
	}

	//读取Ase文件
	private static AseFile ReadAseFile(string assetPath) {
		FileStream fileStream = new FileStream(assetPath, FileMode.Open, FileAccess.Read);
		AseFile aseFile = new AseFile(fileStream);
		fileStream.Close();

		return aseFile;
	}

	//翻转Texture2D
	Texture2D FlipTexture(Texture2D original) {
		Texture2D flipped = new Texture2D(original.width, original.height);

		int xN = original.width;
		int yN = original.height;

		for (int i = 0; i < xN; i++) {
			for (int j = 0; j < yN; j++) {
				flipped.SetPixel(xN - i - 1, j, original.GetPixel(i, j));
			}
		}
		flipped.Apply();

		return flipped;
	}

	#endregion
}
