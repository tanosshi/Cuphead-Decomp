using UnityEngine;

public class BeeLevelBackgroundGroup : AbstractMonoBehaviour
{
	private const float MIN_Y = -800f;

	[SerializeField]
	private GameObject[] variations;

	private BeeLevel level;

	private BeeLevelPlatforms platforms;

	private int count;

	private int lastCount;

	protected override void Start()
	{
		base.Start();
		level = Level.Current as BeeLevel;
	}

	protected override void Update()
	{
		base.Update();
		if (base.transform.localPosition.y < -800f)
		{
			SetY(base.transform.localPosition.y + (float)count * 455f);
			Randomize();
		}
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		SetY(base.transform.localPosition.y + level.Speed * (float)CupheadTime.Delta);
	}

	public void Init(BeeLevelPlatforms platforms, int groupCount)
	{
		level = Level.Current as BeeLevel;
		count = groupCount;
		this.platforms = Object.Instantiate(platforms);
		this.platforms.transform.SetParent(base.transform);
		this.platforms.Init();
		Randomize();
	}

	public void Randomize()
	{
		DisableAll();
		platforms.Randomize(level.MissingPlatformCount);
		variations[Random.Range(0, variations.Length)].SetActive(true);
	}

	private void DisableAll()
	{
		GameObject[] array = variations;
		foreach (GameObject gameObject in array)
		{
			gameObject.SetActive(false);
		}
	}

	public void SetY(float y)
	{
		base.transform.SetPosition(0f, y, 0f);
	}
}
