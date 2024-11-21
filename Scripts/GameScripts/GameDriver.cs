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

	// VARIABLES
	public Player currentTurnPlayer;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
	}

	// call when game starts, see what happens when networking is done
	private void spawnPlayers() {
		int randomPlayerPosition = (int) (GD.Randi() % spawnPoints.GetChildCount());
		for(int i = 0; i < spawnPoints.GetChildCount(); i++) {
			Node3D spawnPoint = spawnPoints.GetChild(i) as Node3D;
			if (i == randomPlayerPosition) {
				Player player = playerScene.Instantiate<Player>();
				player.SetModel(player.SelectedModel);
				Players.Add(player);
				spawnPoint.AddChild(player);

			} else {
				Player remotePlayer = remotePlayerScene.Instantiate<Player>();
				remotePlayer.SetModel(remotePlayer.SelectedModel);
				Players.Add(remotePlayer);
				spawnPoint.AddChild(remotePlayer);
			}
		}
	}
}
