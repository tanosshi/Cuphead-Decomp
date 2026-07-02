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
	private Material coreBlitMaterial;

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
		if ((bool)coreBlitMaterial)
		{
			Object.DestroyImmediate(coreBlitMaterial);
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

		// 1. Render to a temporary texture (This completely stops the black window bug)
		RenderTexture temporary = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);
		Graphics.Blit(source, temporary, blurMaterial, 0);

		// 2. Initialize Unity's built-in copy shader if it isn't ready
		if (coreBlitMaterial == null)
		{
			Shader blitShader = Shader.Find("Hidden/BlitCopy");
			if (blitShader != null)
			{
				coreBlitMaterial = new Material(blitShader);
			}
		}

		// 3. Manually draw the screen quad with inverted UV coordinates to force it right-side up
		Graphics.SetRenderTarget(destination);
		GL.PushMatrix();
		GL.LoadOrtho();

		if (coreBlitMaterial != null)
		{
			coreBlitMaterial.SetTexture("_MainTex", temporary);
			coreBlitMaterial.SetPass(0);

			GL.Begin(GL.QUADS);
			// Explicitly invert the vertical texture mapping (Y axis) to fix the flip
			GL.TexCoord2(0f, 1f); GL.Vertex3(0f, 0f, 0f); // Bottom-Left
			GL.TexCoord2(1f, 1f); GL.Vertex3(1f, 0f, 0f); // Bottom-Right
			GL.TexCoord2(1f, 0f); GL.Vertex3(1f, 1f, 0f); // Top-Right
			GL.TexCoord2(0f, 0f); GL.Vertex3(0f, 1f, 0f); // Top-Left
			GL.End();
		}
		else
		{
			// Fallback if material initialization fails
			Graphics.Blit(temporary, destination);
		}

		GL.PopMatrix();
		RenderTexture.ReleaseTemporary(temporary);
	}
}