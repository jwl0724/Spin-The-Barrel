using Godot;
using System;

public partial class CustomTimer : Node {
	[Signal] public delegate void TimeoutEventHandler();
	public bool IsRunning { get; private set; } = false;
	public bool Repeatable { get; set; } = false;
	public float Time { get; set; } = 0;
	private double ticks = 0;

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		if (!IsRunning) return;
		if (ticks < Time) {
			ticks += delta;
			return;
		}
		IsRunning = Repeatable; // auto sets to false if not repeatable
		ticks = 0;
		EmitSignal(SignalName.Timeout);
	}

	public void Start() {
		IsRunning = true;
		ticks = 0;
	}

	public void Stop() {
		IsRunning = false;
		ticks = 0;
	}
}
