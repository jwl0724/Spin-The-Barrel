using Godot;
using System;

public partial class GunSoundManager : AudioStreamPlayer3D {
	[Export] private AudioStream shootSoundEffect;
	[Export] private AudioStream blankSoundEffect;
	[Export] private AudioStream barrelSpinSoundEffect;
	private Gun gun;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		gun = Owner as Gun;
		gun.Connect(Gun.SignalName.OnShoot, Callable.From((bool hasBullet) => OnShoot(hasBullet)));
		gun.Connect(Gun.SignalName.SpinBarrel, Callable.From(() => OnSpinBarrel()));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
	}

	private void OnShoot(bool hasBullet) {
		Stream = hasBullet ? shootSoundEffect : blankSoundEffect;
		PitchScale = (float) GD.RandRange(0.95, 1.05);
		Play();
	}

	private void OnSpinBarrel() {
		PitchScale = (float) GD.RandRange(0.95, 1.05);
		Stream = barrelSpinSoundEffect;
	}
}
