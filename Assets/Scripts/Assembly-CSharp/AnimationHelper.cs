using UnityEngine;

[RequireComponent(typeof(Animator))]
[DisallowMultipleComponent]
public class AnimationHelper : AbstractMonoBehaviour
{
	[SerializeField]
	private CupheadTime.Layer layer;

	[SerializeField]
	private float speed = 1f;

	[SerializeField]
	private bool ignoreGlobal;

	[SerializeField]
	private bool autoUpdate;

	public CupheadTime.Layer Layer
	{
		get
		{
			return layer;
		}
		set
		{
			layer = value;
			Set();
		}
	}

	public float LayerSpeed
	{
		get
		{
			return CupheadTime.GetLayerSpeed(Layer);
		}
		set
		{
			CupheadTime.SetLayerSpeed(Layer, value);
			Set();
		}
	}

	public float Speed
	{
		get
		{
			return speed;
		}
		set
		{
			speed = value;
			Set();
		}
	}

	public bool IgnoreGlobal
	{
		get
		{
			return ignoreGlobal;
		}
		set
		{
			ignoreGlobal = value;
			Set();
		}
	}

	protected override void Awake()
	{
		base.Awake();
		if (base.animator == null)
		{
			Debug.LogError("AnimationHelper needs Animator component");
			Object.Destroy(this);
		}
		else
		{
			CupheadTime.OnChangedEvent += Set;
			Set();
		}
	}

	protected override void Update()
	{
		base.Update();
		if (autoUpdate)
		{
			Set();
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		CupheadTime.OnChangedEvent -= Set;
	}

	protected void Set()
	{
		if (IgnoreGlobal)
		{
			base.animator.speed = Speed * LayerSpeed;
		}
		else
		{
			base.animator.speed = Speed * LayerSpeed * CupheadTime.GlobalSpeed;
		}
	}
}
