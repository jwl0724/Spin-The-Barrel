using Godot;
using System;

public partial class LookBox : Area3D {
	[Signal] public delegate void LookedAtEventHandler();
	[Signal] public delegate void LookedAwayEventHandler();

	public IInteractableEntity GetInteractableEntity() {
		return Owner as IInteractableEntity;
	}

	public Node GetOwner() {
		return Owner;
	}

	public void TriggerLookedAt() {
		EmitSignal(SignalName.LookedAt);
	}

	public void TriggerLookedAway() {
		EmitSignal(SignalName.LookedAway);
	}
}
