using Godot;
using System;

public partial class TextWobble : Label {
	private float scaleOffset = 0.1f;
	private float scaleSpeed = 2f;
	private float rotationOffset = 10f;
	private float rotationSpeed = 3f;
	private float angle = 0;

	public override void _Process(double delta) {
		if (angle >= 2 * Mathf.Pi) angle = 0;
		Scale += Vector2.One * scaleOffset * Mathf.Sin(angle * scaleSpeed) * (float) delta;
		Rotation = rotationOffset * Mathf.Sin(angle * rotationSpeed) * (float) delta;
		GD.Print(angle);
		GD.Print(delta);
		GD.Print("--------------------");
		angle += (float) delta;
	}
}
