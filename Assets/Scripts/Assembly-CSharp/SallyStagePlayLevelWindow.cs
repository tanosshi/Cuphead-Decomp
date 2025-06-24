using System.Collections;
using UnityEngine;

public class SallyStagePlayLevelWindow : AbstractCollidableObject
{
	[SerializeField]
	private SallyStagePlayLevelWindowProjectile ruler;

	[SerializeField]
	private SallyStagePlayLevelWindowProjectile bottle;

	public int windowNum;

	private LevelProperties.SallyStagePlay properties;

	private SallyStagePlayLevel parent;

	private Vector3 startPos;

	private float speed;

	private float HP;

	private bool isDying;

	protected override void Start()
	{
		base.Start();
		startPos = base.transform.position;
		GetComponent<Collider2D>().enabled = false;
		GetComponent<DamageReceiver>().OnDamageTaken += OnDamageTaken;
	}

	public void Init(Vector2 pos, SallyStagePlayLevel parent)
	{
		base.transform.position = pos;
		this.parent = parent;
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		HP -= info.damage;
		if (HP <= 0f && !isDying)
		{
			StartCoroutine(slide_off_cr());
		}
	}

	public void WindowClosed()
	{
		base.animator.Play("Off");
	}

	public void WindowOpenNun()
	{
		base.animator.Play("Window_Nun");
	}

	public void ShootRuler(float speed)
	{
		AbstractPlayerController next = PlayerManager.GetNext();
		Vector3 vector = next.transform.position - base.transform.position;
		float rotation = MathUtils.DirectionToAngle(vector);
		ruler.Create(base.transform.position, rotation, speed, parent);
	}

	public void WindowOpenBaby(LevelProperties.SallyStagePlay properties)
	{
		speed = properties.CurrentState.baby.bottleSpeed;
		HP = properties.CurrentState.baby.HP;
		GetComponent<SpriteRenderer>().enabled = true;
		GetComponent<Collider2D>().enabled = true;
		string text = ((!Rand.Bool()) ? "_Boy" : "_Girl");
		base.animator.Play("Window_Baby" + text);
	}

	private void LeftWindow()
	{
		GetComponent<Collider2D>().enabled = false;
	}

	private void ShootBottle()
	{
		AbstractPlayerController next = PlayerManager.GetNext();
		Vector3 vector = new Vector3(base.transform.position.x, -360f, 0f) - base.transform.position;
		float rotation = MathUtils.DirectionToAngle(vector);
		bottle.Create(new Vector2(base.transform.position.x, base.transform.position.y - 30f), rotation, speed, parent);
	}

	private IEnumerator slide_off_cr()
	{
		isDying = true;
		base.animator.SetTrigger("Dead");
		GetComponent<Collider2D>().enabled = false;
		yield return CupheadTime.WaitForSeconds(this, 1.5f);
		Vector3 start = base.transform.position;
		Vector3 end = new Vector3(base.transform.position.x, base.transform.position.y - 50f);
		float t = 0f;
		float time = 0.1f;
		while (t < time)
		{
			t += (float)CupheadTime.Delta;
			base.transform.position = Vector3.Lerp(start, end, t / time);
			yield return null;
		}
		yield return null;
		isDying = false;
		GetComponent<SpriteRenderer>().enabled = false;
		base.animator.Play("Off");
		base.transform.position = startPos;
		yield return null;
	}

	private void SoundBabyBoy()
	{
		AudioManager.Play("sally_baby_boy");
		emitAudioFromObject.Add("sally_baby_boy");
	}

	private void SoundBabyGirl()
	{
		AudioManager.Play("sally_baby_girl");
		emitAudioFromObject.Add("sally_baby_girl");
	}
}
