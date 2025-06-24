using System.Collections.Generic;
using UnityEngine;

public static class PauseManager
{
	public enum State
	{
		Unpaused = 0,
		Paused = 1
	}

	public static State state;

	private static float oldSpeed;

	private static List<AbstractPausableComponent> children = new List<AbstractPausableComponent>();

	public static void Reset()
	{
		state = State.Unpaused;
	}

	public static void AddChild(AbstractPausableComponent child)
	{
		children.Add(child);
	}

	public static void RemoveChild(AbstractPausableComponent child)
	{
		children.Remove(child);
	}

	public static void Pause()
	{
		if (state == State.Paused)
		{
			return;
		}
		state = State.Paused;
		AudioListener.pause = true;
		oldSpeed = CupheadTime.GlobalSpeed;
		CupheadTime.GlobalSpeed = 0f;
		foreach (AbstractPausableComponent child in children)
		{
			child.OnPause();
		}
		SetChildren(false);
	}

	public static void Unpause()
	{
		if (state == State.Unpaused)
		{
			return;
		}
		state = State.Unpaused;
		AudioListener.pause = false;
		CupheadTime.GlobalSpeed = oldSpeed;
		foreach (AbstractPausableComponent child in children)
		{
			child.OnUnpause();
		}
		SetChildren(true);
	}

	public static void Toggle()
	{
		if (state == State.Paused)
		{
			Unpause();
		}
		else
		{
			Pause();
		}
	}

	private static void SetChildren(bool enabled)
	{
		for (int i = 0; i < children.Count; i++)
		{
			AbstractPausableComponent abstractPausableComponent = children[i];
			if (abstractPausableComponent == null)
			{
				children.Remove(abstractPausableComponent);
				i--;
			}
			else if (enabled)
			{
				abstractPausableComponent.enabled = abstractPausableComponent.preEnabled;
			}
			else
			{
				abstractPausableComponent.preEnabled = abstractPausableComponent.enabled;
				abstractPausableComponent.enabled = false;
			}
		}
	}
}
