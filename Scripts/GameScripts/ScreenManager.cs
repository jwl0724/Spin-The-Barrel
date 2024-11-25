using Godot;
using System;

public partial class ScreenManager : Node3D {
	// SINGLETON
	public static ScreenManager Instance { get; private set; }

	// STATE VARIABLES
	public enum ScreenState {
		MAIN_MENU,
		HOST_LOBBY,
		JOIN_LOBBY,
		IN_GAME,
		POST_GAME,
		QUIT
	}

	// Singleton Constructor for early initialization
	ScreenManager() {
		Instance = this;
	}

	// VARIABLES
	public ScreenState State { get; private set; } = ScreenState.MAIN_MENU;
	private Control gameMenus;
	
	// SIGNALS
	[Signal] public delegate void GameStateChangedEventHandler(ScreenState newState);

	public override void _Ready() {
		EmitSignal(SignalName.GameStateChanged, (int) ScreenState.MAIN_MENU);
	}

	public void NotifyEnd(ScreenState nextState) {
		switch(nextState) {
			case ScreenState.MAIN_MENU:
				EmitSignal(SignalName.GameStateChanged, (int) ScreenState.MAIN_MENU);
				break;
			case ScreenState.HOST_LOBBY:
				EmitSignal(SignalName.GameStateChanged, (int) ScreenState.HOST_LOBBY);
				break;
			case ScreenState.JOIN_LOBBY:
				EmitSignal(SignalName.GameStateChanged, (int) ScreenState.JOIN_LOBBY);
				break;
			case ScreenState.IN_GAME:
				EmitSignal(SignalName.GameStateChanged, (int) ScreenState.IN_GAME);
				break;
			case ScreenState.POST_GAME:
				EmitSignal(SignalName.GameStateChanged, (int) ScreenState.POST_GAME);
				break;
			case ScreenState.QUIT:
				GetTree().Quit();
				break;
		}
	}
}
