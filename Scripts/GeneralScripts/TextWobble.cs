using Godot;
using System;

public partial class TextWobble : Label
{
	[Export] private float scaleOffset = 0.1f;
	[Export] private float scaleSpeed = 2f;
	[Export] private float rotationOffset = 10f;
	[Export] private float rotationSpeed = 3f;
	private float angle = 0;
	private double startTime;

	public override void _Ready()
	{
		startTime = EngineTimeSeconds();
	}

	public override void _Process(double delta)
	{
		double elapsedTime = EngineTimeSeconds() - startTime;
		float scaleFactor = scaleOffset * Mathf.Sin((float)(elapsedTime * scaleSpeed));
		Scale = Vector2.One * (1 + scaleFactor);

		float rotationAngle = Mathf.DegToRad(rotationOffset) * Mathf.Sin((float)(elapsedTime * rotationSpeed));
		Rotation = rotationAngle;
	}

	private double EngineTimeSeconds()
	{
		return Time.GetTicksMsec() / 1000.0;
	}

}
