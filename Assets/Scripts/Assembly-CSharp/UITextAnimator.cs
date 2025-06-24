using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UITextAnimator : AbstractMonoBehaviour
{
	private const int DIFFERENCE = 1;

	[SerializeField]
	private float frameDelay = 0.07f;

	private Text text;

	private string textString;

	protected override void Awake()
	{
		base.Awake();
		text = GetComponent<Text>();
		textString = text.text;
	}

	protected override void Start()
	{
		base.Start();
		StartCoroutine(anim_cr());
	}

	public void SetString(string s)
	{
		textString = s;
	}

	private IEnumerator anim_cr()
	{
		while (true)
		{
			this.text.text = string.Empty;
			for (int i = 0; i < textString.Length; i++)
			{
				Text obj = this.text;
				string text = obj.text;
				obj.text = text + "<size=" + (this.text.fontSize + Random.Range(-1, 1)) + ">" + textString[i].ToString() + "</size>";
			}
			yield return new WaitForSeconds(frameDelay);
		}
	}
}
