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

		gun.Connect(Gun.SignalName.NewHolder, Callable.From(OnNewHolder));
		addButton.Connect(Clickable3D.SignalName.AreaClicked, Callable.From(() => {
			gun.LoadRound();
		}));
		shootButton.Connect(Clickable3D.SignalName.AreaClicked, Callable.From(() => {
			gun.Shoot();
		}));
		dropButton.Connect(Clickable3D.SignalName.AreaClicked, Callable.From(() => {
			// TODO: ADD CODE AFTER PROPER GAME LOOP IS IMPLEMENTED
		}));
		shootOtherButton.Connect(Clickable3D.SignalName.AreaClicked, Callable.From(() => {
			// TODO: IMPLEMENT LATER WHEN ITEMS ARE ADDED
		}));
		ProcessMode = ProcessModeEnum.Disabled;
	}

	public override void _PhysicsProcess(double delta) {
		// if (gun.Holder != null) LookAt(gun.Holder.GlobalPosition);
	}

	private void OnNewHolder() {
		// TODO: ADD SIGNAL ON GUN FOR WHEN PLAYER PICKS UP GUN
		if (gun.Holder.IsRemotePlayer) {
			Visible = false;
			SetProcessInput(false);
			ProcessMode = ProcessModeEnum.Disabled;
			return;
		}
		Visible = true;
		ProcessMode = ProcessModeEnum.Inherit;
		shootOtherButton.Visible = gun.Holder.CanShootOther;
		shootOtherButton.SetProcessInput(gun.Holder.CanShootOther);
		LookAt(gun.Holder.GlobalPosition);
	}
}
