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
	public static readonly List<PlayerInfo> Players;
	public bool IsHost { get; private set; } = false;
	public int LocalPlayerIndex { get; private set; } = -1;

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
		if (info.IsRemote) Players.Add(info);
		else if (LocalPlayerIndex == -1) LocalPlayerIndex = Players.Count;
		else GD.PushError("Trying to create multiple local players");
		EmitSignal(SignalName.PlayerJoinLobby);
	}

	private void OnGameStateChange(ScreenManager.ScreenState state) {
		// TODO: Add some more logic here if needed
		Visible = state == ScreenManager.ScreenState.HOST_LOBBY;
		LocalPlayerIndex = -1;
	}
}
