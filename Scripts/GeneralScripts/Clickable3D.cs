using Godot;
using System;

public partial class Clickable3D : Area3D {
	// SIGNALS
	[Signal] public delegate void AreaClickedEventHandler();

	public override void _Input(InputEvent inputEvent) {
		if (!GetParent().IsProcessingInput()) return;
		if (inputEvent is not InputEventMouse mouse) return;
		if(!inputEvent.IsActionPressed(ProjectInputs.INTERACT)) return;
		bool isClicked = CheckClicked(mouse.Position);
		if (isClicked) EmitSignal(SignalName.AreaClicked);
	}

	private bool CheckClicked(Vector2 mousePosition) {
		const int raycastLength = 1000;
		PhysicsDirectSpaceState3D world = GetWorld3D().DirectSpaceState;
		Camera3D currentCam = GetViewport().GetCamera3D();
		Vector3 raycastStart = currentCam.ProjectRayOrigin(mousePosition);
		Vector3 raycastEnd = currentCam.ProjectPosition(mousePosition, raycastLength);
        PhysicsRayQueryParameters3D query = new() {
            CollisionMask = 1,
            CollideWithAreas = true,
            From = raycastStart,
            To = raycastEnd
        };
		const string dictKey = "collider";
        Godot.Collections.Dictionary intersect = world.IntersectRay(query);
		if (intersect.Count == 0) return false;
		Node collider = intersect[dictKey].As<Node>();
		return collider.GetInstanceId() == GetInstanceId();
	}
}
