using Godot;
using System;

public partial class SpinCamera : Camera3D {
	[Export] private Vector3 rotationPoint;
	[Export] private ScreenManager.ScreenState stateToEnable;
	[Export] private float rotationSpeed = 1;

	private ScreenManager manager;

	public override void _Ready() {
		manager = ScreenManager.Instance;
		manager.Connect(ScreenManager.SignalName.GameStateChanged, Callable.From((ScreenManager.ScreenState state) => OnStateChanged(state)));
	}

	public override void _Process(double delta) {
		float angle = rotationSpeed * (float) delta;
		Quaternion rotation = new(Vector3.Up, Mathf.DegToRad(angle));
		var offset = GlobalTransform.Origin - rotationPoint;
		offset = rotation * offset;
		GlobalTransform = new Transform3D(GlobalTransform.Basis, offset + rotationPoint);
		LookAt(rotationPoint);
	}

	private void OnStateChanged(ScreenManager.ScreenState state) {
		if (state == stateToEnable) {
			ProcessMode = ProcessModeEnum.Inherit;
			Current = true;
			return;
		}
		ProcessMode = ProcessModeEnum.Disabled;
	}
}
