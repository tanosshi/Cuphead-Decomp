using System;
using System.Collections;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine.UI;

public class UIButtonAnimator : AbstractMonoBehaviour
{
	private const float FRAME_DELAY = 0.4f;

	private Text uiText;

	private TextMeshProUGUI tmpText;

	protected string Text
	{
		get
		{
			return tmpText.text;
		}
		set
		{
			tmpText.SetText(value);
		}
	}

	protected override void Start()
	{
		base.Start();
		uiText = GetComponent<Text>();
		tmpText = GetComponent<TextMeshProUGUI>();
		StartCoroutine(animate_cr());
	}

	private IEnumerator animate_cr()
	{
		yield return null;
		yield return null;
		string first = Text;
		string second = Text;
		MatchCollection keys = Regex.Matches(Text, "{([^}]*)}", RegexOptions.Multiline | RegexOptions.ExplicitCapture);
		CupheadButton[] buttons = new CupheadButton[keys.Count];
		for (int i = 0; i < keys.Count; i++)
		{
			buttons[i] = (CupheadButton)Enum.Parse(typeof(CupheadButton), keys[i].Value.Substring(1, keys[i].Value.Length - 2));
		}
		for (int j = 0; j < CupheadInput.pairs.Length; j++)
		{
			for (int k = 0; k < buttons.Length; k++)
			{
				CupheadInput.InputSymbols inputSymbols = CupheadInput.InputSymbolForButton(buttons[k]);
				if (inputSymbols == CupheadInput.pairs[j].symbol)
				{
					first = first.Replace("{" + buttons[k].ToString() + "}", CupheadInput.pairs[j].first);
				}
			}
		}
		for (int l = 0; l < CupheadInput.pairs.Length; l++)
		{
			for (int m = 0; m < buttons.Length; m++)
			{
				CupheadInput.InputSymbols inputSymbols2 = CupheadInput.InputSymbolForButton(buttons[m]);
				if (inputSymbols2 == CupheadInput.pairs[l].symbol)
				{
					second = second.Replace("{" + buttons[m].ToString() + "}", CupheadInput.pairs[l].second);
				}
			}
		}
		Text = first;
		while (true)
		{
			yield return CupheadTime.WaitForSeconds(this, 0.4f);
			Text = second;
			yield return CupheadTime.WaitForSeconds(this, 0.4f);
			Text = first;
		}
	}
}
