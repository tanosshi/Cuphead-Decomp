using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "LocalizationAsset", menuName = "Localization Asset", order = 1)]
public class Localization : ScriptableObject, ISerializationCallbackReceiver
{
	[SerializeField]
	public enum Languages
	{
		English = 0,
		French = 1,
		Italian = 2,
		German = 3,
		Spanish = 4,
		Japanese = 5,
		Mandarin = 6,
		Russian = 7
	}

	[SerializeField]
	public enum Players
	{
		Multiplayer = 0,
		Cuphead = 1,
		Mugman = 2
	}

	[SerializeField]
	public enum Categories
	{
		NoCategory = 0,
		LevelSelectionName = 1,
		LevelSelectionIn = 2,
		LevelSelectionStage = 3,
		LevelSelectionDifficultyHeader = 4,
		LevelSelectionDifficultys = 5,
		EquipCategoryNames = 6,
		EquipWeaponNames = 7,
		EquipCategoryBackName = 8,
		EquipCategoryBackTitle = 9,
		EquipCategoryBackSubtitle = 10,
		EquipCategoryBackDescription = 11,
		ChecklistTitle = 12,
		ChecklistWorldNames = 13,
		ChecklistContractHeaders = 14,
		ChecklistContracts = 15,
		PauseMenuItems = 16,
		DeathMenuQuote = 17,
		DeathMenuItems = 18,
		ResultsMenuTitle = 19,
		ResultsMenuCategories = 20,
		ResultsMenuGrade = 21,
		ResultsMenuNewRecord = 22,
		ResultsMenuTryNormal = 23,
		IntroEndingText = 24,
		IntroEndingAction = 25,
		CutScenesText = 26,
		SpeechBalloons = 27,
		WorldMapTitles = 28,
		Glyphs = 29,
		TitleScreenSelection = 30,
		Notifications = 31,
		Tutorials = 32,
		OptionMenu = 33,
		RemappingMenu = 34,
		RemappingButton = 35,
		XboxNotification = 36,
		AttractScreen = 37,
		JoinPrompt = 38,
		ConfirmMenu = 39,
		DifficultyMenu = 40
	}

	[Serializable]
	public struct Translation
	{
		[SerializeField]
		public bool hasImage;

		[SerializeField]
		public bool hasCustomFont;

		[SerializeField]
		public string text;

		[SerializeField]
		public CategoryLanguageFont fonts;

		[SerializeField]
		public Sprite image;

		public string SanitizedText()
		{
			return text.Replace("\\n", "\n");
		}
	}

	[Serializable]
	public class CategoryLanguageFont
	{
		public int fontSize;

		[SerializeField]
		public Font font;

		public float fontAssetSize;

		[SerializeField]
		public TMP_FontAsset fontAsset;
	}

	[Serializable]
	public struct CategoryLanguageFonts
	{
		[SerializeField]
		public CategoryLanguageFont[] fonts;

		public CategoryLanguageFont this[int index]
		{
			get
			{
				return fonts[index];
			}
			set
			{
				fonts[index] = value;
			}
		}
	}

	public delegate void LanguageChanged();

	public const string PATH = "LocalizationAsset";

	private static string[] csvKeys = new string[13]
	{
		"key", "category", "description", "|lang|_text", "|lang|_cuphead_text", "|lang|_mugman_text", "|lang|_image", "|lang|_cuphead_image", "|lang|_mugman_image", "|lang|_font",
		"|lang|_fontSize", "|lang|_fontAsset", "|lang|_fontAssetSize"
	};

	private static Localization _instance;

	public static LanguageChanged languageChanged;

	public static Players players = Players.Multiplayer;

	public static Languages language1 = Languages.English;

	public static Languages language2 = Languages.French;

	private static Languages _language = Languages.English;

	[SerializeField]
	private List<TranslationElement> m_TranslationElements = new List<TranslationElement>();

	[SerializeField]
	public CategoryLanguageFonts[] m_Fonts;

	public static Localization Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = Resources.Load<Localization>("LocalizationAsset");
			}
			return _instance;
		}
	}

	public static Languages language
	{
		get
		{
			if (_language == (Languages)(-1))
			{
				_language = Languages.English;
			}
			return _language;
		}
		set
		{
			_language = value;
			languageChanged();
		}
	}

	[SerializeField]
	public CategoryLanguageFonts[] fonts
	{
		get
		{
			if (m_Fonts == null)
			{
				int num = Enum.GetNames(typeof(Languages)).Length;
				int num2 = Enum.GetNames(typeof(Categories)).Length;
				m_Fonts = new CategoryLanguageFonts[num];
				for (int i = 0; i < num; i++)
				{
					m_Fonts[i].fonts = new CategoryLanguageFont[num2];
				}
			}
			return m_Fonts;
		}
		set
		{
			m_Fonts = value;
		}
	}

	[SerializeField]
	public List<TranslationElement> translationElements
	{
		get
		{
			return m_TranslationElements;
		}
		set
		{
			m_TranslationElements = value;
		}
	}

	public static Translation Translate(string key)
	{
		int result;
		if (int.TryParse(key, out result))
		{
			return Translate(result);
		}
		Translation result2 = default(Translation);
		for (int i = 0; i < Instance.m_TranslationElements.Count; i++)
		{
			if (_instance.m_TranslationElements[i].key == key)
			{
				TranslationElement translationElement = _instance.m_TranslationElements[i];
				result2 = translationElement.translation;
			}
		}
		return result2;
	}

	public static Translation Translate(int id)
	{
		Translation result = default(Translation);
		for (int i = 0; i < Instance.m_TranslationElements.Count; i++)
		{
			if (_instance.m_TranslationElements[i].id == id)
			{
				TranslationElement translationElement = _instance.m_TranslationElements[i];
				result = translationElement.translation;
			}
		}
		return result;
	}

	public static TranslationElement Find(string key)
	{
		for (int i = 0; i < Instance.m_TranslationElements.Count; i++)
		{
			if (_instance.m_TranslationElements[i].key == key)
			{
				return _instance.m_TranslationElements[i];
			}
		}
		Debug.LogWarning("Localization Key not found : " + key);
		return null;
	}

	public static TranslationElement Find(int id)
	{
		for (int i = 0; i < Instance.m_TranslationElements.Count; i++)
		{
			if (_instance.m_TranslationElements[i].id == id)
			{
				return _instance.m_TranslationElements[i];
			}
		}
		Debug.LogWarning("Localization ID not found : " + id);
		return null;
	}

	public static void ExportCsv(string path)
	{
		string text = "|lang|";
		string text2 = "|lang|_cuphead";
		string text3 = "|lang|_mugman";
		char value = ';';
		char value2 = '\n';
		StringBuilder stringBuilder = new StringBuilder();
		int num = Enum.GetNames(typeof(Languages)).Length;
		int num2 = Enum.GetNames(typeof(Categories)).Length;
		for (int i = 0; i < csvKeys.Length; i++)
		{
			if (csvKeys[i].Contains(text))
			{
				string value3 = csvKeys[i].Replace(text, string.Empty);
				for (int j = 0; j < num; j++)
				{
					Languages languages = (Languages)j;
					stringBuilder.Append(languages.ToString());
					stringBuilder.Append(value3);
					stringBuilder.Append(value);
				}
			}
			else
			{
				stringBuilder.Append(csvKeys[i]);
				stringBuilder.Append(value);
			}
		}
		stringBuilder.Append(value2);
		for (int k = 0; k < Instance.m_TranslationElements.Count; k++)
		{
			TranslationElement translationElement = _instance.m_TranslationElements[k];
			if (translationElement.depth == -1)
			{
				continue;
			}
			for (int l = 0; l < csvKeys.Length; l++)
			{
				if (csvKeys[l].Contains(text))
				{
					for (int m = 0; m < num; m++)
					{
						Languages languages2 = (Languages)m;
						string text4;
						Translation translation;
						if (csvKeys[l].Contains(text2))
						{
							text4 = csvKeys[l].Replace(text2, string.Empty);
							translation = ((translationElement.translationsCuphead != null && translationElement.translationsCuphead.Length != 0) ? translationElement.translationsCuphead[m] : default(Translation));
						}
						else if (csvKeys[l].Contains(text3))
						{
							text4 = csvKeys[l].Replace(text3, string.Empty);
							translation = ((translationElement.translationsMugman != null && translationElement.translationsMugman.Length != 0) ? translationElement.translationsMugman[m] : default(Translation));
						}
						else
						{
							text4 = csvKeys[l].Replace(text, string.Empty);
							translation = translationElement.translations[m];
						}
						switch (text4)
						{
						case "_text":
							stringBuilder.Append(translation.text);
							break;
						case "_image":
							if (translation.image != null)
							{
								stringBuilder.Append(translation.image.name);
							}
							break;
						case "_font":
							if (translation.fonts.font != null)
							{
								stringBuilder.Append(translation.fonts.font.name);
							}
							break;
						case "_fontSize":
							stringBuilder.Append(translation.fonts.fontSize);
							break;
						case "_fontAsset":
							if (translation.fonts.fontAsset != null)
							{
								stringBuilder.Append(translation.fonts.fontAsset.name);
							}
							break;
						case "_fontAssetSize":
							stringBuilder.Append(translation.fonts.fontAssetSize);
							break;
						}
						stringBuilder.Append(value);
					}
				}
				else
				{
					switch (csvKeys[l])
					{
					case "key":
						stringBuilder.Append(translationElement.key);
						break;
					case "category":
						stringBuilder.Append(translationElement.category.ToString());
						break;
					case "description":
						stringBuilder.Append(translationElement.description);
						break;
					}
					stringBuilder.Append(value);
				}
			}
			stringBuilder.Append(value2);
		}
		Encoding encoding = new UTF8Encoding(true);
		byte[] bytes = encoding.GetBytes(stringBuilder.ToString());
		FileStream fileStream = new FileStream(path, FileMode.Create);
		byte[] preamble = encoding.GetPreamble();
		fileStream.Write(preamble, 0, preamble.Length);
		fileStream.Write(bytes, 0, bytes.Length);
		fileStream.Dispose();
	}

	public static void ImportCsv(string path)
	{
		char c = ';';
		char c2 = '\n';
		string text = "_cuphead";
		string text2 = "_mugman";
		string[] names = Enum.GetNames(typeof(Languages));
		string[] names2 = Enum.GetNames(typeof(Categories));
		Encoding encoding = new UTF8Encoding(true);
		FileStream fileStream = new FileStream(path, FileMode.Open);
		byte[] preamble = encoding.GetPreamble();
		byte[] array = new byte[preamble.Length];
		fileStream.Read(array, 0, preamble.Length);
		bool flag = true;
		for (int i = 0; i < preamble.Length; i++)
		{
			if (preamble[i] != array[i])
			{
				flag = false;
				break;
			}
		}
		array = ((!flag) ? new byte[fileStream.Length] : new byte[fileStream.Length - preamble.Length]);
		fileStream.Read(array, 0, array.Length);
		fileStream.Dispose();
		string text3 = encoding.GetString(array);
		string[] array2 = text3.Split(c2);
		string[] array3 = array2[0].Split(c);
		Instance.m_TranslationElements.Clear();
		TranslationElement item = new TranslationElement("Root", -1, 0);
		_instance.m_TranslationElements.Add(item);
		for (int j = 1; j < array2.Length; j++)
		{
			string[] array4 = array2[j].Split(c);
			if (array4.Length != array3.Length)
			{
				if (array2[j] != string.Empty)
				{
					Debug.Log(array2[j]);
				}
				continue;
			}
			item = Instance.AddKey();
			for (int k = 0; k < array4.Length; k++)
			{
				if (string.IsNullOrEmpty(array4[k]))
				{
					continue;
				}
				string text4 = array3[k];
				switch (text4)
				{
				case "key":
					item.key = array4[k];
					continue;
				case "category":
				{
					int category = -1;
					for (int l = 0; l < names2.Length; l++)
					{
						if (names2[l] == array4[k])
						{
							category = l;
						}
					}
					item.category = (Categories)category;
					continue;
				}
				case "description":
					item.description = array4[k];
					continue;
				}
				for (int m = 0; m < names.Length; m++)
				{
					if (!text4.Contains(names[m]))
					{
						continue;
					}
					text4 = text4.Replace(names[m], string.Empty);
					bool flag2 = false;
					bool flag3 = false;
					Translation translation;
					if (text4.Contains(text))
					{
						flag2 = true;
						text4 = text4.Replace(text, string.Empty);
						if (item.translationsCuphead == null || item.translationsCuphead.Length == 0)
						{
							item.translationsCuphead = new Translation[names.Length];
							item.translationsMugman = new Translation[names.Length];
						}
						translation = item.translationsCuphead[m];
					}
					else if (text4.Contains(text2))
					{
						flag3 = true;
						text4 = text4.Replace(text2, string.Empty);
						if (item.translationsCuphead == null || item.translationsCuphead.Length == 0)
						{
							item.translationsCuphead = new Translation[names.Length];
							item.translationsMugman = new Translation[names.Length];
						}
						translation = item.translationsMugman[m];
					}
					else
					{
						translation = item.translations[m];
					}
					if (translation.fonts == null)
					{
						translation.fonts = new CategoryLanguageFont();
					}
					switch (text4)
					{
					case "_text":
						translation.text = array4[k];
						break;
					case "_image":
						if (string.IsNullOrEmpty(array4[k]))
						{
							goto end_IL_0543;
						}
						break;
					case "_font":
						if (string.IsNullOrEmpty(array4[k]))
						{
							goto end_IL_0543;
						}
						break;
					case "_fontSize":
						if (!string.IsNullOrEmpty(array4[k]))
						{
							int num2 = Convert.ToInt32(array4[k]);
							if (num2 == 0)
							{
								goto end_IL_0543;
							}
							translation.hasCustomFont = true;
							translation.fonts.fontSize = num2;
						}
						break;
					case "_fontAsset":
						if (string.IsNullOrEmpty(array4[k]))
						{
							goto end_IL_0543;
						}
						break;
					case "_fontAssetSize":
						if (!string.IsNullOrEmpty(array4[k]))
						{
							float num = Convert.ToSingle(array4[k]);
							if (num == 0f)
							{
								goto end_IL_0543;
							}
							translation.hasCustomFont = true;
							translation.fonts.fontAssetSize = num;
						}
						break;
					}
					if (flag2)
					{
						item.translationsCuphead[m] = translation;
					}
					else if (flag3)
					{
						item.translationsMugman[m] = translation;
					}
					else
					{
						item.translations[m] = translation;
					}
					break;
					continue;
					end_IL_0543:
					break;
				}
			}
		}
	}

	public TranslationElement AddKey()
	{
		int num = -1;
		for (int i = 0; i < m_TranslationElements.Count; i++)
		{
			if (m_TranslationElements[i].id > num)
			{
				num = m_TranslationElements[i].id;
			}
		}
		num++;
		TranslationElement translationElement = new TranslationElement("Key" + num, Categories.NoCategory, string.Empty, string.Empty, string.Empty, 0, num);
		m_TranslationElements.Add(translationElement);
		return translationElement;
	}

	private void Awake()
	{
		if (m_TranslationElements.Count == 0)
		{
			m_TranslationElements = new List<TranslationElement>(1);
			TranslationElement item = new TranslationElement("Root", -1, 0);
			m_TranslationElements.Add(item);
		}
	}

	public void OnBeforeSerialize()
	{
	}

	public void OnAfterDeserialize()
	{
		bool flag = false;
		int num = Enum.GetNames(typeof(Languages)).Length;
		if (fonts.Length < num)
		{
			flag = true;
		}
		int num2 = Enum.GetNames(typeof(Categories)).Length;
		if (fonts[0].fonts.Length < num2)
		{
			flag = true;
		}
		if (flag)
		{
			fonts = GrowFonts(fonts, num, num2);
		}
	}

	private CategoryLanguageFonts[] GrowFonts(CategoryLanguageFonts[] oldFonts, int newLanguagesLength, int newCategoriesLength)
	{
		CategoryLanguageFonts[] array = new CategoryLanguageFonts[newLanguagesLength];
		for (int i = 0; i < newLanguagesLength; i++)
		{
			array[i].fonts = new CategoryLanguageFont[newCategoriesLength];
		}
		for (int j = 0; j < oldFonts.Length; j++)
		{
			for (int k = 0; k < oldFonts[j].fonts.Length; k++)
			{
				array[j][k] = oldFonts[j][k];
			}
		}
		return array;
	}
}
