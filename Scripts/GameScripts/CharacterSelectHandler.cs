using Godot;
using System;

public partial class CharacterSelectHandler : Node {
	private LobbyDriver driver;
	public override void _Ready() {
		driver = LobbyDriver.Instance;
		driver.Connect(LobbyDriver.SignalName.PlayerJoinLobby, Callable.From(() => OnLobbyChange()));
		driver.Connect(LobbyDriver.SignalName.PlayerLeaveLobby, Callable.From(() => OnLobbyChange()));
	}

	private void OnLobbyChange() {
		for(int i = 0; i < LobbyDriver.MAX_PLAYERS; i++) {
			if (i >= LobbyDriver.Players.Count) (GetChild(i) as CharacterSelector).CloseSelector();
			else (GetChild(i) as CharacterSelector).ActivateSelector(LobbyDriver.Players[i].IsRemote);
		}
	}
}
