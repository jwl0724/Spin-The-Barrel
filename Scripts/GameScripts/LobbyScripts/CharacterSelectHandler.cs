using Godot;
using System;
using System.Linq;

public partial class CharacterSelectHandler : Node3D {
	private LobbyDriver driver;
	public override void _Ready() {
		driver = LobbyDriver.Instance;
		driver.Connect(LobbyDriver.SignalName.PlayerJoinLobby, Callable.From(() => OnLobbyChange()));
		driver.Connect(LobbyDriver.SignalName.PlayerLeaveLobby, Callable.From(() => OnLobbyChange()));
		foreach(CharacterSelector selector in GetChildren().Cast<CharacterSelector>()) {
				selector.Connect(CharacterSelector.SignalName.ModelSwitch, 
				Callable.From((int selectorIndex, int modelIndex) => SetChosenModel(selectorIndex, modelIndex))
			);
		}
	}

	private void OnLobbyChange() {
		for(int i = 0; i < LobbyDriver.MAX_PLAYERS; i++) {
			if (i >= LobbyDriver.Players.Count) (GetChild(i) as CharacterSelector).CloseSelector();
			else (GetChild(i) as CharacterSelector).ActivateSelector(LobbyDriver.Players[i].IsRemote);
		}
	}

	private void SetChosenModel(int selectorIndex, int modelIndex) {
		PlayerInfo updatee = LobbyDriver.Players[selectorIndex];
		updatee.ChosenModel = modelIndex;
	}
}
