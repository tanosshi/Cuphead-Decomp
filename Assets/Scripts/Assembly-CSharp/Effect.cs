using System;
using UnityEngine;

[Serializable]
public class Effect : AbstractCollidableObject
{
	[SerializeField]
	protected bool randomRotation;

	[Space(10f)]
	[SerializeField]
	protected bool randomMirrorX;

	[SerializeField]
	protected bool randomMirrorY;

	protected new Animator animator;

	public virtual Effect Create(Vector3 position)
	{
		return Create(position, Vector3.one);
	}

	public virtual Effect Create(Vector3 position, Vector3 scale)
	{
		Effect component = UnityEngine.Object.Instantiate(base.gameObject).GetComponent<Effect>();
		component.name = component.name.Replace("(Clone)", string.Empty);
		if (randomMirrorX)
		{
			scale.x = ((!Rand.Bool()) ? (0f - scale.x) : scale.x);
		}
		if (randomMirrorY)
		{
			scale.y = ((!Rand.Bool()) ? (0f - scale.y) : scale.y);
		}
		component.Initialize(position, scale, randomRotation);
		return component;
	}

	protected virtual void Initialize(Vector3 position, Vector3 scale, bool randomR)
	{
		animator = GetComponent<Animator>();
		int value = UnityEngine.Random.Range(0, animator.GetInteger("Count"));
		animator.SetInteger("Effect", value);
		Transform transform = base.transform;
		transform.position = position;
		transform.localScale = scale;
		if (randomR)
		{
			transform.eulerAngles = new Vector3(0f, 0f, UnityEngine.Random.Range(0f, 360f));
		}
	}

	protected virtual void OnEffectComplete()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public void Play()
	{
		animator.Play("A");
	}

	private void PlaySound(Sfx sfx)
	{
		AudioManager.Play(sfx);
	}
}
