using Godot;
using System;
using System.Collections.Generic;

public partial class GameDriver : Node {
	// SET UP SINGLETON
	public static GameDriver Instance { get; private set; }
	GameDriver() {
		Instance = this;
	}

	// EXPORT VARIABLES
	[Export] private PackedScene playerScene; // scene for player
	[Export] private PackedScene remotePlayerScene; // scene for remote player
	[Export] private Node spawnPoints; // node for spawn points

	// SIGNALS
	

	// STATIC CONSTANTS
	public static readonly List<Player> Players = new(4); // list of players in the game
	private static readonly string CONFETTI_NODE_NAME = "WinPosition/Confetti";

	// VARIABLES
	public Player currentTurnPlayer;
	private GpuParticles3D confetti;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		ScreenManager.Instance.Connect(ScreenManager.SignalName.GameStateChanged,
			Callable.From((ScreenManager.ScreenState newState) => OnScreenStateChanged(newState))
		);
		confetti = GetNode<GpuParticles3D>(CONFETTI_NODE_NAME);
		confetti.Emitting = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
	}

	// call when game starts, see what happens when networking is done
	private void spawnPlayers() {
		int index = 0;
		foreach(PlayerInfo player in LobbyDriver.Players) {
			Node3D spawnPoint = spawnPoints.GetChild<Node3D>(index++);
			Player spawningPlayer = player.IsRemote ? remotePlayerScene.Instantiate<Player>() : playerScene.Instantiate<Player>();
			spawningPlayer.SetPlayerInfo(player);
			Players.Add(spawningPlayer);
			spawnPoint.AddChild(spawningPlayer);
		}
	}

	private void DisplayWinner() {
		// find winner
		Player winner = null;
		foreach(Player player in Players) {
			if (!player.IsDead) winner = player;
			player.IsRemotePlayer = true;
			player.Visible = !player.IsDead;
		}
		winner.Position = confetti.GlobalPosition;
		confetti.Emitting = true;
	}

	private void DeletePlayers() {
		confetti.Emitting = false;
		foreach(Player player in Players) {
			player.QueueFree();
		}
	}

	private void OnScreenStateChanged(ScreenManager.ScreenState newState) {
		if (newState == ScreenManager.ScreenState.IN_GAME) spawnPlayers();
		else if (newState == ScreenManager.ScreenState.POST_GAME) DisplayWinner();
	}
}
