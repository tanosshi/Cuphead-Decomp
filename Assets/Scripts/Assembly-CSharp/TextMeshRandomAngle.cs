using TMPro;
using UnityEngine;

public class TextMeshRandomAngle : MonoBehaviour
{
	[SerializeField]
	private TMP_Text m_TextComponent;

	[SerializeField]
	private TMP_Text m_ShadowTextComponent;

	public float m_AngleAmplitude = 5f;

	public float m_JitterAngleAmplitude = 0.7f;

	public float m_JitterOffsetAmplitude = 0.1f;

	private float[] initialAngles;

	private float[] jitterAngles;

	private float jitterDelay = 0.1f;

	private float currentJitterDelay;

	private void Start()
	{
		initialAngles = new float[m_TextComponent.text.Length];
		jitterAngles = new float[m_TextComponent.text.Length];
		for (int i = 0; i < initialAngles.Length; i++)
		{
			initialAngles[i] = Random.Range(0f - m_AngleAmplitude, m_AngleAmplitude);
		}
		jitterDelay = 1f / 12f;
		ApplyRotation();
	}

	private void Update()
	{
		currentJitterDelay -= CupheadTime.Delta;
		if (!(currentJitterDelay > 0f))
		{
			currentJitterDelay = jitterDelay;
			for (int i = 0; i < initialAngles.Length; i++)
			{
				jitterAngles[i] = Random.Range(0f - m_JitterAngleAmplitude, m_JitterAngleAmplitude);
			}
			ApplyRotation();
		}
	}

	private void ApplyRotation()
	{
		m_TextComponent.havePropertiesChanged = true;
		m_ShadowTextComponent.havePropertiesChanged = true;
		m_TextComponent.ForceMeshUpdate();
		m_ShadowTextComponent.ForceMeshUpdate();
		TMP_TextInfo textInfo = m_TextComponent.textInfo;
		int characterCount = textInfo.characterCount;
		if (characterCount == 0 || m_TextComponent.text.Length == 0)
		{
			return;
		}
		float x = m_TextComponent.bounds.min.x;
		float x2 = m_TextComponent.bounds.max.x;
		for (int i = 0; i < characterCount; i++)
		{
			if (textInfo.characterInfo[i].isVisible)
			{
				int vertexIndex = textInfo.characterInfo[i].vertexIndex;
				int materialReferenceIndex = textInfo.characterInfo[i].materialReferenceIndex;
				Vector3[] vertices = textInfo.meshInfo[materialReferenceIndex].vertices;
				Vector3 vector = new Vector2((vertices[vertexIndex].x + vertices[vertexIndex + 2].x) / 2f, (vertices[vertexIndex].y + vertices[vertexIndex + 2].y) / 2f);
				vertices[vertexIndex] += -vector;
				vertices[vertexIndex + 1] += -vector;
				vertices[vertexIndex + 2] += -vector;
				vertices[vertexIndex + 3] += -vector;
				float z = initialAngles[i] + jitterAngles[i];
				Matrix4x4 matrix4x = Matrix4x4.TRS(new Vector3(0f, 0f, 0f), Quaternion.Euler(0f, 0f, z), Vector3.one);
				vertices[vertexIndex] = matrix4x.MultiplyPoint3x4(vertices[vertexIndex]);
				vertices[vertexIndex + 1] = matrix4x.MultiplyPoint3x4(vertices[vertexIndex + 1]);
				vertices[vertexIndex + 2] = matrix4x.MultiplyPoint3x4(vertices[vertexIndex + 2]);
				vertices[vertexIndex + 3] = matrix4x.MultiplyPoint3x4(vertices[vertexIndex + 3]);
				vector += new Vector3(Random.Range(0f - m_JitterOffsetAmplitude, m_JitterOffsetAmplitude), Random.Range(0f - m_JitterOffsetAmplitude, m_JitterOffsetAmplitude), 0f);
				vertices[vertexIndex] += vector;
				vertices[vertexIndex + 1] += vector;
				vertices[vertexIndex + 2] += vector;
				vertices[vertexIndex + 3] += vector;
				m_ShadowTextComponent.textInfo.meshInfo[materialReferenceIndex].vertices = vertices;
			}
		}
		m_TextComponent.UpdateVertexData();
		m_ShadowTextComponent.UpdateVertexData();
	}
}
