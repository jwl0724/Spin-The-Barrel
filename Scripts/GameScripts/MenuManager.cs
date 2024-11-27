using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class MenuManager : Control {
	private static readonly string MAIN_MENU = "MainMenu";
	private static readonly string HOST_LOBBY_MENU = "HostLobbyMenu";
	private static readonly string JOIN_LOBBY_MENU = "JoinLobbyMenu";
	private static readonly string POST_GAME_MENU = "PostGameMenu";
	private ScreenManager screenManager;
	private readonly List<MenuItem> menus = new();

	public override void _Ready() {
		screenManager = ScreenManager.Instance;
		screenManager.Connect(ScreenManager.SignalName.GameStateChanged, Callable.From((ScreenManager.ScreenState state) => 
			OnGameStateChange(state)
		));
		foreach(MenuItem menu in GetChildren().Cast<MenuItem>()) {
			menu.HideScreen();
			menus.Add(menu);
		}
	}

	public void NotifyChange(ScreenManager.ScreenState nextState) {
		screenManager.NotifyEnd(nextState);
	}

	private void OnGameStateChange(ScreenManager.ScreenState state) {
		switch(state) {
			case ScreenManager.ScreenState.MAIN_MENU:
				ToggleMenu(MAIN_MENU);
				break;
			case ScreenManager.ScreenState.HOST_LOBBY:
				ToggleMenu(HOST_LOBBY_MENU);
				break;
			case ScreenManager.ScreenState.JOIN_LOBBY:
				ToggleMenu(JOIN_LOBBY_MENU);
				break;
			case ScreenManager.ScreenState.IN_GAME:
				// menu will be tied to player since it needs to read player info
				HideAllMenus();
				break;
			case ScreenManager.ScreenState.POST_GAME:
				ToggleMenu(POST_GAME_MENU);
				break;
		}
	}
	private void HideAllMenus() {
		foreach(var menu in menus) menu.HideScreen();
	}

	private void ToggleMenu(string name) {
		foreach(var menu in menus) {
			if (menu.GetName() == name) menu.ShowScreen();
			else menu.HideScreen();
		}
	}
}
