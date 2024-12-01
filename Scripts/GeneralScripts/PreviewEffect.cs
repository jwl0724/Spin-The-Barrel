using Godot;
using System;

public partial class PreviewEffect : Node3D
{
	[Export] private float rotationSpeed = 40f;
	[Export] private float hoverSpeed = 8;
	[Export] private float hoverOffsetAmount = 0.005f;
	private Vector3 basePosition;
	private float angleVariable = 0;

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		basePosition = Position;
		DoHoverEffect(delta);
		DoRotateEffect(delta);
	}

	private void DoHoverEffect(double delta)
	{
		if (angleVariable >= Mathf.Pi * 2) angleVariable = 0;
		angleVariable += (float) delta * hoverSpeed;
		Position = Vector3.Up * Mathf.Sin(angleVariable) * hoverOffsetAmount + basePosition;
	}

	private void DoRotateEffect(double delta)
	{
		float radians = Mathf.DegToRad(rotationSpeed * (float)delta);
		Rotate(Vector3.Up, radians);
	}
}
