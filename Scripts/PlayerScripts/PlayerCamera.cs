using Godot;
using System;

public partial class PlayerCamera : Camera3D {
	private static readonly float MAX_CAMERA_Y_ANGLE = 60f; // up/down camera movement
	private static readonly float MAX_CAMERA_X_ANGLE = 70f; // left/right camera movement
	private bool isLocked = false;
	private Vector3 lerpPosition = Vector3.Zero;
	private Player player;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		player = Owner as Player;	
		player.Connect(Player.SignalName.PlayerDied, Callable.From(() => OnPlayerDeath()));
		player.Connect(Player.SignalName.PlayerHurt, Callable.From(() => OnPlayerHurt()));
		player.Connect(Player.SignalName.PlayerHoldGun, Callable.From((Vector3 gunPosition) => OnGunHold(gunPosition)));
		player.Connect(Player.SignalName.PlayerReset, Callable.From(() => Current = true ));
		CameraManager.Instance.SetPlayerCameraInstance(this);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		if (lerpPosition.IsEqualApprox(Vector3.Zero)) return;
		Rotation = Rotation.Lerp(lerpPosition.DirectionTo(lerpPosition), 0.4f * (float) delta);
	}

	
	// handle camera turning
	public override void _Input(InputEvent inputEvent) {
		if (!Current) return;
		if (inputEvent is not InputEventMouseMotion movement || isLocked) return;
		float rotationY = Mathf.Clamp(
			Mathf.DegToRad(-movement.Relative.X * player.MouseSensitivity) + Rotation.Y,
			Mathf.DegToRad(-MAX_CAMERA_Y_ANGLE), Mathf.DegToRad(MAX_CAMERA_Y_ANGLE)	
		);
		float rotationX = Mathf.Clamp(
			Mathf.DegToRad(-movement.Relative.Y * player.MouseSensitivity) + Rotation.X,
			Mathf.DegToRad(-MAX_CAMERA_X_ANGLE), Mathf.DegToRad(MAX_CAMERA_X_ANGLE)	
		);
		Rotation = new Vector3(rotationX, rotationY, 0);
	}

	private void OnPlayerDeath() {
		// TODO: animate the camera jerking upwards then falling
		Tween deathTween = CreateTween();
		deathTween.TweenProperty(this, nameof(Rotation).ToLower(), Vector3.Up * Mathf.DegToRad(90), 0.5f); // TODO: double check this later
		// TODO: add a delay to the death tween so the player can see the death animation
		deathTween.TweenCallback(Callable.From(() => CameraManager.Instance.SwitchToDeadCamera()));
		deathTween.Play();
	}

	private void OnPlayerHurt() {
		// create hurt animation
		Tween hurtTween = CreateTween();
		hurtTween.TweenProperty(this, nameof(Rotation).ToLower(), Vector3.Right * Mathf.DegToRad(20), 0.25f);
		hurtTween.TweenProperty(this, nameof(Rotation).ToLower(), Vector3.Zero, 0.75f);
		hurtTween.TweenCallback(Callable.From(() => isLocked = false));
		hurtTween.Play();
	}


	private void OnGunHold(Vector3 gunPosition) {
		isLocked = true;
		lerpPosition = gunPosition;
	}
}
