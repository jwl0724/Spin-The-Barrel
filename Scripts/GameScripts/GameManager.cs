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

	public void NotifyEnd(GameState nextState) {
		switch(nextState) {
			case GameState.MAIN_MENU:
				EmitSignal(SignalName.GameStateChanged, (int) GameState.MAIN_MENU);
				break;
			case GameState.LOBBY:
				EmitSignal(SignalName.GameStateChanged, (int) GameState.LOBBY);
				break;
			case GameState.IN_GAME:
				EmitSignal(SignalName.GameStateChanged, (int) GameState.IN_GAME);
				break;
			case GameState.POST_GAME:
				EmitSignal(SignalName.GameStateChanged, (int) GameState.POST_GAME);
				break;
		}
	}
}
