using Godot;
using System;

public partial class TextWobble : Label {
	[Export] private float scaleOffset = 0.1f;
	[Export] private float scaleSpeed = 2f;
	[Export] private float rotationOffset = 10f;
	[Export] private float rotationSpeed = 3f;
	private float angle = 0;

	public override void _Process(double delta) {
		if (angle >= 2 * Mathf.Pi) angle = 0;
		Scale += Vector2.One * scaleOffset * Mathf.Sin(angle * scaleSpeed) * (float) delta;
		Rotation = rotationOffset * Mathf.Sin(angle * rotationSpeed) * (float) delta;
		angle += (float) delta;
	}
}
