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
	[Signal] public delegate void NewTurnEventHandler();
	[Signal] public delegate void NewRoundEventHandler();
	[Signal] public delegate void GameOverEventHandler();
	[Signal] public delegate void BackToMenuEventHandler();

	// STATIC CONSTANTS
	public static readonly List<Player> Players = new(); // list of players in the game
	private static readonly string CONFETTI_NODE_NAME = "WinPosition/Confetti";
	private static readonly string GUN_POINT_NODE_NAME = "GunPoint";

	// VARIABLES
	private Player currentTurnPlayer;
	private int currentPlayerIndex = 0;
	private GpuParticles3D confetti;

	public override void _Ready() {
		ScreenManager.Instance.Connect(ScreenManager.SignalName.GameStateChanged,
			Callable.From((ScreenManager.ScreenState newState) => OnScreenStateChanged(newState))
		);
		confetti = GetNode<GpuParticles3D>(CONFETTI_NODE_NAME);
		confetti.Emitting = false;
	}

	// FUNCTIONS FOR PLAYER TO CALL IN GAME
	public Vector3 GetCurrentPlayerGunPoint() {
		Node3D spawnPoint = spawnPoints.GetChild<Node3D>(currentPlayerIndex);
		Node3D gunPoint = spawnPoint.GetNode<Node3D>(GUN_POINT_NODE_NAME);
		return gunPoint.GlobalPosition;
	}

	public void EndTurn(bool reverse = false) {
		// reverse indicates to move backwards on Players list
		currentPlayerIndex = reverse ? currentPlayerIndex - 1 : currentPlayerIndex + 1;
		if (currentPlayerIndex < 0) currentPlayerIndex = Players.Count - 1;
		else if (currentPlayerIndex >= Players.Count) currentPlayerIndex  = 0;
		EmitSignal(SignalName.NewTurn, currentTurnPlayer);
	}

	private void StartGame() {
		SpawnPlayers();
		int currentPlayerIndex = (int) (GD.Randi() % Players.Count);
		currentTurnPlayer = Players[currentPlayerIndex];
		EmitSignal(SignalName.NewRound, currentTurnPlayer);
	}

	private void SpawnPlayers() {
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
		if (newState == ScreenManager.ScreenState.IN_GAME) StartGame();
		else if (newState == ScreenManager.ScreenState.POST_GAME) DisplayWinner();
	}
}
