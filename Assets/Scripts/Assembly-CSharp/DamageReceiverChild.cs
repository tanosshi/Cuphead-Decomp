using UnityEngine;

public class DamageReceiverChild : AbstractMonoBehaviour
{
	[SerializeField]
	private DamageReceiver receiver;

	public DamageReceiver Receiver
	{
		get
		{
			return receiver;
		}
	}

	protected override void Start()
	{
		base.Start();
		base.tag = receiver.tag;
	}
}
