using Godot;
using System;

public partial class PreviewEffect : Node3D {
	private float angleVariable = 0;

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		DoHoverEffect(delta);
		DoRotateEffect(delta);
	}

	private void DoHoverEffect(double delta) {
		const int offsetSpeed = 8;
		float offsetAmount = 0.005f;
		if (angleVariable >= Mathf.Pi * 2) angleVariable = 0;
		angleVariable += (float) delta * offsetSpeed;
		Position += Vector3.Up * Mathf.Sin(angleVariable) * offsetAmount;
	}

	private void DoRotateEffect(double delta) {
		const float rotationSpeed = 0.04f;
		Rotate(Vector3.Up, Mathf.DegToRad(angleVariable * (float) delta * rotationSpeed));
	}
}
