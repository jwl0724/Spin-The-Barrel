using Godot;
using System;

public partial class DialogBox : Control {
	// subclassses will have these exports in the editor, do not add anythingn to them in subclasses
	[Export] private Button openButton;
	[Export] private Button closeButton;
	[Export] private Button actionButton;
	[Export] private float transitionSpeed = 0.15f;
	[Signal] public delegate void DialogBoxOpenedEventHandler();
	[Signal] public delegate void DialogBoxClosedEventHandler();
	[Signal] public delegate void DialogBoxActionEventHandler();
    public override void _Ready() {
		CloseDialogBoxImmediate();

		// reset dialogbox state when leaving menu associated with box
		Control parent = GetParent() as Control;
		parent.Connect(Control.SignalName.VisibilityChanged, Callable.From(() => {
			if (!parent.Visible) CloseDialogBoxImmediate();
		}));

		openButton?.Connect(Button.SignalName.Pressed, Callable.From(OpenDialogBox));
		closeButton?.Connect(Button.SignalName.Pressed, Callable.From(CloseDialogBox));
		actionButton?.Connect(Button.SignalName.Pressed, Callable.From(() => EmitSignal(SignalName.DialogBoxAction)));
    }

    protected void OpenDialogBox() {
		Visible = true;
		Tween popUpAnimation = CreateTween();
		popUpAnimation.TweenProperty(this, nameof(Scale).ToLower(), Vector2.One, transitionSpeed);
		popUpAnimation.TweenCallback(Callable.From(() => {
				SetProcessInput(true);
				EmitSignal(SignalName.DialogBoxOpened);
			}
		));
		popUpAnimation.Play();
	}

	protected void CloseDialogBox() {
		SetProcessInput(false);
		Tween closeAnimation = CreateTween();
		closeAnimation.TweenProperty(this, nameof(Scale).ToLower(), Vector2.Zero, transitionSpeed);
		closeAnimation.TweenCallback(Callable.From(() => {
				Visible = false;
				EmitSignal(SignalName.DialogBoxClosed);
			}
		));
		closeAnimation.Play();
	}

	protected void CloseDialogBoxImmediate() {
		SetProcessInput(false);
		Scale = Vector2.Zero;
		Visible = false;
	}
}
