using Godot;
using System;
using System.Collections.Generic;

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
	public static readonly int MAX_PLAYERS = 4;
	public static readonly List<PlayerInfo> Players = new();	
	public bool IsHost { get; private set; } = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		ScreenManager.Instance.Connect(ScreenManager.SignalName.GameStateChanged, 
			Callable.From((ScreenManager.ScreenState state) => OnGameStateChange(state))
		);
	}

	public void RemovePlayer(PlayerInfo info) {
		Players.Remove(info);
		EmitSignal(SignalName.PlayerLeaveLobby);
	}

	public void AddPlayer(PlayerInfo info) {
		if (Players.Count >= MAX_PLAYERS) {
			GD.PushWarning("Attempting to add beyond max players");
			return;
		}
		Players.Add(info);
		IsHost = GameNetwork.Instance.MultiplayerAPIObject.IsServer();
		EmitSignal(SignalName.PlayerJoinLobby);
	}

	private void OnGameStateChange(ScreenManager.ScreenState state) {
		Visible = state == ScreenManager.ScreenState.HOST_LOBBY;
		// clear players from list when exiting the lobbies
		if (state == ScreenManager.ScreenState.MAIN_MENU || state == ScreenManager.ScreenState.JOIN_LOBBY)
			Players.Clear();
	}
}
