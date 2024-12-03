using Godot;
using System;

public partial class JoinLobbyInput : Control {
	private static readonly string LINE_EDIT_NODE_NAME = "Input";
	private static readonly string SUBMIT_BUTTON_NODE_NAME = "Submit";
	private JoinLobbyMenu joinLobbyMenu;
	private LineEdit inputField;

	public override void _Ready() {
		joinLobbyMenu = Owner as JoinLobbyMenu;
		inputField = GetNode<LineEdit>(LINE_EDIT_NODE_NAME);
		inputField.Connect(LineEdit.SignalName.TextSubmitted, Callable.From((string code) => OnSubmit(code)));
		GetNode<Button>(SUBMIT_BUTTON_NODE_NAME).Connect(Button.SignalName.Pressed, Callable.From(
			() => OnSubmitButtonPressed()
		));
	}

	private void OnSubmit(string lobbyCode) {
		joinLobbyMenu.SubmitLobbyCode(lobbyCode);
		LobbyNetwork.Instance.JoinGame(lobbyCode);
	}

	private void OnSubmitButtonPressed() {
		string lobbyCode = inputField.Text;
		lobbyCode = lobbyCode.Trim();
		GD.Print("Joining lobby with code: " + lobbyCode);
		OnSubmit(lobbyCode);
	}
}
