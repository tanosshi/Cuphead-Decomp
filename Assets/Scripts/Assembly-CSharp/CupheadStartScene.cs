using System.Collections;
using UnityEngine.SceneManagement;

public class CupheadStartScene : AbstractMonoBehaviour
{
	protected override void Awake()
	{
		Cuphead.Init(true);
	}

	protected override void Start()
	{
		base.Start();
		StartCoroutine(start_cr());
	}

	private IEnumerator start_cr()
	{
		yield return null;
		SceneManager.LoadSceneAsync(1);
	}
}
