using Godot;
using System;
using System.Linq;

public partial class GunMenu : Node3D {
	private static readonly string ADD_NODE_NAME = "Add";
	private static readonly string SHOOT_NODE_NAME = "Shoot";
	private static readonly string DROP_NODE_NAME = "Drop";
	private static readonly string SHOOT_OTHER_NODE_NAME = "ShootOther";
	private Gun gun;
	private Clickable3D addButton;
	private Clickable3D shootButton;
	private Clickable3D dropButton;
	private Clickable3D shootOtherButton;

	public override void _Ready() {
		SetProcessInput(false);
		gun = Owner as Gun;
		addButton = GetNode<Clickable3D>(ADD_NODE_NAME);
		shootButton = GetNode<Clickable3D>(SHOOT_NODE_NAME);
		dropButton = GetNode<Clickable3D>(DROP_NODE_NAME);
		shootOtherButton = GetNode<Clickable3D>(SHOOT_OTHER_NODE_NAME);

		gun.Connect(Gun.SignalName.PickedUp, Callable.From(() => ShowMenu(true)));
		gun.Connect(Gun.SignalName.Dropped, Callable.From(() => ShowMenu(false)));
		addButton.Connect(Clickable3D.SignalName.AreaClicked, Callable.From(() => {
			gun.LoadRound();
		}));
		shootButton.Connect(Clickable3D.SignalName.AreaClicked, Callable.From(() => {
			gun.Shoot();
		}));
		dropButton.Connect(Clickable3D.SignalName.AreaClicked, Callable.From(() => {
			gun.Drop();
		}));
		shootOtherButton.Connect(Clickable3D.SignalName.AreaClicked, Callable.From(() => {
			DisableButtons();
			gun.EnterAimMode();
			gun.Holder.ToggleAimMode(true);

			// to read cancel input from player
			ProcessMode = ProcessModeEnum.Inherit;
			SetProcessInput(true);
		}));
		ProcessMode = ProcessModeEnum.Disabled;
		Visible = false;
	}

	private void ShowMenu(bool show) {
		if (!show || gun.Holder.IsRemotePlayer) {
			SetProcessInput(false);
			DisableButtons();
			Visible = false;
			ProcessMode = ProcessModeEnum.Disabled;
			return;

		} else {
			SetProcessInput(true);
			ProcessMode = ProcessModeEnum.Inherit;
			Visible = true;

			// re-enable all of the menu buttons
			if (gun.Holder.CanShootOther)
				DisableButtons(addButton, shootButton, dropButton, shootOtherButton);
			else DisableButtons(addButton, shootButton, dropButton);

			LookAt(gun.Holder.GlobalPosition);
			Rotation += Vector3.Up * Mathf.DegToRad(90);
			Rotation = new Vector3(0, Rotation.Y, 0);
		}
	}

	private void DisableButtons(params Clickable3D[] exceptions) {
		addButton.Visible = exceptions.Contains(addButton);
		shootButton.Visible = exceptions.Contains(shootButton);
		dropButton.Visible = exceptions.Contains(dropButton);
		shootOtherButton.Visible = exceptions.Contains(shootOtherButton);

		addButton.SetProcessInput(exceptions.Contains(addButton));
		shootButton.SetProcessInput(exceptions.Contains(shootButton));
		dropButton.SetProcessInput(exceptions.Contains(dropButton));
		shootOtherButton.SetProcessInput(exceptions.Contains(shootOtherButton));
	}

	public override void _Input(InputEvent inputEvent) {
		if (inputEvent is not InputEventMouseButton) return;
		if (!gun.InAimMode) return;
		if (Input.IsActionJustPressed(ProjectInputs.CANCEL)) {
			gun.Holder.ToggleAimMode(false);
			gun.ExitAimMode();
			ShowMenu(true);
			Input.MouseMode = Input.MouseModeEnum.Visible;
		}
	}
}
