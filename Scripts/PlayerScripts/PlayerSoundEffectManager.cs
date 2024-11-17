using Godot;
using System;

public partial class PlayerSoundEffectManager : AudioStreamPlayer3D {
	private Player player;
	[Export] private Godot.Collections.Array<AudioStream> hurtCollection;
	[Export] private Godot.Collections.Array<AudioStream> deathCollection;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		player = Owner as Player;

		// connect signals
		player.Connect(Player.SignalName.PlayerHurt, Callable.From(() => OnPlayerHit()));
		player.Connect(Player.SignalName.PlayerDied, Callable.From(() => OnPlayerDeath()));
		player.Connect(Player.SignalName.PlayerReset, Callable.From(() => OnPlayerReset()));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
	}

	private void OnPlayerHit() {	
		int soundIndex = (int) GD.Randi() % hurtCollection.Count;
		PitchScale = (float) GD.RandRange(0.95, 1.05);
		Stream = hurtCollection[soundIndex];
		Play();
	}

	private void OnPlayerDeath() {
		int soundIndex = (int) GD.Randi() % hurtCollection.Count;
		VolumeDb = (float) GD.RandRange(0.95, 1.05);
		PitchScale = (float) GD.RandRange(0.95, 1.05);
		Stream = deathCollection[soundIndex];
		Play();
	}

	private void OnPlayerReset() {
		Stream = null;
		PitchScale = 1;
		VolumeDb = 1;
	}
}
