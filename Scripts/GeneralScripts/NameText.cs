using Godot;
using System;

public partial class NameText : MeshInstance3D {
	[Export] private HitBox hitbox;
	[Export] private Vector3 namePosition = new Vector3(0, 0.92f, 0);
	private bool isLookedAt = false;
	private float sinVariable = 0;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		SetNameDirection();

		// set text
		TextMesh textMesh = Mesh as TextMesh;
		textMesh.Text = (Owner as IInteractableEntity).GetEntityName();

		// set hide properties
		Position = namePosition;
		Scale = Vector3.Zero;

		// connect signals
		_ = hitbox.Connect(HitBox.SignalName.LookedAt, Callable.From(() => OnLookedAt()));
		_ = hitbox.Connect(HitBox.SignalName.LookedAway, Callable.From(() => OnLookedAway()));
	}

	public override void _Process(double delta) {
		if (isLookedAt) {
			// animates a hover effect
			int offsetSpeed = 8;
			float offsetAmount = 0.005f;
			if (sinVariable >= Mathf.Pi * 2) sinVariable = 0;
			sinVariable += (float) delta * offsetSpeed;
			Position += Vector3.Up * Mathf.Sin(sinVariable) * offsetAmount;
			return;
		}
		Position = namePosition;
		sinVariable = 0;
	}

	private void OnLookedAt() {
		Tween showAnimation = CreateTween();
		showAnimation.TweenProperty(this, nameof(Scale).ToLower(), Vector3.One, 0.25f);
		showAnimation.TweenCallback(Callable.From(() => isLookedAt = true));
		showAnimation.Play();
	}

	private void OnLookedAway() {
		Tween hideAnimation = CreateTween();
		hideAnimation.TweenProperty(this, nameof(Scale).ToLower(), Vector3.Zero, 0.25f);
		hideAnimation.TweenCallback(Callable.From(() => isLookedAt = false));
		hideAnimation.Play();
	}
	
	private void SetNameDirection() {
		foreach(Player player in GameDriver.Players) {
			if(player.IsRemotePlayer) continue;
			LookAt(player.GlobalPosition);
			
			float tiltDownAngle = 30, fullRotation = 180;
			Rotation += Vector3.Up * Mathf.DegToRad(fullRotation);
			Rotation += Vector3.Right * Mathf.DegToRad(tiltDownAngle);
			return;
		}
	}
}
