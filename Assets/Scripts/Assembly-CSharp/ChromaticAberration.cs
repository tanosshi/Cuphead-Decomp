using UnityEngine;

[ExecuteInEditMode]
public class ChromaticAberration : MonoBehaviour
{
	public Shader shader;

	public Vector2 r;

	public Vector2 g;

	public Vector2 b;

	private Material curMaterial;

	private Material material
	{
		get
		{
			if (curMaterial == null)
			{
				curMaterial = new Material(shader);
				curMaterial.hideFlags = HideFlags.HideAndDontSave;
			}
			return curMaterial;
		}
	}

	protected virtual void Start()
	{
		if (!SystemInfo.supportsImageEffects)
		{
			base.enabled = false;
		}
	}

	protected virtual void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
	{
		if (shader != null)
		{
			float num = (float)destTexture.width / (float)destTexture.height;
			float num2 = ((!(num < 1.7777778f)) ? 1f : (num / 1.7777778f));
			num2 *= 1f - 0.1f * SettingsData.Data.overscan;
			float num3 = num2 * (float)destTexture.height / 1080f;
			material.SetVector("_Screen", new Vector2(destTexture.width, destTexture.height));
			material.SetVector("_Red", r * num3);
			material.SetVector("_Green", g * num3);
			material.SetVector("_Blue", b * num3);
			Graphics.Blit(sourceTexture, destTexture, material);
		}
		else
		{
			Graphics.Blit(sourceTexture, destTexture);
		}
	}

	protected virtual void OnDisable()
	{
		if ((bool)curMaterial)
		{
			Object.DestroyImmediate(curMaterial);
		}
	}
}
