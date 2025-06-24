using System.Collections;
using UnityEngine;

public class DicePalaceTestLevelTest : LevelProperties.DicePalaceTest.Entity
{
	[SerializeField]
	private BasicProjectile basic;

	[SerializeField]
	private Transform shoot1;

	[SerializeField]
	private Transform shoot2;

	[SerializeField]
	private Transform shoot3;

	private float speed = 200f;

	public IEnumerator start_it_cr()
	{
		StartCoroutine(shoot_right());
		StartCoroutine(shoot_right_2());
		StartCoroutine(shoot_left());
		yield return null;
	}

	private IEnumerator shoot_right()
	{
		while (true)
		{
			basic.Create(shoot1.transform.position, 0f, speed);
			yield return CupheadTime.WaitForSeconds(this, 2f);
		}
	}

	private IEnumerator shoot_right_2()
	{
		while (true)
		{
			basic.Create(shoot3.transform.position, 0f, speed);
			yield return CupheadTime.WaitForSeconds(this, 2f);
		}
	}

	private IEnumerator shoot_left()
	{
		while (true)
		{
			basic.Create(shoot2.transform.position, 0f, 0f - speed);
			yield return CupheadTime.WaitForSeconds(this, 2f);
		}
	}
}
