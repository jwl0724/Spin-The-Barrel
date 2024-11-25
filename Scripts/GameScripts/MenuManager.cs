using Godot;
using System;
using System.Linq;

public partial class MenuManager : Control {
	private static readonly string MAIN_MENU = "MainMenu";
	private static readonly string LOBBY_MENU = "LobbyMenu";
	private static readonly string IN_GAME_MENU = "InGameMenu";
	private static readonly string POST_GAME_MENU = "PostGameMenu";
	private ScreenManager screenManager;
	private Godot.Collections.Array<Control> menus;

	public override void _Ready() {
		screenManager.Connect(ScreenManager.SignalName.GameStateChanged, Callable.From((ScreenManager.ScreenState state) => 
			OnGameStateChange(state)
		));
		foreach(Control menu in GetChildren().Cast<Control>()) {
			menus.Add(menu);
		}
	}

	private void OnGameStateChange(ScreenManager.ScreenState state) {
		switch(state) {
			case ScreenManager.ScreenState.MAIN_MENU:
				ToggleMenu(MAIN_MENU);
				break;
			case ScreenManager.ScreenState.LOBBY:
				ToggleMenu(LOBBY_MENU);
				break;
			case ScreenManager.ScreenState.IN_GAME:
				ToggleMenu(IN_GAME_MENU);
				break;
			case ScreenManager.ScreenState.POST_GAME:
				ToggleMenu(POST_GAME_MENU);
				break;
		}
	}

	private void ToggleMenu(string name) {
		foreach(var menu in menus) {
			if (menu.Name == name) {
				menu.Visible = true;
				menu.SetProcessInput(true);
			} else {
				menu.Visible = false;
				menu.SetProcessInput(false);
			}
		}
	}
}
