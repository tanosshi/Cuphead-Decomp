using System.Collections;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Other/Screen Overlay Animated")]
public class ScreenOverlayAnimated : PostEffectsBase
{
	public enum OverlayBlendMode
	{
		Additive = 0,
		ScreenBlend = 1,
		Multiply = 2,
		Overlay = 3,
		AlphaBlend = 4
	}

	private const float FRAME_TIME = 0.025f;

	private Vector4 UV_Transform = new Vector4(1f, 0f, 0f, 1f);

	public OverlayBlendMode blendMode = OverlayBlendMode.Overlay;

	public float intensity = 1f;

	public bool animated = true;

	public Texture2D[] textures;

	public Shader overlayShader;

	private int currentTexture;

	private Material overlayMaterial;

	protected override void Start()
	{
		StartCoroutine(animate_cr());
	}

	private IEnumerator animate_cr()
	{
		while (true)
		{
			yield return new WaitForSeconds(0.025f);
			if (animated)
			{
				currentTexture++;
				if (currentTexture >= textures.Length)
				{
					currentTexture = 0;
				}
			}
		}
	}

	public override bool CheckResources()
	{
		CheckSupport(false);
		overlayMaterial = CheckShaderAndCreateMaterial(overlayShader, overlayMaterial);
		if (!isSupported)
		{
			ReportAutoDisable();
		}
		return isSupported;
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (!CheckResources())
		{
			Graphics.Blit(source, destination);
			return;
		}
		overlayMaterial.SetVector("_UV_Transform", UV_Transform);
		overlayMaterial.SetFloat("_Intensity", intensity);
		if (textures != null && textures.Length > currentTexture && textures[currentTexture] != null)
		{
			overlayMaterial.SetTexture("_Overlay", textures[currentTexture]);
		}
		Graphics.Blit(source, destination, overlayMaterial, (int)blendMode);
	}
}
