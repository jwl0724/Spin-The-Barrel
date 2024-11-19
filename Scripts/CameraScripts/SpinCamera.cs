using Godot;
using System;

public partial class SpinCamera : Camera3D {
	[Export] private Vector3 rotationPoint;
	[Export] private GameManager.GameState stateToEnable;
	[Export] private float rotationSpeed = 1;

	private GameManager manager;

	public async override void _Ready() {
		// TEMP SOLUTION FIGURE SOMETHING OUT LATER, PROBLEM: MANAGER FIRES SIGNAL BEFORE NODE CONNECTS TO SIGNAL
		await ToSignal(GetTree().Root, SignalName.Ready);
		manager = GameManager.Instance;
		manager.Connect(GameManager.SignalName.GameStateChanged, Callable.From((GameManager.GameState state) => OnStateChanged(state)));
	}

	public override void _Process(double delta) {
		float angle = rotationSpeed * (float) delta;
		Quaternion rotation = new(Vector3.Up, Mathf.DegToRad(angle));
		var offset = GlobalTransform.Origin - rotationPoint;
		offset = rotation * offset;
		GlobalTransform = new Transform3D(GlobalTransform.Basis, offset + rotationPoint);
		LookAt(rotationPoint);
	}

	private void OnStateChanged(GameManager.GameState state) {
		if (state == stateToEnable) {
			ProcessMode = ProcessModeEnum.Inherit;
			Current = true;
			return;
		}
		ProcessMode = ProcessModeEnum.Disabled;
	}
}
