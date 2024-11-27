using Godot;
using System;

public partial class PostGameMenu : MenuItem {
	private static readonly string BACK_TO_MENU_BUTTON_NODE_NAME = "Back";
	public override void _Ready() {
		GetNode<Button>(BACK_TO_MENU_BUTTON_NODE_NAME).Connect(Button.SignalName.Pressed, Callable.From(
			() => GoNextScreen(ScreenManager.ScreenState.MAIN_MENU)
		));
	}
}
