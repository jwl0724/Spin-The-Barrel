using Godot;
using System;

public partial class LobbyDriver : Node3D {
	// SINGLETON SETUP	
	public static LobbyDriver Instance { get; private set; }
	LobbyDriver() {
		Instance = this;
	}

	// SIGNALS
	[Signal] public delegate void PlayerJoinLobbyEventHandler();
	[Signal] public delegate void PlayerLeaveLobbyEventHandler();

	// VARIABLES
	public int PlayerCount { get; private set; } = 1;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		GameManager.Instance.Connect(GameManager.SignalName.GameStateChanged, 
			Callable.From((GameManager.GameState state) => OnGameStateChange(state))
		);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
	}

	private void OnGameStateChange(GameManager.GameState state) {
		// TODO: Add some more logic here if needed
		Visible = state == GameManager.GameState.LOBBY;
	}
}
