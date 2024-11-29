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
	private bool isPickedUp = false;

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
			// TODO: IMPLEMENT LATER WHEN ITEMS ARE ADDED
		}));
		ProcessMode = ProcessModeEnum.Disabled;
		Visible = false;
	}

	private void ShowMenu(bool show) {
		isPickedUp = show;
		if (gun.Holder.IsRemotePlayer) {
			Visible = false;
			SetProcessInput(false);
			ProcessMode = ProcessModeEnum.Disabled;
			shootOtherButton.Visible = false;
			shootOtherButton.SetProcessInput(false);

		} else {
			SetProcessInput(true);
			ProcessMode = ProcessModeEnum.Inherit;
			shootOtherButton.Visible = gun.Holder.CanShootOther;
			shootOtherButton.SetProcessInput(gun.Holder.CanShootOther);

			LookAt(gun.Holder.GlobalPosition);
			Rotation += Vector3.Up * Mathf.DegToRad(90);
			Rotation = new Vector3(0, Rotation.Y, 0);
			Visible = show;
		}
	}
}
