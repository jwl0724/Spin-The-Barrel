using Godot;
using System;

public partial class PlayerReticle : RayCast3D {
	private Player player;
	private LookBox lookAtTarget;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		player = Owner as Player;
		player.Connect(Player.SignalName.PlayerInteract, Callable.From(() => OnPlayerInteract()));
	}

    public override void _PhysicsProcess(double delta) {
        if (!IsColliding()) {
			lookAtTarget?.TriggerLookedAway();
			lookAtTarget = null;
			return;
		}
		LookBox target = GetCollider() as LookBox;
		if (target == lookAtTarget) return;
		else {
			lookAtTarget = target;
			lookAtTarget.TriggerLookedAt();
		}
    }

    private void OnPlayerInteract() {
		if (lookAtTarget?.GetOwner() is Player otherPlayer) {
			if (player.CanShootOther && player.NerfGun.InAimMode) {
				player.NerfGun.Shoot(otherPlayer);
				player.NerfGun.ExitAimMode();
				player.CanShootOther = false;

			} else if (player.CanChooseNextTurn) {
				GameDriver.Instance.SetNextTurn(otherPlayer);
				player.CanChooseNextTurn = false;
			}

		} else lookAtTarget?.GetInteractableEntity()?.Interact();
	}
}
