using Godot;
using System;

public partial class PlayerCamera : Camera3D {
	private static readonly float MAX_CAMERA_Y_ANGLE = 60f; // up/down camera movement
	private static readonly float MAX_CAMERA_X_ANGLE = 70f; // left/right camera movement
	private bool isLocked = false;
	private Player player;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		player = Owner as Player;	
		player.Connect(Player.SignalName.PlayerDied, Callable.From(() => OnPlayerDeath()));
		player.Connect(Player.SignalName.PlayerHurt, Callable.From(() => OnPlayerHurt()));
		player.Connect(Player.SignalName.PlayerPickUpGun, Callable.From(() => OnGunInteract(true)));
		player.Connect(Player.SignalName.PlayerDropGun, Callable.From(() => OnGunInteract(false)));
		player.Connect(Player.SignalName.PlayerEnterAimMode, Callable.From((bool inAimMode) => OnAimMode(inAimMode)));
		player.Connect(Player.SignalName.PlayerReset, Callable.From(() => Current = true ));
		CameraManager.Instance.SetPlayerCameraInstance(this);
	}
	
	// handle camera turning
	public override void _Input(InputEvent inputEvent) {
		if (!Current || player.IsDead) return;
		if (inputEvent is not InputEventMouseMotion movement || isLocked || Input.MouseMode != Input.MouseModeEnum.Captured) 
			return;
		float rotationY = Mathf.Clamp(
			Mathf.DegToRad(-movement.Relative.X * player.MouseSensitivity) + Rotation.Y,
			Mathf.DegToRad(-MAX_CAMERA_Y_ANGLE), Mathf.DegToRad(MAX_CAMERA_Y_ANGLE)	
		);
		float rotationX = Mathf.Clamp(
			Mathf.DegToRad(-movement.Relative.Y * player.MouseSensitivity) + Rotation.X,
			Mathf.DegToRad(-MAX_CAMERA_X_ANGLE), Mathf.DegToRad(MAX_CAMERA_X_ANGLE)	
		);
		Rotation = new Vector3(rotationX, rotationY, 0);
		GD.Print($"{player.NetworkID}: {Rotation}");
		GameNetwork network = GameNetwork.Instance;
		network.Rpc(GameNetwork.MethodName.SyncRotation, player.NetworkID, Rotation);
	}

	private void OnPlayerDeath() {
		Tween deathTween = CreateTween();
		deathTween.SetEase(Tween.EaseType.Out);
		deathTween.TweenProperty(this, nameof(Rotation).ToLower(), Vector3.Right * Mathf.DegToRad(110), 0.3f);
		deathTween.TweenProperty(this, nameof(Rotation).ToLower(), Vector3.Up * Mathf.DegToRad(70) + Vector3.Right * Mathf.DegToRad(110), 0.1f);
		deathTween.TweenInterval(4f);
		deathTween.TweenCallback(Callable.From(() => CameraManager.Instance.SwitchToDeadCamera()));
		deathTween.Play();
	}

	private void OnPlayerHurt() {
		// create hurt animation
		Tween hurtTween = CreateTween();
		hurtTween.TweenProperty(this, nameof(Rotation).ToLower(), Vector3.Right * Mathf.DegToRad(20), 0.05f);
		hurtTween.SetEase(Tween.EaseType.Out);
		hurtTween.TweenProperty(this, nameof(Rotation).ToLower(), Vector3.Zero, 0.4f);
		hurtTween.TweenCallback(Callable.From(() => isLocked = false));
		hurtTween.Play();
	}

	private void OnGunInteract(bool isPickUpEvent) {
		if (!isPickUpEvent) return;

		Vector3 originalRotation = Rotation;
		LookAt(player.NerfGun.GlobalPosition);
		Vector3 finalRotation = Rotation;
		Rotation = originalRotation;
		
		const float tweenTime = 0.4f;
		Tween centerCameraTween = CreateTween();
		centerCameraTween.SetEase(Tween.EaseType.Out);
		centerCameraTween.TweenProperty(this, nameof(Rotation).ToLower(), finalRotation, tweenTime);
		centerCameraTween.Play();
	}

	private void OnAimMode(bool inAimMode) {
		if (inAimMode) Input.MouseMode = Input.MouseModeEnum.Captured;
		else {
		 	Input.MouseMode = Input.MouseModeEnum.Visible;
			OnGunInteract(true);
		}
	}
}
