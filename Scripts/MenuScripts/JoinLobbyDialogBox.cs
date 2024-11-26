using Godot;
using System;

public partial class JoinLobbyDialogBox : DialogBox {
	private readonly string CLOSE_BUTTON_NODE_NAME = "Close";
	private JoinLobbyMenu joinLobbyMenu;
	public override void _Ready() {
		joinLobbyMenu = Owner as JoinLobbyMenu;
		joinLobbyMenu.Connect(JoinLobbyMenu.SignalName.JoinRequestUpdate, Callable.From(
			(bool success) => OnRequestUpdate(success)
		));
		CloseDialogBoxImmediate();
		GetNode<Button>(CLOSE_BUTTON_NODE_NAME).Connect(Button.SignalName.Pressed, Callable.From(CloseDialogBox));
	}

	private void OnRequestUpdate(bool success) {
		if (!success) OpenDialogBox();
		else CloseDialogBox();
	}
}
