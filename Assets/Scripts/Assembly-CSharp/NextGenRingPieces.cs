using System;
using UnityEngine;

[Serializable]
public class NextGenRingPieces
{
	public Texture topLeft;

	public Texture topRight;

	public Texture middleLeft;

	public Texture middleRight;

	public Texture bottomLeft;

	public Texture bottomRight;

	private Texture[] _pieces;

	public Texture[] getPieces()
	{
		if (_pieces == null)
		{
			_pieces = new Texture[6];
			_pieces[0] = topRight;
			_pieces[1] = middleRight;
			_pieces[2] = bottomRight;
			_pieces[3] = topLeft;
			_pieces[4] = middleLeft;
			_pieces[5] = bottomLeft;
		}
		return _pieces;
	}
}
