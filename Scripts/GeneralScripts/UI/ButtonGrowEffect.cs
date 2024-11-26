using Godot;
using System;

public partial class ButtonGrowEffect : Button {
	[Export] private float maxSize = 1.2f;
	[Export] private float lerpWeight = 0.3f;
	private bool doGrow = false;
	public override void _Ready() {
		Connect(SignalName.MouseEntered, Callable.From(() => doGrow = true));
		Connect(SignalName.MouseExited, Callable.From(() => doGrow = false));
	}

	public override void _Process(double delta) {
		if (doGrow) Scale = Scale.Lerp(Vector2.One * maxSize, lerpWeight);
		else Scale = Scale.Lerp(Vector2.One, lerpWeight);
	}
}
