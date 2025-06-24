using System.Collections;
using UnityEngine;

public class SplashScreenMDHR : AbstractMonoBehaviour
{
	private const float WAIT = 3f;

	[SerializeField]
	private SpriteRenderer fader;

	private CupheadInput.AnyPlayerInput input;

	private bool fading;

	private new Animator animator;

	protected override void Awake()
	{
		base.Awake();
		Cuphead.Init();
		input = new CupheadInput.AnyPlayerInput();
		fader.color = new Color(0f, 0f, 0f, 1f);
		animator = GetComponent<Animator>();
	}

	protected override void Start()
	{
		base.Start();
		StartCoroutine(go_cr());
	}

	protected override void Update()
	{
		base.Update();
		if (!fading && input.GetButtonDown(CupheadButton.Accept))
		{
			BeginFadeOut();
		}
	}

	private IEnumerator go_cr()
	{
		yield return CupheadTime.WaitForSeconds(this, 3f);
		animator.Play("Logo");
	}

	private void BeginFadeOut()
	{
		if (!fading)
		{
			fading = true;
			SceneLoader.properties.transitionEnd = SceneLoader.Transition.Iris;
			SceneLoader.properties.transitionStart = SceneLoader.Transition.Iris;
			SceneLoader.LoadScene(Scenes.scene_title, SceneLoader.Transition.Fade, SceneLoader.Transition.Iris);
		}
	}
}
