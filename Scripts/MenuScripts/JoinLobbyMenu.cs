using Godot;
using System;
using System.Linq;

public partial class JoinLobbyMenu : MenuItem {
	[Signal] public delegate void LobbyCodeSubmitEventHandler();
	[Signal] public delegate void JoinRequestUpdateEventHandler();

	private static readonly string BLUR_RECT_NODE_NAME = "Blur";
	private static readonly string DIALOG_BOX_NODE_NAME = "DialogBox";
	private static readonly string BACK_BUTTON_NODE_NAME = "Back";
	private readonly Color opaqueColor = new(1, 1, 1, 1);
	private readonly Color transparentColor = new(1, 1, 1, 0);

	public override void _Ready() {
		fadeInTime = 0.3f;
		foreach(Control component in GetChildren().Cast<Control>()) {
			if (component.Name == BLUR_RECT_NODE_NAME || component.Name == DIALOG_BOX_NODE_NAME) 
				continue;
			component.Modulate = transparentColor;
			components.Add(component);
		}
		// connect back button
		GetNode<Button>(BACK_BUTTON_NODE_NAME).Connect(Button.SignalName.Pressed, Callable.From(
			() => GoNextScreen(ScreenManager.ScreenState.MAIN_MENU)
		));
	}

	// when networking is implemented, use this to interface with network code
	public void SubmitLobbyCode(string code) {
		GoNextScreen(ScreenManager.ScreenState.HOST_LOBBY);
		// EmitSignal(SignalName.LobbyCodeSubmit, code);
	}

	public void UpdateJoinRequest(bool success) {
		EmitSignal(SignalName.JoinRequestUpdate, success);
	}
}
