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

	CameraManager() {
		Instance = this;
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		// applicable to any scene as long as cameras are named accordingly
		backgroundCamera = GetNode<Camera3D>("BackgroundCamera");
		deadCamera = GetNode<Camera3D>("DeadCamera");
		lobbyCamera = GetNode<Camera3D>("LobbyCamera");
		postGameCamera = GetNode<Camera3D>("PostGameCamera");

		GameManager.Instance.Connect(GameManager.SignalName.GameStateChanged, Callable.From((GameManager.GameState state) => OnGameStateChange(state)));
	}

	public void SwitchToPlayerCamera(Camera3D camera) {
		playerCamera = camera;
		playerCamera.Current = true;		
	}

	public void SwitchToDeadCamera() {
		deadCamera.Current = true;
	}

	private void OnGameStateChange(GameManager.GameState state) {
		switch (state) {
			case GameManager.GameState.MAIN_MENU:
				backgroundCamera.Current = true;
				break;
			case GameManager.GameState.LOBBY:
				lobbyCamera.Current = true;
				break;
			case GameManager.GameState.POST_GAME:
				playerCamera.Current = false;
				playerCamera = null;
				postGameCamera.Current = true;
				break;
		}
	}
}
