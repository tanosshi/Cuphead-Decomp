using System;
using System.Collections;
using UnityEngine;

public abstract class AbstractMonoBehaviour : MonoBehaviour
{
	public delegate void TweenUpdateHandler(float value);

	private Transform _transform;

	private RectTransform _rectTransform;

	private Rigidbody _rigidbody;

	private Rigidbody2D _rigidbody2D;

	private Animator _animator;

	protected bool ignoreGlobalTime;

	protected CupheadTime.Layer timeLayer;

	public Transform baseTransform
	{
		get
		{
			return base.transform;
		}
	}

	public new Transform transform
	{
		get
		{
			if (_transform == null)
			{
				_transform = baseTransform;
			}
			return _transform;
		}
	}

	public RectTransform baseRectTransform
	{
		get
		{
			return base.transform as RectTransform;
		}
	}

	public RectTransform rectTransform
	{
		get
		{
			if (_rectTransform == null)
			{
				_rectTransform = baseRectTransform;
			}
			return _rectTransform;
		}
	}

	public Rigidbody baseRigidbody
	{
		get
		{
			return GetComponent<Rigidbody>();
		}
	}

	public Rigidbody rigidbody
	{
		get
		{
			if (_rigidbody == null)
			{
				_rigidbody = baseRigidbody;
			}
			return _rigidbody;
		}
	}

	public Rigidbody2D baseRigidbody2D
	{
		get
		{
			return GetComponent<Rigidbody2D>();
		}
	}

	public Rigidbody2D rigidbody2D
	{
		get
		{
			if (_rigidbody2D == null)
			{
				_rigidbody2D = baseRigidbody2D;
			}
			return _rigidbody2D;
		}
	}

	public Animator baseAnimator
	{
		get
		{
			return GetComponent<Animator>();
		}
	}

	public Animator animator
	{
		get
		{
			if (_animator == null)
			{
				_animator = baseAnimator;
			}
			return _animator;
		}
	}

	protected float LocalDeltaTime
	{
		get
		{
			if (ignoreGlobalTime)
			{
				return Time.deltaTime;
			}
			return CupheadTime.Delta[timeLayer];
		}
	}

	protected virtual void Awake()
	{
		base.useGUILayout = false;
	}

	protected virtual void Start()
	{
	}

	protected virtual void Reset()
	{
	}

	protected virtual void OnEnable()
	{
	}

	protected virtual void OnDisable()
	{
	}

	protected virtual void OnDestroy()
	{
	}

	protected virtual void OnValidate()
	{
	}

	protected virtual void Update()
	{
	}

	protected virtual void FixedUpdate()
	{
	}

	protected virtual void LateUpdate()
	{
	}

	protected virtual void OnMouseDown()
	{
	}

	protected virtual void OnMouseDrag()
	{
	}

	protected virtual void OnMouseEnter()
	{
	}

	protected virtual void OnMouseExit()
	{
	}

	protected virtual void OnMouseOver()
	{
	}

	protected virtual void OnMouseUp()
	{
	}

	protected virtual void OnMouseUpAsButton()
	{
	}

	protected virtual void OnCollisionEnter(Collision collision)
	{
	}

	protected virtual void OnCollisionExit(Collision collision)
	{
	}

	protected virtual void OnCollisionStay(Collision collision)
	{
	}

	protected virtual void OnTriggerEnter(Collider collider)
	{
	}

	protected virtual void OnTriggerExit(Collider collider)
	{
	}

	protected virtual void OnTriggerStay(Collider collider)
	{
	}

	protected virtual void OnCollisionEnter2D(Collision2D collision)
	{
	}

	protected virtual void OnCollisionExit2D(Collision2D collision)
	{
	}

	protected virtual void OnCollisionStay2D(Collision2D collision)
	{
	}

	protected virtual void OnTriggerEnter2D(Collider2D collider)
	{
	}

	protected virtual void OnTriggerExit2D(Collider2D collider)
	{
	}

	protected virtual void OnTriggerStay2D(Collider2D collider)
	{
	}

	protected virtual void OnApplicationFocus(bool focusStatus)
	{
	}

	protected virtual void OnApplicationPause(bool pauseStatus)
	{
	}

	protected virtual void OnApplicationQuit()
	{
	}

	protected virtual void OnWillRenderObject()
	{
	}

	protected virtual void OnPostRender()
	{
	}

	protected virtual void OnPreRender()
	{
	}

	protected virtual void OnRenderObject()
	{
	}

	protected virtual void OnDrawGizmos()
	{
	}

	protected virtual void OnDrawGizmosSelected()
	{
	}

	public virtual T InstantiatePrefab<T>() where T : MonoBehaviour
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(base.gameObject);
		gameObject.name = gameObject.name.Replace("(Clone)", string.Empty);
		return gameObject.GetComponent<T>();
	}

	public Coroutine FrameDelayedCallback(Action callback, int frames)
	{
		return StartCoroutine(frameDelayedCallback_cr(callback, frames));
	}

	public IEnumerator frameDelayedCallback_cr(Action callback, int frames)
	{
		for (int i = 0; i < frames; i++)
		{
			yield return null;
		}
		if (callback != null)
		{
			callback();
		}
	}

	public Coroutine TweenValue(float start, float end, float time, EaseUtils.EaseType ease, TweenUpdateHandler updateCallback)
	{
		return StartCoroutine(tweenValue_cr(start, end, time, ease, updateCallback));
	}

	protected IEnumerator tweenValue_cr(float start, float end, float time, EaseUtils.EaseType ease, TweenUpdateHandler updateCallback)
	{
		float t = 0f;
		while (t < time)
		{
			float val = t / time;
			if (updateCallback != null)
			{
				updateCallback(EaseUtils.Ease(ease, start, end, val));
			}
			t += LocalDeltaTime;
			yield return null;
		}
		if (updateCallback != null)
		{
			updateCallback(end);
		}
		yield return null;
	}

	public Coroutine TweenScale(Vector2 start, Vector2 end, float time, EaseUtils.EaseType ease)
	{
		return StartCoroutine(tweenScale_cr(start, end, time, ease));
	}

	public Coroutine TweenScale(Vector2 start, Vector2 end, float time, EaseUtils.EaseType ease, TweenUpdateHandler updateCallback)
	{
		return StartCoroutine(tweenScale_cr(start, end, time, ease, updateCallback));
	}

	private IEnumerator tweenScale_cr(Vector2 start, Vector2 end, float time, EaseUtils.EaseType ease, TweenUpdateHandler updateCallback = null)
	{
		transform.SetScale(start.x, start.y);
		float t = 0f;
		while (t < time)
		{
			float val = t / time;
			float x = EaseUtils.Ease(ease, start.x, end.x, val);
			TransformExtensions.SetScale(y: EaseUtils.Ease(ease, start.y, end.y, val), transform: transform, x: x);
			if (updateCallback != null)
			{
				updateCallback(val);
			}
			t += LocalDeltaTime;
			yield return null;
		}
		transform.SetScale(end.x, end.y);
		if (updateCallback != null)
		{
			updateCallback(1f);
		}
		yield return null;
	}

	public Coroutine TweenPosition(Vector2 start, Vector2 end, float time, EaseUtils.EaseType ease)
	{
		return StartCoroutine(tweenPosition_cr(start, end, time, ease));
	}

	public Coroutine TweenPosition(Vector2 start, Vector2 end, float time, EaseUtils.EaseType ease, TweenUpdateHandler updateCallback)
	{
		return StartCoroutine(tweenPosition_cr(start, end, time, ease, updateCallback));
	}

	private IEnumerator tweenPosition_cr(Vector2 start, Vector2 end, float time, EaseUtils.EaseType ease, TweenUpdateHandler updateCallback = null)
	{
		transform.position = start;
		float t = 0f;
		while (t < time)
		{
			float val = t / time;
			float x = EaseUtils.Ease(ease, start.x, end.x, val);
			TransformExtensions.SetPosition(y: EaseUtils.Ease(ease, start.y, end.y, val), transform: transform, x: x, z: 0f);
			if (updateCallback != null)
			{
				updateCallback(val);
			}
			t += LocalDeltaTime;
			yield return null;
		}
		transform.position = end;
		if (updateCallback != null)
		{
			updateCallback(1f);
		}
		yield return null;
	}

	public Coroutine TweenLocalPosition(Vector2 start, Vector2 end, float time, EaseUtils.EaseType ease)
	{
		return StartCoroutine(tweenLocalPosition_cr(start, end, time, ease));
	}

	public Coroutine TweenLocalPosition(Vector2 start, Vector2 end, float time, EaseUtils.EaseType ease, TweenUpdateHandler updateCallback)
	{
		return StartCoroutine(tweenLocalPosition_cr(start, end, time, ease, updateCallback));
	}

	private IEnumerator tweenLocalPosition_cr(Vector2 start, Vector2 end, float time, EaseUtils.EaseType ease, TweenUpdateHandler updateCallback = null)
	{
		transform.localPosition = start;
		float t = 0f;
		while (t < time)
		{
			float val = t / time;
			float x = EaseUtils.Ease(ease, start.x, end.x, val);
			TransformExtensions.SetLocalPosition(y: EaseUtils.Ease(ease, start.y, end.y, val), transform: transform, x: x, z: 0f);
			if (updateCallback != null)
			{
				updateCallback(val);
			}
			t += LocalDeltaTime;
			yield return null;
		}
		transform.localPosition = end;
		if (updateCallback != null)
		{
			updateCallback(1f);
		}
		yield return null;
	}

	public Coroutine TweenPositionX(float start, float end, float time, EaseUtils.EaseType ease)
	{
		return StartCoroutine(tweenPositionX_cr(start, end, time, ease));
	}

	public Coroutine TweenPositionX(float start, float end, float time, EaseUtils.EaseType ease, TweenUpdateHandler updateCallback)
	{
		return StartCoroutine(tweenPositionX_cr(start, end, time, ease, updateCallback));
	}

	private IEnumerator tweenPositionX_cr(float start, float end, float time, EaseUtils.EaseType ease, TweenUpdateHandler updateCallback = null)
	{
		transform.SetPosition(start);
		float t = 0f;
		while (t < time)
		{
			float val = t / time;
			transform.SetPosition(EaseUtils.Ease(ease, start, end, val));
			if (updateCallback != null)
			{
				updateCallback(val);
			}
			t += LocalDeltaTime;
			yield return null;
		}
		transform.SetPosition(end);
		if (updateCallback != null)
		{
			updateCallback(1f);
		}
		yield return null;
	}

	public Coroutine TweenLocalPositionX(float start, float end, float time, EaseUtils.EaseType ease)
	{
		return StartCoroutine(tweenLocalPositionX_cr(start, end, time, ease));
	}

	public Coroutine TweenLocalPositionX(float start, float end, float time, EaseUtils.EaseType ease, TweenUpdateHandler updateCallback)
	{
		return StartCoroutine(tweenLocalPositionX_cr(start, end, time, ease, updateCallback));
	}

	private IEnumerator tweenLocalPositionX_cr(float start, float end, float time, EaseUtils.EaseType ease, TweenUpdateHandler updateCallback = null)
	{
		transform.SetLocalPosition(start);
		float t = 0f;
		while (t < time)
		{
			float val = t / time;
			transform.SetLocalPosition(EaseUtils.Ease(ease, start, end, val));
			if (updateCallback != null)
			{
				updateCallback(val);
			}
			t += LocalDeltaTime;
			yield return null;
		}
		transform.SetLocalPosition(end);
		if (updateCallback != null)
		{
			updateCallback(1f);
		}
		yield return null;
	}

	public Coroutine TweenPositionY(float start, float end, float time, EaseUtils.EaseType ease)
	{
		return StartCoroutine(tweenPositionY_cr(start, end, time, ease));
	}

	public Coroutine TweenPositionY(float start, float end, float time, EaseUtils.EaseType ease, TweenUpdateHandler updateCallback)
	{
		return StartCoroutine(tweenPositionY_cr(start, end, time, ease, updateCallback));
	}

	private IEnumerator tweenPositionY_cr(float start, float end, float time, EaseUtils.EaseType ease, TweenUpdateHandler updateCallback = null)
	{
		transform.SetPosition(null, start);
		float t = 0f;
		while (t < time)
		{
			float val = t / time;
			transform.SetPosition(null, EaseUtils.Ease(ease, start, end, val));
			if (updateCallback != null)
			{
				updateCallback(val);
			}
			t += LocalDeltaTime;
			yield return null;
		}
		transform.SetPosition(null, end);
		if (updateCallback != null)
		{
			updateCallback(1f);
		}
		yield return null;
	}

	public Coroutine TweenLocalPositionY(float start, float end, float time, EaseUtils.EaseType ease)
	{
		return StartCoroutine(tweenLocalPositionY_cr(start, end, time, ease));
	}

	public Coroutine TweenLocalPositionY(float start, float end, float time, EaseUtils.EaseType ease, TweenUpdateHandler updateCallback)
	{
		return StartCoroutine(tweenLocalPositionY_cr(start, end, time, ease, updateCallback));
	}

	private IEnumerator tweenLocalPositionY_cr(float start, float end, float time, EaseUtils.EaseType ease, TweenUpdateHandler updateCallback = null)
	{
		transform.SetLocalPosition(null, start);
		float t = 0f;
		while (t < time)
		{
			float val = t / time;
			transform.SetLocalPosition(null, EaseUtils.Ease(ease, start, end, val));
			if (updateCallback != null)
			{
				updateCallback(val);
			}
			t += LocalDeltaTime;
			yield return null;
		}
		transform.SetLocalPosition(null, end);
		if (updateCallback != null)
		{
			updateCallback(1f);
		}
		yield return null;
	}

	public Coroutine TweenPositionZ(float start, float end, float time, EaseUtils.EaseType ease)
	{
		return StartCoroutine(tweenPositionZ_cr(start, end, time, ease));
	}

	public Coroutine TweenPositionZ(float start, float end, float time, EaseUtils.EaseType ease, TweenUpdateHandler updateCallback)
	{
		return StartCoroutine(tweenPositionZ_cr(start, end, time, ease, updateCallback));
	}

	private IEnumerator tweenPositionZ_cr(float start, float end, float time, EaseUtils.EaseType ease, TweenUpdateHandler updateCallback = null)
	{
		transform.SetPosition(null, null, start);
		float t = 0f;
		while (t < time)
		{
			float val = t / time;
			transform.SetPosition(null, null, EaseUtils.Ease(ease, start, end, val));
			if (updateCallback != null)
			{
				updateCallback(val);
			}
			t += LocalDeltaTime;
			yield return null;
		}
		transform.SetPosition(null, null, end);
		if (updateCallback != null)
		{
			updateCallback(1f);
		}
		yield return null;
	}

	public Coroutine TweenLocalPositionZ(float start, float end, float time, EaseUtils.EaseType ease)
	{
		return StartCoroutine(tweenLocalPositionZ_cr(start, end, time, ease));
	}

	public Coroutine TweenLocalPositionZ(float start, float end, float time, EaseUtils.EaseType ease, TweenUpdateHandler updateCallback)
	{
		return StartCoroutine(tweenLocalPositionZ_cr(start, end, time, ease, updateCallback));
	}

	private IEnumerator tweenLocalPositionZ_cr(float start, float end, float time, EaseUtils.EaseType ease, TweenUpdateHandler updateCallback = null)
	{
		transform.SetLocalPosition(null, null, start);
		float t = 0f;
		while (t < time)
		{
			float val = t / time;
			transform.SetLocalPosition(null, null, EaseUtils.Ease(ease, start, end, val));
			if (updateCallback != null)
			{
				updateCallback(val);
			}
			t += LocalDeltaTime;
			yield return null;
		}
		transform.SetLocalPosition(null, null, end);
		if (updateCallback != null)
		{
			updateCallback(1f);
		}
		yield return null;
	}

	public Coroutine TweenRotation2D(float start, float end, float time, EaseUtils.EaseType ease)
	{
		return StartCoroutine(tweenRotation2D_cr(start, end, time, ease));
	}

	public Coroutine TweenRotation2D(float start, float end, float time, EaseUtils.EaseType ease, TweenUpdateHandler updateCallback)
	{
		return StartCoroutine(tweenRotation2D_cr(start, end, time, ease, updateCallback));
	}

	private IEnumerator tweenRotation2D_cr(float start, float end, float time, EaseUtils.EaseType ease, TweenUpdateHandler updateCallback = null)
	{
		transform.SetEulerAngles(null, null, start);
		float t = 0f;
		while (t < time)
		{
			float val = t / time;
			transform.SetEulerAngles(null, null, EaseUtils.Ease(ease, start, end, val));
			if (updateCallback != null)
			{
				updateCallback(val);
			}
			t += LocalDeltaTime;
			yield return null;
		}
		transform.SetEulerAngles(null, null, end);
		if (updateCallback != null)
		{
			updateCallback(1f);
		}
		yield return null;
	}

	public Coroutine TweenLocalRotation2D(float start, float end, float time, EaseUtils.EaseType ease)
	{
		return StartCoroutine(tweenLocalRotation2D_cr(start, end, time, ease));
	}

	public Coroutine TweenLocalRotation2D(float start, float end, float time, EaseUtils.EaseType ease, TweenUpdateHandler updateCallback)
	{
		return StartCoroutine(tweenLocalRotation2D_cr(start, end, time, ease, updateCallback));
	}

	private IEnumerator tweenLocalRotation2D_cr(float start, float end, float time, EaseUtils.EaseType ease, TweenUpdateHandler updateCallback = null)
	{
		transform.SetLocalEulerAngles(null, null, start);
		float t = 0f;
		while (t < time)
		{
			float val = t / time;
			transform.SetLocalEulerAngles(null, null, EaseUtils.Ease(ease, start, end, val));
			if (updateCallback != null)
			{
				updateCallback(val);
			}
			t += LocalDeltaTime;
			yield return null;
		}
		transform.SetLocalEulerAngles(null, null, end);
		if (updateCallback != null)
		{
			updateCallback(1f);
		}
		yield return null;
	}

	public new virtual void StopAllCoroutines()
	{
		base.StopAllCoroutines();
	}
}
