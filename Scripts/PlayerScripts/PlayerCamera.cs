using Godot;
using System;

public partial class PlayerCamera : Camera3D {
	private Player player; // maybe get rid of later? will see if necessary

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		player = Owner as Player;	
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {

	}
}
