using System;
using System.Collections;
using UnityEngine;

public class CircusPlatformingLevelHotdogProjectile : BasicProjectile
{
	private const string KetchupState = "Ketchup";

	private const string MustardState = "Mustard";

	private const string RelishState = "Relish";

	private const string Ketchup = "K";

	private const string Mustard = "M";

	private const string Relish = "R";

	[SerializeField]
	private float scaleFactor;

	[SerializeField]
	private SpriteRenderer[] renderers;

	[SerializeField]
	private Effect spark;

	private Collider2D collider2d;

	protected override float DestroyLifetime
	{
		get
		{
			return 20f;
		}
	}

	public event Action<CircusPlatformingLevelHotdogProjectile> OnDestroyCallback;

	protected override void Awake()
	{
		base.Awake();
		collider2d = GetComponent<Collider2D>();
	}

	protected override void Start()
	{
		base.Start();
		base.transform.localScale = new Vector3(0f, 1f, 1f);
		spark.Create(base.transform.position - new Vector3(10f, 0f, 0f));
		StartCoroutine(scaleOnStart_cr());
	}

	private IEnumerator scaleOnStart_cr()
	{
		while (base.transform.localScale.x < 1f)
		{
			base.transform.AddScale(scaleFactor * (float)CupheadTime.Delta);
			yield return null;
		}
		base.transform.SetScale(1f, 1f, 1f);
	}

	public void Side(bool isRight)
	{
		if (isRight)
		{
			for (int i = 0; i < renderers.Length; i++)
			{
				renderers[i].sortingOrder += 3;
			}
		}
	}

	public void SetCondiment(string type)
	{
		switch (type)
		{
		case "K":
			animator.Play("Ketchup");
			break;
		case "M":
			animator.Play("Mustard");
			break;
		case "R":
			animator.Play("Relish");
			break;
		}
	}

	public void EnableCollider(bool enable)
	{
		if (collider2d != null)
		{
			collider2d.enabled = enable;
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (this.OnDestroyCallback != null)
		{
			this.OnDestroyCallback(this);
		}
	}
}
