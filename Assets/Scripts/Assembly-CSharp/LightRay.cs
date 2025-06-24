using UnityEngine;

public class LightRay : AbstractMonoBehaviour
{
	private float t;

	private float accumulator;

	private float textureWidth;

	[SerializeField]
	private float speed = 0.03f;

	[SerializeField]
	private bool randomOffset = true;

	[SerializeField]
	[Range(0f, 1f)]
	private float customOffset = 0.5f;

	protected override void Start()
	{
		base.Start();
		float num = ((!randomOffset) ? customOffset : Random.Range(0f, 1f));
		t = 4f * num / speed;
		Texture2D texture = GetComponent<SpriteRenderer>().sprite.texture;
		float num2 = GetComponent<SpriteRenderer>().sprite.texture.width;
		float num3 = GetComponent<SpriteRenderer>().sprite.texture.height;
		textureWidth = 2.3232484f / ((float)texture.width / (float)texture.height);
	}

	protected override void Update()
	{
		base.Update();
		accumulator += CupheadTime.Delta;
		while (accumulator > 1f / 24f)
		{
			accumulator -= 1f / 24f;
			t += 1f / 24f;
		}
		Material material = GetComponent<SpriteRenderer>().material;
		material.SetFloat("t", t);
		material.SetFloat("textureWidth", textureWidth);
		material.SetFloat("textureSpeed", speed);
	}
}
