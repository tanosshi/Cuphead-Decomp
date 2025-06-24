using System;
using System.Collections.Generic;
using UnityEngine;

public class FlyingMermaidLevelBackgroundChange : AbstractPausableComponent
{
	public List<Transform> points;

	private float size;

	private const float X_OUT = -1280f;

	[Range(0f, 2000f)]
	public float speed;

	[NonSerialized]
	public float b_playbackSpeed = 1f;

	[SerializeField]
	private FlyingMermaidLevelCoralCluster toCopy;

	private FlyingMermaidLevelCoralCluster copy1;

	private List<FlyingMermaidLevelCoralCluster> copies;

	private Vector3 getOffset;

	private Vector3 _offset;

	private int index;

	protected override void Start()
	{
		base.Start();
		points = new List<Transform>();
		size = toCopy.GetComponent<Collider2D>().bounds.size.x;
		getOffset.x = base.transform.position.x;
		copy1 = UnityEngine.Object.Instantiate(toCopy);
		copy1.transform.parent = base.transform;
		FlyingMermaidLevelCoralCluster flyingMermaidLevelCoralCluster = UnityEngine.Object.Instantiate(toCopy);
		flyingMermaidLevelCoralCluster.transform.parent = base.transform;
		copy1.transform.SetPosition(getOffset.x + size, toCopy.transform.position.y, 0f);
		flyingMermaidLevelCoralCluster.transform.SetPosition(getOffset.x + size * 2f, toCopy.transform.position.y, 0f);
		points.AddRange(toCopy.points);
		points.AddRange(copy1.points);
		points.AddRange(flyingMermaidLevelCoralCluster.points);
		copies = new List<FlyingMermaidLevelCoralCluster>();
		copies.Add(toCopy);
		copies.Add(copy1);
		copies.Add(flyingMermaidLevelCoralCluster);
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		if (GetComponent<ParallaxLayer>() != null)
		{
			GetComponent<ParallaxLayer>().enabled = false;
		}
		Vector3 localPosition = base.transform.localPosition;
		if (copies[index].transform.position.x <= 0f - size)
		{
			copies[index].transform.position = new Vector2(size * 2f, copies[index].transform.position.y);
			index = (index + 1) % copies.Count;
		}
		localPosition.x -= speed * CupheadTime.FixedDelta * b_playbackSpeed;
		base.transform.localPosition = localPosition;
	}
}
