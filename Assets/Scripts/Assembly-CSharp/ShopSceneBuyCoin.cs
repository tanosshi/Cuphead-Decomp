using System.Collections;
using UnityEngine;

public class ShopSceneBuyCoin : MonoBehaviour
{
	public float VelocityXMin = -500f;

	public float VelocityXMax = 500f;

	public float VelocityYMin = 500f;

	public float VelocityYMax = 1000f;

	private const float GRAVITY = -100f;

	private Vector2 velocity;

	private Vector2 randomRotation;

	private float accumulatedGravity;

	[SerializeField]
	private SpriteRenderer spriteRenderer;

	[SerializeField]
	private bool isCoinA;

	private void Start()
	{
		velocity = new Vector2(Random.Range(VelocityXMin, VelocityXMax), Random.Range(VelocityYMin, VelocityYMax));
		randomRotation = new Vector2(Random.Range(-500, 500), Random.Range(-500, 500));
		StartCoroutine(scaledown_cr());
	}

	private void Update()
	{
		base.transform.position += (Vector3)(velocity + new Vector2(-300f, accumulatedGravity)) * Time.fixedDeltaTime;
		accumulatedGravity += -100f;
		base.transform.Rotate(randomRotation * Time.deltaTime);
	}

	private IEnumerator scaledown_cr()
	{
		Vector2 startScale = base.transform.localScale;
		float t = 0f;
		float TIME = 1f;
		while (t < TIME)
		{
			float val = t / TIME;
			float newAlpha = Mathf.Lerp(1f, 0f, EaseUtils.Ease(EaseUtils.EaseType.easeInOutSine, 0f, 1f, val));
			Color newColor = spriteRenderer.color;
			newColor.a = newAlpha;
			spriteRenderer.color = newColor;
			t += Time.deltaTime;
			yield return null;
		}
		Object.Destroy(base.gameObject);
	}
}
