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

	// Singleton Constructor for early initialization
	GameManager() {
		Instance = this;
	}

	// VARIABLES
	public GameState State { get; private set; } = GameState.MAIN_MENU;
	private Control gameMenus;
	
	// SIGNALS
	[Signal] public delegate void GameStateChangedEventHandler(GameState newState);

	public override void _Ready() {
		EmitSignal(SignalName.GameStateChanged, (int) GameState.MAIN_MENU);
	}

	public override void _Process(double delta) {
	}
}
