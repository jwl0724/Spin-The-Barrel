using Godot;
using System;

public partial class Clickable3D : Area3D {
	// SIGNALS
	[Signal] public delegate void AreaClickedEventHandler();

	public override void _Ready() {
		Connect(SignalName.InputEvent, Callable.From(
			(Node camera, InputEvent inputEvent, Vector3 position, Vector3 normal) =>
				OnInputEvent(camera, inputEvent, position, normal)
		));
	}

	private void OnInputEvent(Node camera, InputEvent inputEvent, Vector3 position, Vector3 normal) {
		if(inputEvent.IsActionPressed(ProjectInputs.INTERACT)) {
			EmitSignal(SignalName.AreaClicked, camera, inputEvent, position,  normal);
		}
	}
}
