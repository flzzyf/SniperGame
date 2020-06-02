using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;

[CustomEditor(typeof(AseImporter))]
[CanEditMultipleObjects]
public class AseImporterEditor : ScriptedImporterEditor{
	private bool customSpritePivot = false;

	AseImporter importer;

	Dictionary<string, bool> foldoutStates = new Dictionary<string, bool>();

	public override void OnEnable() {
		base.OnEnable();

		importer = target as AseImporter;
		foldoutStates.Clear();
	}

	public override void OnInspectorGUI() {
		//更新数据
		serializedObject.Update();

		string textureSettings = "textureSettings.";

		//像素每单位
		EditorGUILayout.PropertyField(serializedObject.FindProperty(textureSettings + "pixelsPerUnit"));

		//锚点设置
		PivotPopup("Pivot");

		//水平翻转贴图
		//EditorGUILayout.PropertyField(serializedObject.FindProperty(textureSettings + "flipTexture"), new GUIContent("水平翻转贴图"));

		EditorGUILayout.PropertyField(serializedObject.FindProperty("addSuffix"), new GUIContent("动画名自动添加前缀"));

		//生成动画
		EditorGUILayout.PropertyField(serializedObject.FindProperty("generateAnimation"), new GUIContent("生成动画"));

		//动画设置
		if (importer.animationSettings != null) {
			EditorGUILayout.LabelField("动画设置", EditorStyles.boldLabel);
			{
				for (int i = 0; i < importer.animationSettings.Length; i++) {
					DrawAnimationSetting(importer.animationSettings[i], i);
				}
			}
		}

		//if (GUILayout.Button("生成动画文件")) {
		//	AseImporter.generateAnimation = true;
		//	//重新导入
		//	AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(importer), ImportAssetOptions.ForceUpdate);
		//	AseImporter.generateAnimation = false;
		//}

		//提交修改
		serializedObject.ApplyModifiedProperties();
		//显示Apply按钮
		ApplyRevertGUI();
	}

	//动画设置
	void DrawAnimationSetting(AseAnimationSettings setting, int index) {

		if (!foldoutStates.ContainsKey(setting.animationName)) {
			foldoutStates.Add(setting.animationName, false);
		}

		EditorGUILayout.BeginVertical(GUI.skin.box);
		EditorGUI.indentLevel++;

		string title = string.Format("{0}. {1}", index + 1, setting.animationName);
		if (foldoutStates[setting.animationName] = EditorGUILayout.Foldout(foldoutStates[setting.animationName], title, true)) {
			EditorGUI.indentLevel++;
			setting.loop = EditorGUILayout.Toggle("Loop", setting.loop);

			//EditorGUILayout.HelpBox("qwer", MessageType.None);

			EditorGUI.indentLevel--;
		}

		EditorGUI.indentLevel--;
		EditorGUILayout.EndVertical();
	}

	#region 锚点设置
	private string[] spritePivotOptions = new string[]{
		"Center", "Top Left", "Top", "Top Right", "Left", "Right", "Bottom Left", "Bottom", "Bottom Right", "Custom"
	};

	private void PivotPopup(string label) {
		var pivotProperty = serializedObject.FindProperty("textureSettings.spritePivot");
		var pivot = pivotProperty.vector2Value;

		EditorGUI.BeginChangeCheck();
		switch (EditorGUILayout.Popup(label, GetSpritePivotOptionIndex(pivot), spritePivotOptions)) {
			case 0:
				customSpritePivot = false;
				pivot = new Vector2(0.5f, 0.5f);
				break;
			case 1:
				customSpritePivot = false;
				pivot = new Vector2(0f, 1f);
				break;
			case 2:
				customSpritePivot = false;
				pivot = new Vector2(0.5f, 1f);
				break;
			case 3:
				customSpritePivot = false;
				pivot = new Vector2(1f, 1f);
				break;
			case 4:
				customSpritePivot = false;
				pivot = new Vector2(0f, 0.5f);
				break;
			case 5:
				customSpritePivot = false;
				pivot = new Vector2(1f, 0.5f);
				break;
			case 6:
				customSpritePivot = false;
				pivot = new Vector2(0f, 0f);
				break;
			case 7:
				customSpritePivot = false;
				pivot = new Vector2(0.5f, 0f);
				break;
			case 8:
				customSpritePivot = false;
				pivot = new Vector2(1f, 0f);
				break;
			default:
				customSpritePivot = true;
				break;
		}

		if (customSpritePivot) {
			EditorGUI.indentLevel++;
			EditorGUILayout.PropertyField(serializedObject.FindProperty("textureSettings.spritePivot"),
				new GUIContent(label));
			EditorGUI.indentLevel--;
		}
		else if (EditorGUI.EndChangeCheck() && !customSpritePivot) {
			pivotProperty.vector2Value = pivot;
		}
	}

	private int GetSpritePivotOptionIndex(Vector2 spritePivot) {
		if (customSpritePivot)
			return spritePivotOptions.Length - 1;

		if (spritePivot.x == 0.5f && spritePivot.y == 0.5f) return 0;
		if (spritePivot.x == 0f && spritePivot.y == 1f) return 1;
		if (spritePivot.x == 0.5f && spritePivot.y == 1f) return 2;
		if (spritePivot.x == 1f && spritePivot.y == 1f) return 3;
		if (spritePivot.x == 0f && spritePivot.y == 0.5f) return 4;
		if (spritePivot.x == 1f && spritePivot.y == 0.5f) return 5;
		if (spritePivot.x == 0f && spritePivot.y == 0f) return 6;
		if (spritePivot.x == 0.5f && spritePivot.y == 0f) return 7;
		if (spritePivot.x == 1f && spritePivot.y == 0f) return 8;

		return spritePivotOptions.Length - 1; // Last one = custom
	}

	#endregion
}
