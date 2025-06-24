using System.Collections.Generic;
using UnityEngine;

public class ClownLevelCoasterPiece : AbstractCollidableObject
{
	public List<ClownLevelRiders> riders;

	public Transform newPieceRoot;

	public Transform tailRoot;

	public Transform ridersFrontRoot;

	public Transform ridersBackRoot;

	public void Init(Vector3 startPos)
	{
		base.transform.position = startPos;
	}
}
