using System.Collections;
using TMPro;
using UnityEngine;

public class TextMeshCurveAndJitter : MonoBehaviour
{
	[SerializeField]
	private TMP_Text m_TextComponent;

	public AnimationCurve VertexCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.25f, 2f), new Keyframe(0.5f, 0f), new Keyframe(0.75f, 2f), new Keyframe(1f, 0f));

	public float AngleMultiplier = 1f;

	public float SpeedMultiplier = 1f;

	public float CurveScale = 1f;

	public float jitterAmplitude = 0.1f;

	public float jitterAngleAmplitude = 0.1f;

	private float jitterDelay = 0.1f;

	private float currentJitterDelay;

	private void Awake()
	{
		jitterDelay = 1f / 12f;
		m_TextComponent = base.gameObject.GetComponent<TMP_Text>();
	}

	private void Start()
	{
		StartCoroutine(WarpText());
	}

	private AnimationCurve CopyAnimationCurve(AnimationCurve curve)
	{
		AnimationCurve animationCurve = new AnimationCurve();
		animationCurve.keys = curve.keys;
		return animationCurve;
	}

	private IEnumerator WarpText()
	{
		VertexCurve.preWrapMode = WrapMode.Once;
		VertexCurve.postWrapMode = WrapMode.Once;
		m_TextComponent.havePropertiesChanged = true;
		float old_CurveScale = CurveScale;
		AnimationCurve old_curve = CopyAnimationCurve(VertexCurve);
		bool jitter = true;
		while (true)
		{
			currentJitterDelay -= CupheadTime.Delta;
			if (currentJitterDelay <= 0f)
			{
				jitter = true;
				currentJitterDelay = jitterDelay;
			}
			ApplyCurveAndJitter(jitter);
			old_CurveScale = CurveScale;
			old_curve = CopyAnimationCurve(VertexCurve);
			yield return null;
		}
	}

	public void ApplyCurveAndJitter(bool jitter)
	{
		m_TextComponent.ForceMeshUpdate();
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
				Vector3 vector = new Vector2((vertices[vertexIndex].x + vertices[vertexIndex + 2].x) / 2f, textInfo.characterInfo[i].baseLine);
				vertices[vertexIndex] += -vector;
				vertices[vertexIndex + 1] += -vector;
				vertices[vertexIndex + 2] += -vector;
				vertices[vertexIndex + 3] += -vector;
				float num = (vector.x - x) / (x2 - x);
				float num2 = num + 0.0001f;
				float y = VertexCurve.Evaluate(num) * CurveScale;
				float y2 = VertexCurve.Evaluate(num2) * CurveScale;
				Vector3 lhs = new Vector3(1f, 0f, 0f);
				Vector3 rhs = new Vector3(num2 * (x2 - x) + x, y2) - new Vector3(vector.x, y);
				float num3 = Mathf.Acos(Vector3.Dot(lhs, rhs.normalized)) * 57.29578f;
				float num4 = ((!(Vector3.Cross(lhs, rhs).z > 0f)) ? (360f - num3) : num3);
				float num5 = 0f;
				if (jitter)
				{
					num5 = Random.Range(0f - jitterAngleAmplitude, jitterAngleAmplitude);
				}
				Matrix4x4 matrix4x = Matrix4x4.TRS(new Vector3(0f, y, 0f), Quaternion.Euler(0f, 0f, num4 + num5), Vector3.one);
				vertices[vertexIndex] = matrix4x.MultiplyPoint3x4(vertices[vertexIndex]);
				vertices[vertexIndex + 1] = matrix4x.MultiplyPoint3x4(vertices[vertexIndex + 1]);
				vertices[vertexIndex + 2] = matrix4x.MultiplyPoint3x4(vertices[vertexIndex + 2]);
				vertices[vertexIndex + 3] = matrix4x.MultiplyPoint3x4(vertices[vertexIndex + 3]);
				vertices[vertexIndex] += vector;
				vertices[vertexIndex + 1] += vector;
				vertices[vertexIndex + 2] += vector;
				vertices[vertexIndex + 3] += vector;
				Vector3 vector2 = Vector3.zero;
				if (jitter)
				{
					vector2 = new Vector3(Random.Range(0f - jitterAmplitude, jitterAmplitude), Random.Range(0f - jitterAmplitude, jitterAmplitude), 0f);
				}
				vertices[vertexIndex] += vector2;
				vertices[vertexIndex + 1] += vector2;
				vertices[vertexIndex + 2] += vector2;
				vertices[vertexIndex + 3] += vector2;
			}
		}
		jitter = false;
		m_TextComponent.UpdateVertexData();
	}
}
