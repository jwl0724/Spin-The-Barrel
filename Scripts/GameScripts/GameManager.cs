using Godot;
using System;

public partial class GameManager : Node3D {
	// SINGLETON
	public static GameManager Instance { get; private set; }

	// STATE VARIABLES
	public enum GameState{
		MAIN_MENU,
		LOBBY,
		IN_GAME,
		POST_GAME
	}

	// VARIABLES
	public GameState State { get; private set; } = GameState.MAIN_MENU;
	private Control gameMenus;
	
	// SIGNALS
	[Signal] public delegate void GameStateChangedEventHandler(GameState newState);

	public override void _Ready() {
		Instance = this;
		
	}

	public override void _Process(double delta) {
	}
}
