using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LocalizationHelper : MonoBehaviour
{
	public struct LocalizationSubtext
	{
		public string key;

		public string value;

		public bool dontTranslate;

		public LocalizationSubtext(string key, string value, bool dontTranslate = false)
		{
			this.key = key;
			this.value = value;
			this.dontTranslate = dontTranslate;
		}
	}

	public bool existingKey;

	public string currentKey;

	public int currentID = -1;

	public Localization.Languages currentLanguage = (Localization.Languages)(-1);

	public Localization.Categories currentCategory;

	public bool currentCustomFont;

	public Text textComponent;

	public Image imageComponent;

	public SpriteRenderer spriteRendererComponent;

	public TMP_Text textMeshProComponent;

	public TranslationElement translationElement;

	private LocalizationSubtext[] subTranslations;

	private void OnEnable()
	{
		ApplyTranslation();
	}

	public void ApplyTranslation(TranslationElement translationElement, LocalizationSubtext[] subTranslations = null)
	{
		this.translationElement = translationElement;
		this.subTranslations = subTranslations;
		currentLanguage = Localization.language;
		ApplyTranslation();
	}

	public void ApplyTranslation()
	{
		if (currentLanguage == (Localization.Languages)(-1) || translationElement == null)
		{
			return;
		}
		Localization.Translation translation = translationElement.translation;
		string text = translation.text;
		if (text.Contains("{") && text.Contains("}"))
		{
			if (subTranslations != null)
			{
				bool flag = true;
				while (flag)
				{
					flag = false;
					for (int i = 0; i < subTranslations.Length; i++)
					{
						if (text.Contains("{" + subTranslations[i].key + "}"))
						{
							flag = true;
							if (subTranslations[i].dontTranslate)
							{
								text = text.Replace("{" + subTranslations[i].key + "}", subTranslations[i].value);
								continue;
							}
							Localization.Translation translation2 = Localization.Translate(subTranslations[i].value);
							text = ((!string.IsNullOrEmpty(translation2.text)) ? text.Replace("{" + subTranslations[i].key + "}", translation2.text) : text.Replace("{" + subTranslations[i].key + "}", subTranslations[i].value));
						}
					}
				}
			}
			string[] array = text.Split('{');
			if (array.Length > 1)
			{
				string[] array2 = array[1].Split('}');
				if (array2.Length > 1)
				{
					string text2 = array2[0];
					Localization.Translation translation3 = Localization.Translate(text2);
					if (!string.IsNullOrEmpty(translation3.text))
					{
						text = text.Replace("{" + text2 + "}", translation3.text);
					}
				}
			}
		}
		if (textComponent != null)
		{
			textComponent.text = text;
			textComponent.enabled = false;
			textComponent.enabled = !string.IsNullOrEmpty(text);
			if (translation.hasCustomFont)
			{
				textComponent.font = translation.fonts.font;
			}
			else if (Localization.Instance.fonts[(int)currentLanguage][(int)translationElement.category].font != null)
			{
				textComponent.font = Localization.Instance.fonts[(int)currentLanguage][(int)translationElement.category].font;
			}
			else
			{
				Debug.Log(translationElement.category.ToString() + " font is null!");
			}
		}
		if (textMeshProComponent != null)
		{
			textMeshProComponent.text = text;
			textMeshProComponent.enabled = false;
			textMeshProComponent.enabled = !string.IsNullOrEmpty(text);
			if (translation.hasCustomFont)
			{
				textMeshProComponent.font = translation.fonts.fontAsset;
			}
			else
			{
				textMeshProComponent.font = Localization.Instance.fonts[(int)currentLanguage][(int)translationElement.category].fontAsset;
			}
		}
		if (spriteRendererComponent != null)
		{
			spriteRendererComponent.sprite = translation.image;
			spriteRendererComponent.enabled = false;
			spriteRendererComponent.enabled = translation.image != null;
		}
		if (imageComponent != null)
		{
			imageComponent.sprite = translation.image;
			imageComponent.enabled = false;
			imageComponent.enabled = translation.image != null;
		}
	}
}
