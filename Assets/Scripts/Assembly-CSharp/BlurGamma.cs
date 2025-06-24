using UnityEngine;
using UnityStandardAssets.ImageEffects;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class BlurGamma : PostEffectsBase
{
	public enum Filter
	{
		None = 0,
		TwoStrip = 1,
		BW = 2
	}

	[Range(0f, 10f)]
	public float blurSize = 3f;

	[Range(1f, 4f)]
	public int blurIterations = 2;

	public Shader blurShader;

	private Material blurMaterial;

	public override bool CheckResources()
	{
		CheckSupport(false);
		blurMaterial = CheckShaderAndCreateMaterial(blurShader, blurMaterial);
		if (!isSupported)
		{
			ReportAutoDisable();
		}
		return isSupported;
	}

	public void OnDisable()
	{
		if ((bool)blurMaterial)
		{
			Object.DestroyImmediate(blurMaterial);
		}
	}

	public void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (!CheckResources())
		{
			Graphics.Blit(source, destination);
			return;
		}
		float num = (float)source.width / (float)source.height;
		float num2 = ((!(num < 1.7777778f)) ? 1f : (num / 1.7777778f));
		num2 *= 1f - 0.1f * SettingsData.Data.overscan;
		float num3 = (float)source.height / 1080f;
		num3 *= num2;
		if (SettingsData.Data.filter == Filter.BW)
		{
			num3 *= 1.35f;
		}
		blurMaterial.SetVector("_Parameter", new Vector4(blurSize * num3, (0f - blurSize) * num3, Mathf.Pow(1.4f, 0f - SettingsData.Data.Brightness), 0f));
		source.filterMode = FilterMode.Bilinear;
		RenderTexture temporary = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);
		Graphics.Blit(source, temporary, blurMaterial, 0);
		int num4 = 1;
		switch (SettingsData.Data.filter)
		{
		case Filter.TwoStrip:
			num4++;
			break;
		case Filter.BW:
			num4 += 2;
			break;
		}
		Graphics.Blit(temporary, destination, blurMaterial, num4);
		RenderTexture.ReleaseTemporary(temporary);
	}
}
