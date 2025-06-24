using UnityEngine;
using UnityEngine.UI;

public class AnimatedUISprite : MonoBehaviour
{
	public bool Animating;

	public bool Loop;

	public Image UIImage;

	public Sprite[] Sprites;

	public int FrameRate = 24;

	private int _currentSpriteIndex;

	private float _lastRefreshTime;

	public void ResetAnimation()
	{
		_currentSpriteIndex = 0;
	}

	private void OnEnable()
	{
		ResetAnimation();
	}

	private void OnDisable()
	{
		ResetAnimation();
	}

	private void Update()
	{
		if ((Loop || _currentSpriteIndex < Sprites.Length - 1) && Animating && Time.time >= _lastRefreshTime + 1f / (float)FrameRate)
		{
			_currentSpriteIndex++;
			if (_currentSpriteIndex >= Sprites.Length)
			{
				_currentSpriteIndex = 0;
			}
			UIImage.sprite = Sprites[_currentSpriteIndex];
			_lastRefreshTime = Time.time;
		}
	}
}
