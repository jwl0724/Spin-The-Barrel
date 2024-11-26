using Godot;
using System;

public partial class JoinLobbyDialogBox : Control {
	private readonly string CLOSE_BUTTON_NODE_NAME = "Close";
	private JoinLobbyMenu joinLobbyMenu;
	public override void _Ready() {
		joinLobbyMenu = Owner as JoinLobbyMenu;
		joinLobbyMenu.Connect(JoinLobbyMenu.SignalName.JoinRequestUpdate, Callable.From(
			(bool success) => OnRequestUpdate(success)
		));
		SetProcessInput(false);
		Scale = Vector2.Zero;
		Visible = false;
		// connect close button
		GetNode<Button>(CLOSE_BUTTON_NODE_NAME).Connect(Button.SignalName.Pressed, Callable.From(CloseDialogBox));
	}

	private void OnRequestUpdate(bool success) {
		if (!success) OpenDialogBox();
		else CloseDialogBox();
	}

	private void OpenDialogBox() {
		Visible = true;
		Tween popUpAnimation = CreateTween();
		popUpAnimation.TweenProperty(this, nameof(Scale).ToLower(), Vector2.One, 0.5f);
		popUpAnimation.TweenCallback(Callable.From(() => SetProcessInput(true)));
		popUpAnimation.Play();
	}

	private void CloseDialogBox() {
		SetProcessInput(false);
		Tween closeAnimation = CreateTween();
		closeAnimation.TweenProperty(this, nameof(Scale).ToLower(), Vector2.Zero, 0.5f);
		closeAnimation.TweenCallback(Callable.From(() => Visible = false));
		closeAnimation.Play();
	}
}
