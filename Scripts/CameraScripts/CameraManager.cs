using Godot;
using System;

public partial class CameraManager : Node {
	public static CameraManager Instance { get; private set; }
	public enum Camera {
		BACKGROUND_CAMERA,
		LOBBY_CAMERA,
		DEAD_CAMERA,
		POST_GAME_CAMERA,
		PLAYER_CAMERA
	}

	private Camera3D backgroundCamera;
	private Camera3D deadCamera;
	private Camera3D lobbyCamera;
	private Camera3D postGameCamera;
	private Camera3D playerCamera = null;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		Instance = this;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
	}

	public void AddPlayerCamera(Camera3D camera) {
		playerCamera = camera;
	}

	public static void SetCurrentCamera(Camera camera) {

	}

	
}
