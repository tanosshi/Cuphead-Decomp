using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanePlayerMotor : AbstractPlanePlayerComponent
{
	public enum BufferedInput
	{
		Jump = 0,
		Super = 1
	}

	public class Force
	{
		public Vector2 force;

		public bool enabled;

		public Force(Vector2 force, bool enabled)
		{
			this.force = force;
			this.enabled = enabled;
		}
	}

	[Serializable]
	public class Properties
	{
		public float speed = 520f;

		public float shrunkSpeed = 720f;
	}

	private const float PADDING_TOP = 65f;

	private const float PADDING_BOTTOM = 35f;

	private const float PADDING_LEFT = 70f;

	private const float PADDING_RIGHT = 30f;

	private const float DIAGONAL_FALLOFF = 0.75f;

	private const float ANALOG_THRESHOLD = 0.35f;

	private const float HIT_STUN_TIME = 0.15f;

	private const float EASING_TIME = 15f;

	private static bool USE_FALLOFF = true;

	[NonSerialized]
	public Properties properties;

	private bool damageStun;

	private Vector2 pos;

	private List<Force> externalForces = new List<Force>();

	private BufferedInput bufferedInput;

	private float timeSinceInputBuffered = 0.134f;

	private Vector2 _velocity;

	public Trilean2 MoveDirection { get; private set; }

	public Vector2 Velocity
	{
		get
		{
			return _velocity;
		}
	}

	protected override void OnAwake()
	{
		base.OnAwake();
		MoveDirection = default(Trilean2);
		properties = new Properties();
		base.player.damageReceiver.OnDamageTaken += OnDamageTaken;
		base.player.OnReviveEvent += OnRevive;
		pos = base.transform.position;
	}

	protected override void Start()
	{
		base.Start();
		pos = base.transform.position;
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		if (!(base.player.stats.StoneTime > 0f))
		{
			HandleInput();
			Move();
			HandleRaycasts();
			ClampPosition();
		}
	}

	protected override void LateUpdate()
	{
		base.LateUpdate();
		ClampPosition();
	}

	private void OnDamageTaken(DamageDealer.DamageInfo info)
	{
		if (info.damage > 0f)
		{
			StartCoroutine(onDamageTaken_cr());
		}
	}

	private void HandleInput()
	{
		timeSinceInputBuffered += CupheadTime.FixedDelta;
		if (base.player.WeaponBusy)
		{
			BufferInputs();
		}
		if (damageStun)
		{
			MoveDirection = new Trilean2(-1, 0);
			return;
		}
		Trilean trilean = 0;
		Trilean trilean2 = 0;
		if (base.player.input.actions.GetAxis(CupheadButton.MoveHorizontal.ToString()) > 0.35f || base.player.input.actions.GetAxis(CupheadButton.MoveHorizontal.ToString()) < -0.35f)
		{
			trilean = base.player.input.actions.GetAxis(CupheadButton.MoveHorizontal.ToString());
		}
		if (base.player.input.actions.GetAxis(CupheadButton.MoveVertical.ToString()) > 0.35f || base.player.input.actions.GetAxis(CupheadButton.MoveVertical.ToString()) < -0.35f)
		{
			trilean2 = base.player.input.actions.GetAxis(CupheadButton.MoveVertical.ToString());
		}
		MoveDirection = new Trilean2(trilean.Value, trilean2.Value);
	}

	private void BufferInput(BufferedInput input)
	{
		bufferedInput = input;
		timeSinceInputBuffered = 0f;
	}

	public void BufferInputs()
	{
		if (base.player.input.actions.GetButtonDown(2))
		{
			BufferInput(BufferedInput.Jump);
		}
		else if (base.player.input.actions.GetButtonDown(4))
		{
			BufferInput(BufferedInput.Super);
		}
	}

	public void ClearBufferedInput()
	{
		timeSinceInputBuffered = 0.134f;
	}

	public bool HasBufferedInput(BufferedInput input)
	{
		return bufferedInput == input && timeSinceInputBuffered < 0.134f;
	}

	private void Move()
	{
		float num = ((!base.player.Shrunk) ? properties.speed : properties.shrunkSpeed);
		if ((int)MoveDirection.x != 0 && (int)MoveDirection.y != 0)
		{
			num *= 0.75f;
		}
		pos.x += (float)MoveDirection.x * num * CupheadTime.FixedDelta;
		pos.y += (float)MoveDirection.y * num * CupheadTime.FixedDelta;
		foreach (Force externalForce in externalForces)
		{
			if (externalForce.enabled)
			{
				pos += externalForce.force * CupheadTime.FixedDelta;
			}
		}
		Vector2 vector = base.transform.position;
		if (USE_FALLOFF)
		{
			float num2 = 15f;
			base.transform.position = Vector2.Lerp(base.transform.position, pos, num2 * CupheadTime.FixedDelta);
		}
		else
		{
			base.transform.AddPosition(0f, (float)MoveDirection.y * num * CupheadTime.FixedDelta);
		}
		Vector2 vector2 = base.transform.position;
		_velocity = (vector2 - vector) / CupheadTime.FixedDelta;
	}

	private void HandleRaycasts()
	{
		int layerMask = 262144;
		int layerMask2 = 524288;
		int layerMask3 = 1048576;
		Vector2 origin = (Vector2)base.transform.position + new Vector2(-20f, 15f);
		float num = 100f;
		float num2 = 100f;
		RaycastHit2D raycastHit2D = Physics2D.BoxCast(origin, new Vector2(1f, num2), 0f, Vector2.left, num * 0.5f, layerMask);
		RaycastHit2D raycastHit2D2 = Physics2D.BoxCast(origin, new Vector2(1f, num2), 0f, Vector2.right, num * 0.5f, layerMask);
		RaycastHit2D raycastHit2D3 = Physics2D.BoxCast(origin, new Vector2(num, 1f), 0f, Vector2.up, num2 * 0.5f, layerMask2);
		RaycastHit2D raycastHit2D4 = Physics2D.BoxCast(origin, new Vector2(num, 1f), 0f, Vector2.down, num2 * 0.5f, layerMask3);
		if (raycastHit2D.collider != null)
		{
			base.transform.SetPosition(raycastHit2D.point.x + 70f);
		}
		if (raycastHit2D2.collider != null)
		{
			base.transform.SetPosition(raycastHit2D2.point.x - 30f);
		}
		if (raycastHit2D3.collider != null)
		{
			base.transform.SetPosition(null, raycastHit2D3.point.y - 65f);
		}
		if (raycastHit2D4.collider != null)
		{
			base.transform.SetPosition(null, raycastHit2D4.point.y + 35f);
		}
	}

	private void ClampPosition()
	{
		Vector2 vector = pos;
		vector.x = Mathf.Clamp(vector.x, (float)Level.Current.Left + 70f, (float)Level.Current.Right - 30f);
		vector.y = Mathf.Clamp(vector.y, (float)Level.Current.Ground + 35f, (float)Level.Current.Ceiling - 65f);
		pos = vector;
	}

	public void OnRevive(Vector3 pos)
	{
		base.transform.position = pos;
		this.pos = pos;
		MoveDirection = default(Trilean2);
		damageStun = false;
	}

	private IEnumerator onDamageTaken_cr()
	{
		damageStun = true;
		yield return CupheadTime.WaitForSeconds(this, 0.15f);
		damageStun = false;
	}

	public void AddForce(Force force)
	{
		externalForces.Add(force);
	}

	public void RemoveForce(Force force)
	{
		externalForces.Remove(force);
	}
}
