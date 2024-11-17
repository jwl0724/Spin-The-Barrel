using Godot;
using System;

public partial class Player : Node3D {
	// SIGNALS
	[Signal] public delegate void PlayerHurtEventHandler();
	[Signal] public delegate void PlayerDiedEventHandler();
	[Signal] public delegate void PlayerResetEventHandler();

	// CONSTANTS
	public static readonly int DEFAULT_PLAYER_HEATLH = 4;

	// VARIABLES
	public int Health { get; private set; } = DEFAULT_PLAYER_HEATLH;
	public bool IsDead { get; private set; } = false;
	public string PlayerName { get; private set; } = "TODO";

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {

	}

	public void DamagePlayer(int amount) {
		Health -= amount;
		if (Health < 0) {
			IsDead = true;
			EmitSignal(SignalName.PlayerDied);

		} else EmitSignal(SignalName.PlayerHurt);
	}

	public void ResetPlayerState() {
		Health = DEFAULT_PLAYER_HEATLH;
		IsDead = false;
		EmitSignal(SignalName.PlayerReset);
	}
}
