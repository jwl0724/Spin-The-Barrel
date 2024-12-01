using Godot;
using System.Collections.Generic;
using System.Linq;

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
	private static readonly int START_ROUND = 1;
	private static readonly string CONFETTI_NODE_NAME = "WinPosition/Confetti";
	private static readonly string GUN_POINT_NODE_NAME = "GunPoint";

	// VARIABLES
	public bool Reverse { get; set; } = false;
	public int Round { get; private set; } = START_ROUND;
	private Player forcedNextTurnPlayer = null;
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

	// intended to be called by a player, to force the next turn onto someone
	public void SetNextTurn(Player player) {
		forcedNextTurnPlayer = player;
	}

	public Vector3 GetCurrentPlayerGunPoint() {
		Node3D spawnPoint = spawnPoints.GetChild<Node3D>(currentPlayerIndex);
		Node3D gunPoint = spawnPoint.GetNode<Node3D>(GUN_POINT_NODE_NAME);
		return gunPoint.GlobalPosition;
	}

	public void EndTurn() {
		if (forcedNextTurnPlayer != null) {
			currentTurnPlayer = forcedNextTurnPlayer;
			currentPlayerIndex = Players.IndexOf(forcedNextTurnPlayer);
			forcedNextTurnPlayer = null;
			
		} else {
			var aliveIndices = Enumerable.Range(0, Players.Count).Where(i => !Players[i].IsDead).ToList();
			int tempIndex = aliveIndices.IndexOf(currentPlayerIndex);
			tempIndex = Reverse ? tempIndex - 1 : tempIndex + 1; // check if reverse is in effect
			if (tempIndex >= aliveIndices.Count) tempIndex = 0;
			else if (tempIndex < 0) tempIndex = aliveIndices.Count - 1;
			currentPlayerIndex = aliveIndices[tempIndex];
			currentTurnPlayer = Players[currentPlayerIndex];
		}
		EmitSignal(SignalName.NewTurn, currentTurnPlayer);
	}

	public void EndRound() {
		Player winner = GetWinner();
		if (winner != null) {
			DisplayWinner(winner);
			EmitSignal(SignalName.GameOver);
			ScreenManager.Instance.NotifyEnd(ScreenManager.ScreenState.POST_GAME);
			return;
		}
		Round++;
		Reverse = false;
		forcedNextTurnPlayer = null;
		var aliveIndices = Enumerable.Range(0, Players.Count).Where(i => !Players[i].IsDead).ToList();
		currentPlayerIndex = aliveIndices[(int) (GD.Randi() % aliveIndices.Count)];
		currentTurnPlayer = Players[currentPlayerIndex];
		EmitSignal(SignalName.NewRound, currentTurnPlayer);
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

	private void DisplayWinner(Player winner) {
		foreach(Player player in Players) player.Visible = !player.IsDead; // hide dead players
		winner.IsRemotePlayer = true; // disable inputs, even for local in win screen
		winner.GlobalPosition = confetti.GlobalPosition;
		winner.Rotation = Vector3.Up * Mathf.DegToRad(220);
		confetti.Emitting = true;
	}

	private Player GetWinner() {
		Player winner = null;
		foreach(Player player in Players) {
			if (!player.IsDead && winner != null) return null;
			else if (!player.IsDead) winner = player;
		}
		return winner;
	}

	private void DeletePlayers() {
		confetti.Emitting = false;
		foreach(Player player in Players) {
			player.QueueFree();
		}
	}

	private void ResetDriverState() {
		Round = START_ROUND;
		Reverse = false;
		forcedNextTurnPlayer = null;
		DeletePlayers();
		Players.Clear();
		confetti.Emitting = false;
	}

	private void OnScreenStateChanged(ScreenManager.ScreenState newState) {
		if (newState == ScreenManager.ScreenState.IN_GAME) StartGame();
		else if (newState == ScreenManager.ScreenState.POST_GAME) EmitSignal(SignalName.GameOver);
		else if (newState == ScreenManager.ScreenState.MAIN_MENU) {
			EmitSignal(SignalName.BackToMenu);
			ResetDriverState();
		}
	}
}
