using Godot;
using System;

public partial class NameText : MeshInstance3D
{
	[Export] private LookBox hitbox;
	[Export] private Vector3 namePosition = new Vector3(0, 0.92f, 0);
	[Export] private float offsetAmount = 0.4f;
	[Export] private float offsetSpeed = 8;
	[Export] private float tiltAngle = 30;
	private bool isLookedAt = false;
	private float sinVariable = 0;
	private Vector3 basePosition;
	private Player localPlayer = null;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

		// set text
		TextMesh textMesh = Mesh as TextMesh;
		textMesh.Text = (Owner as IInteractableEntity).GetEntityName();

		// set hide properties
		Position = namePosition;
		Scale = Vector3.Zero;
		basePosition = namePosition;

		// connect signals
		_ = hitbox.Connect(LookBox.SignalName.LookedAt, Callable.From(() => OnLookedAt()));
		_ = hitbox.Connect(LookBox.SignalName.LookedAway, Callable.From(() => OnLookedAway()));
	}

	public override void _Process(double delta)
	{
		if (isLookedAt)
		{
			// animates a hover effect
			if (sinVariable >= Mathf.Pi * 2) sinVariable = 0;
			sinVariable += (float)delta * offsetSpeed;
			Position = Vector3.Up * Mathf.Sin(sinVariable) * offsetAmount + basePosition;
			return;
		}
		Position = namePosition;
		sinVariable = 0;
	}

	private void OnLookedAt()
	{
		SetNameDirection();
		Tween showAnimation = CreateTween();
		showAnimation.TweenProperty(this, nameof(Scale).ToLower(), Vector3.One, 0.25f);
		showAnimation.TweenCallback(Callable.From(() => isLookedAt = true));
		showAnimation.Play();
	}

	private void OnLookedAway()
	{
		Rotation = Vector3.Zero;
		Tween hideAnimation = CreateTween();
		hideAnimation.TweenProperty(this, nameof(Scale).ToLower(), Vector3.Zero, 0.25f);
		hideAnimation.TweenCallback(Callable.From(() => isLookedAt = false));
		hideAnimation.Play();
	}

	private void SetNameDirection()
	{
		if (localPlayer != null)
		{
			LookAt(localPlayer.GlobalPosition);
			Rotation = new Vector3(0, Rotation[1], 0);
			Rotation += Vector3.Up * Mathf.DegToRad(180); 
			Rotation += Vector3.Right * Mathf.DegToRad(tiltAngle);
			// GD.Print("rotations: ", Rotation);
			return;
		}

		foreach (Player player in GameDriver.Players)
		{
			if (player.IsRemotePlayer) continue;
			localPlayer = player;
			localPlayer.Connect(Node.SignalName.TreeExiting, Callable.From(() => localPlayer = null));

			LookAt(player.GlobalPosition);

			float fullRotation = 180;
			Rotation += Vector3.Up * Mathf.DegToRad(fullRotation);
			Rotation += Vector3.Right * Mathf.DegToRad(tiltAngle);
			// GD.Print("rotations: ", Rotation);
			return;
		}
	}
}
