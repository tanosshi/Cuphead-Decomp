using System;
using System.Collections.Generic;
using RektTransform;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectList : AbstractMonoBehaviour
{
	[Serializable]
	public class SceneGroup
	{
		public bool included;

		public Scenes scene;
	}

	[HideInInspector]
	public SceneGroup[] scenes;

	public Button button;

	public RectTransform contentPanel;

	protected override void Awake()
	{
		base.Awake();
		SetupList();
	}

	private void SetupList()
	{
		List<Scenes> list = new List<Scenes>();
		Scenes[] values = EnumUtils.GetValues<Scenes>();
		foreach (Scenes scenes in values)
		{
			if (GetSceneGroup(scenes).included)
			{
				list.Add(scenes);
			}
		}
		int num = 0;
		foreach (Scenes item in list)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(button.gameObject);
			Button b = gameObject.GetComponent<Button>();
			string text = item.ToString().Replace("scene_", string.Empty).Replace("level_", string.Empty)
				.Replace("dice_palace_", string.Empty)
				.Replace("platforming_", string.Empty);
			b.name = item.ToString();
			gameObject.GetComponentInChildren<Text>().text = text;
			b.onClick.AddListener(delegate
			{
				SceneLoader.LoadScene(b.name, SceneLoader.Transition.Iris, SceneLoader.Transition.Iris);
			});
			b.transform.SetParent(button.transform.parent);
			b.transform.ResetLocalTransforms();
			num++;
		}
		button.gameObject.SetActive(false);
		contentPanel.SetHeight(30f * (float)num);
	}

	public SceneGroup GetSceneGroup(Scenes s)
	{
		SceneGroup[] array = scenes;
		foreach (SceneGroup sceneGroup in array)
		{
			if (sceneGroup.scene == s)
			{
				return sceneGroup;
			}
		}
		return new SceneGroup();
	}

	public bool ContainsScene(Scenes s)
	{
		SceneGroup[] array = scenes;
		foreach (SceneGroup sceneGroup in array)
		{
			if (sceneGroup.scene == s)
			{
				return true;
			}
		}
		return false;
	}
}
