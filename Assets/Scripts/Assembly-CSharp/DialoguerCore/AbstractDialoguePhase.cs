using System.Collections.Generic;
using UnityEngine;

namespace DialoguerCore
{
	public abstract class AbstractDialoguePhase
	{
		public delegate void PhaseCompleteHandler(int nextPhaseId);

		public readonly int[] outs;

		protected int nextPhaseId;

		protected DialoguerVariables _localVariables;

		private PhaseState _state;

		public PhaseState state
		{
			get
			{
				return _state;
			}
			protected set
			{
				_state = value;
				switch (_state)
				{
				case PhaseState.Inactive:
					break;
				case PhaseState.Start:
					onStart();
					break;
				case PhaseState.Action:
					onAction();
					break;
				case PhaseState.Complete:
					onComplete();
					break;
				}
			}
		}

		public event PhaseCompleteHandler onPhaseComplete;

		public AbstractDialoguePhase(List<int> outs)
		{
			if (outs != null)
			{
				int[] array = outs.ToArray();
				this.outs = array.Clone() as int[];
			}
		}

		public void Start(DialoguerVariables localVars)
		{
			Reset();
			_localVariables = localVars;
			state = PhaseState.Start;
		}

		public virtual void Continue(int outId)
		{
			int num = 0;
			if (outs != null && outs[outId] >= 0)
			{
				num = outs[outId];
			}
			else
			{
				Debug.LogWarning("Invalid Out Id");
			}
			nextPhaseId = num;
		}

		protected virtual void onStart()
		{
			state = PhaseState.Action;
		}

		protected virtual void onAction()
		{
			state = PhaseState.Complete;
		}

		protected virtual void onComplete()
		{
			dispatchPhaseComplete(nextPhaseId);
			state = PhaseState.Inactive;
			Reset();
		}

		protected virtual void Reset()
		{
			nextPhaseId = ((outs != null && outs[0] >= 0) ? outs[0] : 0);
			_localVariables = null;
		}

		private void dispatchPhaseComplete(int nextPhaseId)
		{
			if (this.onPhaseComplete != null)
			{
				this.onPhaseComplete(nextPhaseId);
			}
		}

		public void resetEvents()
		{
			this.onPhaseComplete = null;
		}

		public override string ToString()
		{
			return "AbstractDialoguePhase";
		}
	}
}
