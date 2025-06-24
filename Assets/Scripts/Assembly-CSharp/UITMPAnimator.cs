using System.Collections;
using TMPro;
using UnityEngine;

public class UITMPAnimator : AbstractMonoBehaviour
{
	private TextMeshProUGUI text;

	protected override void Awake()
	{
		base.Awake();
		text = GetComponent<TextMeshProUGUI>();
		StartCoroutine(animateCharacters_cr());
		ignoreGlobalTime = true;
	}

	private IEnumerator animateCharacters_cr()
	{
		Mesh mesh = text.textInfo.meshInfo[0].mesh;
		text.havePropertiesChanged = true;
		while (true)
		{
			text.ForceMeshUpdate();
			TMP_TextInfo textInfo = text.textInfo;
			int characterCount = textInfo.characterCount;
			if (characterCount == 0)
			{
				continue;
			}
			for (int i = 0; i < characterCount; i++)
			{
				if (textInfo.characterInfo[i].isVisible)
				{
					Vector3 vector = new Vector3(Random.Range(-0.25f, 0.25f), Random.Range(-0.25f, 0.25f), 0f);
					int vertexIndex = textInfo.characterInfo[i].vertexIndex;
					int materialReferenceIndex = textInfo.characterInfo[i].materialReferenceIndex;
					Vector3[] vertices = textInfo.meshInfo[materialReferenceIndex].vertices;
					vertices[vertexIndex] += vector;
					vertices[vertexIndex + 1] += vector;
					vertices[vertexIndex + 2] += vector;
					vertices[vertexIndex + 3] += vector;
				}
			}
			text.UpdateVertexData();
			yield return new WaitForSeconds(0.07f);
		}
	}

	private IEnumerator updateTextLikeAMoron_cr()
	{
		text.transform.SetScale(1f, 1f, 0.99f);
		yield return null;
		text.transform.SetScale(1f, 1f, 1f);
	}
}
