public class SoundEmitter
{
	private AbstractPausableComponent parent;

	public SoundEmitter(AbstractPausableComponent parent)
	{
		this.parent = parent;
	}

	public void Add(string key)
	{
		parent.EmitSound(key);
	}
}
