using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TranslationElement : ISerializationCallbackReceiver
{
	[SerializeField]
	private int m_ID;

	[SerializeField]
	private int m_Depth;

	[NonSerialized]
	private TranslationElement m_Parent;

	[NonSerialized]
	private List<TranslationElement> m_Children;

	[SerializeField]
	public string key = string.Empty;

	[SerializeField]
	public Localization.Categories category;

	[SerializeField]
	public string description = string.Empty;

	[SerializeField]
	public Localization.Translation[] translations = new Localization.Translation[8];

	[SerializeField]
	public Localization.Translation[] translationsCuphead;

	[SerializeField]
	public Localization.Translation[] translationsMugman;

	public bool enabled;

	[NonSerialized]
	public bool multiplayerLock = true;

	public Localization.Translation translation
	{
		get
		{
			if (!PlayerManager.Multiplayer && translationsCuphead != null && translationsCuphead.Length > (int)Localization.language)
			{
				return translationsCuphead[(int)Localization.language];
			}
			if (!PlayerManager.Multiplayer && translationsMugman != null && translationsMugman.Length > (int)Localization.language)
			{
				return translationsMugman[(int)Localization.language];
			}
			return translations[(int)Localization.language];
		}
		set
		{
			if (!PlayerManager.Multiplayer && translationsCuphead != null && translationsCuphead.Length > (int)Localization.language)
			{
				translationsCuphead[(int)Localization.language] = value;
			}
			if (!PlayerManager.Multiplayer && translationsMugman != null && translationsMugman.Length > (int)Localization.language)
			{
				translationsMugman[(int)Localization.language] = value;
			}
			translations[(int)Localization.language] = value;
		}
	}

	public int depth
	{
		get
		{
			return m_Depth;
		}
		set
		{
			m_Depth = value;
		}
	}

	public TranslationElement parent
	{
		get
		{
			return m_Parent;
		}
		set
		{
			m_Parent = value;
		}
	}

	public List<TranslationElement> children
	{
		get
		{
			return m_Children;
		}
		set
		{
			m_Children = value;
		}
	}

	public bool hasChildren
	{
		get
		{
			return children != null && children.Count > 0;
		}
	}

	public int id
	{
		get
		{
			return m_ID;
		}
		set
		{
			m_ID = value;
		}
	}

	public TranslationElement()
	{
	}

	public TranslationElement(string key, int depth, int id)
	{
		this.key = key;
		m_ID = id;
		m_Depth = depth;
	}

	public TranslationElement(string key, Localization.Categories category, string description, string translation1, string translation2, int depth, int id)
	{
		m_ID = id;
		m_Depth = depth;
		this.key = key;
		this.category = category;
		this.description = description;
		translations[(int)Localization.language1].text = translation1;
		translations[(int)Localization.language2].text = translation2;
	}

	private Localization.Translation[] Grow(Localization.Translation[] oldTranslations, int newLength)
	{
		Localization.Translation[] array = new Localization.Translation[newLength];
		for (int i = 0; i < oldTranslations.Length; i++)
		{
			array[i].fonts = oldTranslations[i].fonts;
			array[i].image = oldTranslations[i].image;
			array[i].hasImage = oldTranslations[i].hasImage;
			array[i].text = oldTranslations[i].text;
		}
		for (int j = oldTranslations.Length; j < array.Length; j++)
		{
			array[j].fonts = null;
			array[j].image = null;
			array[j].hasImage = false;
			array[j].text = string.Empty;
		}
		return array;
	}

	public void OnBeforeSerialize()
	{
	}

	public void OnAfterDeserialize()
	{
		int num = Enum.GetNames(typeof(Localization.Languages)).Length;
		if (translations.Length < num)
		{
			translations = Grow(translations, num);
			if (translationsCuphead != null)
			{
				translationsCuphead = Grow(translationsCuphead, num);
			}
			if (translationsMugman != null)
			{
				translationsMugman = Grow(translationsMugman, num);
			}
		}
	}
}
