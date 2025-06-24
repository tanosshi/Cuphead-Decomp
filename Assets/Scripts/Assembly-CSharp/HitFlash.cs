using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitFlash : AbstractMonoBehaviour
{
	public class RendererProperties
	{
		public readonly SpriteRenderer renderer;

		public readonly Color normalColor;

		public readonly Transform transform;

		public readonly Vector3 scale;

		public RendererProperties(SpriteRenderer r)
		{
			renderer = r;
			normalColor = r.color;
			transform = r.transform;
			scale = r.transform.localScale;
		}
	}

	[SerializeField]
	private Color damageColor = new Color(1f, 0f, 0f, 1f);

	[SerializeField]
	private DamageReceiver damageReceiver;

	[SerializeField]
	private bool includeSelf = true;

	public SpriteRenderer[] otherRenderers;

	private float time;

	private Coroutine coroutine;

	private RendererProperties self;

	private List<RendererProperties> renderers;

	public bool flashing { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		if (includeSelf)
		{
			SpriteRenderer component = GetComponent<SpriteRenderer>();
			if (component != null)
			{
				self = new RendererProperties(component);
			}
		}
		renderers = new List<RendererProperties>();
		for (int i = 0; i < otherRenderers.Length; i++)
		{
			renderers.Add(new RendererProperties(otherRenderers[i]));
		}
		if (damageReceiver == null)
		{
			damageReceiver = GetComponent<DamageReceiver>();
		}
		damageReceiver.OnDamageTaken += OnDamageTaken;
	}

	protected override void Update()
	{
		base.Update();
		time -= CupheadTime.Delta;
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		Flash();
	}

	public override void StopAllCoroutines()
	{
		base.StopAllCoroutines();
		SetColor(1f);
		SetScale(Vector3.one, 1f);
		time = 0f;
		flashing = false;
	}

	public void Flash()
	{
		time = 0.1f;
		if (!flashing && base.gameObject.activeSelf && base.gameObject.activeInHierarchy)
		{
			StartCoroutine(flash_cr());
		}
	}

	public void SetColor(float t)
	{
		if (self != null)
		{
			Color color = Color.Lerp(self.normalColor, damageColor, t);
			self.renderer.color = color;
		}
		foreach (RendererProperties renderer in renderers)
		{
			Color color2 = Color.Lerp(renderer.normalColor, damageColor, t);
			renderer.renderer.color = color2;
		}
	}

	private void SetScale(Vector3 original, float s)
	{
		if (self != null)
		{
			self.transform.localScale = original * s;
		}
		foreach (RendererProperties renderer in renderers)
		{
			renderer.transform.localScale = renderer.scale * s;
		}
	}

	private IEnumerator flash_cr()
	{
		flashing = true;
		while (time > 0f)
		{
			SetColor(1f);
			yield return CupheadTime.WaitForSeconds(this, 0.0416f);
			SetColor(0f);
			yield return CupheadTime.WaitForSeconds(this, 0.0832f);
		}
		flashing = false;
	}
}
