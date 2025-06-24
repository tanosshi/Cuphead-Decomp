using System;
using System.Collections;
using UnityEngine;

public class ClownLevelCoasterHandler : LevelProperties.Clown.Entity
{
	public bool finalRun;

	public bool isRunning;

	[SerializeField]
	private ClownLevelClownSwing swing;

	[SerializeField]
	private ClownLevelLights warningLight;

	[SerializeField]
	private Transform frontTrackStart;

	[SerializeField]
	private Transform backTrackStart;

	[SerializeField]
	private ClownLevelCoasterPiece redCoaster;

	[SerializeField]
	private ClownLevelCoasterPiece blueCoaster;

	[SerializeField]
	private ClownLevelRiders ridersPrefab;

	[SerializeField]
	private GameObject tailPrefab;

	[SerializeField]
	private ClownLevelCoaster coasterPrefab;

	public event Action OnCoasterLeave;

	public override void LevelInit(LevelProperties.Clown properties)
	{
		base.LevelInit(properties);
		finalRun = false;
	}

	public void StartCoaster()
	{
		isRunning = true;
		StartCoroutine(coaster_cr());
	}

	private IEnumerator coaster_cr()
	{
		LevelProperties.Clown.Coaster p = base.properties.CurrentState.coaster;
		string[] coasterPattern = p.coasterTypeString.GetRandom().Split(',');
		float coasterSize = redCoaster.GetComponent<Renderer>().bounds.size.x;
		if (base.properties.CurrentState.stateName == LevelProperties.Clown.States.Swing)
		{
			while (swing.state == ClownLevelClownSwing.State.Intro)
			{
				yield return null;
			}
		}
		yield return CupheadTime.WaitForSeconds(this, p.initialDelay);
		while (isRunning)
		{
			yield return null;
			ClownLevelCoaster coaster = UnityEngine.Object.Instantiate(coasterPrefab);
			coaster.Init(backTrackStart.position, frontTrackStart.position, p, coasterPattern.Length, coasterSize, warningLight);
			Transform lastInstantiatedRoot = coaster.pieceRoot;
			for (int i = 0; i < coasterPattern.Length; i++)
			{
				if (i % 2 == 1)
				{
					ClownLevelCoasterPiece clownLevelCoasterPiece = UnityEngine.Object.Instantiate(blueCoaster);
					clownLevelCoasterPiece.Init(lastInstantiatedRoot.position);
					lastInstantiatedRoot = clownLevelCoasterPiece.newPieceRoot;
					clownLevelCoasterPiece.transform.parent = coaster.transform;
					if (i == coasterPattern.Length)
					{
						lastInstantiatedRoot = clownLevelCoasterPiece.tailRoot;
					}
					if (coasterPattern[i][0] == 'F')
					{
						ClownLevelRiders clownLevelRiders = UnityEngine.Object.Instantiate(ridersPrefab);
						ClownLevelRiders clownLevelRiders2 = UnityEngine.Object.Instantiate(ridersPrefab);
						clownLevelRiders.transform.position = clownLevelCoasterPiece.ridersFrontRoot.position;
						clownLevelRiders.transform.parent = clownLevelCoasterPiece.ridersFrontRoot.transform;
						clownLevelRiders.inFront = true;
						clownLevelCoasterPiece.riders.Add(clownLevelRiders);
						clownLevelRiders2.transform.position = clownLevelCoasterPiece.ridersBackRoot.position;
						clownLevelRiders2.transform.parent = clownLevelCoasterPiece.ridersBackRoot.transform;
						clownLevelRiders2.inFront = false;
						clownLevelCoasterPiece.riders.Add(clownLevelRiders2);
					}
				}
				else
				{
					ClownLevelCoasterPiece clownLevelCoasterPiece2 = UnityEngine.Object.Instantiate(redCoaster);
					clownLevelCoasterPiece2.Init(lastInstantiatedRoot.position);
					lastInstantiatedRoot = clownLevelCoasterPiece2.newPieceRoot;
					clownLevelCoasterPiece2.transform.parent = coaster.transform;
					if (i == coasterPattern.Length)
					{
						lastInstantiatedRoot = clownLevelCoasterPiece2.tailRoot;
					}
					if (coasterPattern[i][0] == 'F')
					{
						ClownLevelRiders clownLevelRiders3 = UnityEngine.Object.Instantiate(ridersPrefab);
						ClownLevelRiders clownLevelRiders4 = UnityEngine.Object.Instantiate(ridersPrefab);
						clownLevelRiders3.transform.position = clownLevelCoasterPiece2.ridersFrontRoot.position;
						clownLevelRiders3.transform.parent = clownLevelCoasterPiece2.ridersFrontRoot.transform;
						clownLevelRiders3.inFront = true;
						clownLevelCoasterPiece2.riders.Add(clownLevelRiders3);
						clownLevelRiders4.transform.position = clownLevelCoasterPiece2.ridersBackRoot.position;
						clownLevelRiders4.transform.parent = clownLevelCoasterPiece2.ridersBackRoot.transform;
						clownLevelRiders4.inFront = false;
						clownLevelCoasterPiece2.riders.Add(clownLevelRiders4);
					}
				}
			}
			GameObject tail = UnityEngine.Object.Instantiate(tailPrefab);
			tail.transform.position = lastInstantiatedRoot.position;
			tail.transform.parent = coaster.transform;
			coaster.BackCoasterSetup();
			while (coaster != null)
			{
				yield return null;
			}
			if (this.OnCoasterLeave != null)
			{
				this.OnCoasterLeave();
			}
			if (finalRun)
			{
				isRunning = false;
				finalRun = false;
				break;
			}
			yield return CupheadTime.WaitForSeconds(this, p.mainLoopDelay);
			yield return null;
		}
	}
}
