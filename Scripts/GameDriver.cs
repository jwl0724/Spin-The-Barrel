using Godot;
using System;
using System.Collections.Generic;

public partial class GameDriver : Node {
	// EXPORT VARIABLES
	[Export] private PackedScene playerScene; // scene for player
	[Export] private PackedScene remotePlayerScene; // scene for remote player
	[Export] private Node spawnPoints; // node for spawn points
	// SIGNALS

	// STATIC CONSTANTS
	public static readonly List<Player> Players = new List<Player>(4); // list of players in the game

	// VARIABLES
	

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		// hard code for now, this will have to be done in the server later
		spawnPlayers();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
	}

	private void spawnPlayers() {
		int randomPlayerPosition = (int) (GD.Randi() % spawnPoints.GetChildCount());
		for(int i = 0; i < spawnPoints.GetChildCount(); i++) {
			Node3D spawnPoint = spawnPoints.GetChild(i) as Node3D;
			if (i == randomPlayerPosition) {
				Player player = playerScene.Instantiate<Player>();
				Players.Add(player);
				spawnPoint.AddChild(player);

			} else {
				Player remotePlayer = remotePlayerScene.Instantiate<Player>();
				Players.Add(remotePlayer);
				spawnPoint.AddChild(remotePlayer);
			}
		}
	}
}
