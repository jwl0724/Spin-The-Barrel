using Godot;
using System;

// class will animate gun
public partial class GunModelManager : Node3D {
	[Export] AnimationPlayer gunAnimator;
	private static readonly Vector3 droppedRotation = new(Mathf.DegToRad(83.6f), Mathf.DegToRad(-49.4f), Mathf.DegToRad(0.9f));
	private static readonly Vector3 pickedUpRotation = new(Mathf.DegToRad(15.5f), Mathf.DegToRad(-72.9f), Mathf.DegToRad(24.5f));
	private static readonly Vector3 spinningRotation = new(Mathf.DegToRad(0.7f), Mathf.DegToRad(-154.3f), Mathf.DegToRad(18.6f));
	// private static readonly Vector3 shootRotationFactor = new(Mathf.DegToRad(15.5f), Mathf.DegToRad(-72.9f), Mathf.DegToRad(24.5f)); // will see if lookat works
	private static readonly string SPIN_ANIMATION = "NerfGun_001Action_001";
	private static readonly string SHOOT_ANIMATION = "NerfGun_002Action_001";
	private Gun gun;

	public override void _Ready() {
		gun = Owner as Gun;
		// lay gun down onto table on first load
		Rotation = droppedRotation;
	}

	// handles only rotational component, since gun actually needs to move not just the visuals
	public void PlayPickUp(float duration) {
		Tween tween = CreateTween();
		tween.TweenProperty(this, nameof(Rotation).ToLower(), pickedUpRotation, duration);
		tween.Play();
	}	

	public void PlayDrop(float duration) {
		Tween tween = CreateTween();
		tween.TweenProperty(this, nameof(Rotation).ToLower(), droppedRotation, duration);
		tween.Play();
	}

	public void PlayShoot(Player player, Callable function) {
		Player target = player == null ? gun.Holder : player;
		Vector3 originalRotation = Rotation;
		LookAt(target.GlobalPosition);
		Vector3 finalRotation = Rotation;
		Rotation = originalRotation;

		const float turnTime = 0.3f;
		Tween tween = CreateTween();
		tween.TweenProperty(this, nameof(Rotation).ToLower(), finalRotation, turnTime);
		tween.TweenInterval(turnTime * 2);
		tween.TweenCallback(Callable.From(() => {
			gunAnimator.Play(SHOOT_ANIMATION);
			function.Call();
		}));
	}

	public void PlaySpinBarrel() {
		const float turnTime = 0.15f;
		Tween tween = CreateTween();
		tween.TweenProperty(this, nameof(Rotation).ToLower(), spinningRotation, turnTime);
		tween.TweenProperty(this, nameof(Rotation).ToLower(), pickedUpRotation, turnTime * 2);
		tween.Play();
	}
}
