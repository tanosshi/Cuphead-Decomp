using UnityEngine;

public class SymbolPicker : MonoBehaviour
{
	[SerializeField]
	private LocalizationHelper localizationHelper;

	public CupheadButton button;

	private void OnEnable()
	{
		ApplySymbol();
	}

	public void ApplySymbol()
	{
		TranslationElement translationElement = Localization.Find(button.ToString());
		localizationHelper.ApplyTranslation(translationElement);
	}
}
