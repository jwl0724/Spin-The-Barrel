using Godot;
using System;

public partial class PlayerReticle : RayCast3D {
	private Player player;
	private HitBox lookAtTarget;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		player = Owner as Player;
		player.Connect(Player.SignalName.PlayerInteract, Callable.From(() => OnPlayerInteract()));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
	}

    public override void _PhysicsProcess(double delta) {
        if (!IsColliding()) {
			lookAtTarget?.TriggerLookedAway();
			lookAtTarget = null;
			return;
		}
		HitBox target = GetCollider() as HitBox;
		if (target == lookAtTarget) return;
		else {
			lookAtTarget = target;
			lookAtTarget.TriggerLookedAt();
		}
    }

    private void OnPlayerInteract() {
		lookAtTarget?.GetInteractableEntity()?.Interact();
	}
}
