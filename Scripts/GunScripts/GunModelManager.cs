using Godot;
using System;

public partial class GunModelManager : Node3D {
	[Export] AnimationPlayer gunAnimator;
	private static readonly string spinAnimationName = "NerfGun_001Action_001";
	private static readonly string shootAnimationName = "NerfGun_002Action_001";
	private Gun gun;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		gun = Owner as Gun;
		gun.Connect(Gun.SignalName.OnShoot, Callable.From((bool hasBullet) => OnShoot()));
		gun.Connect(Gun.SignalName.SpinBarrel, Callable.From(() => OnSpinBarrel()));
		gun.Connect(Gun.SignalName.GunReset, Callable.From(() => ResetModelState()));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
	}

	private void OnShoot() {
		// TODO: animate the gun turning towards the camera
		gunAnimator.Play(shootAnimationName);
	}

	private void OnSpinBarrel() {
		// TODO: animate the gun tilting and then spinning
		gunAnimator.Play(spinAnimationName);
	}

	private void ResetModelState() {
		// TODO: add when the above two are implemented
	}
}
