using Godot;
using System;

public partial class GunManager : Node {
	private static readonly string GUN_NODE_NAME = "Gun";
	private GameDriver driver;
	private Gun gun;
	private Player holder = null;
	
	public override void _Ready() {
		gun = GetNode<Gun>(GUN_NODE_NAME);
		driver = GetParent<GameDriver>();
		driver.Connect(GameDriver.SignalName.NewTurn, Callable.From((Player holder) => OnNewTurn(holder)));
	}

	private void OnNewTurn(Player holder) {
		const float delay = 0.3f;
		this.holder = holder;
		Tween slide = CreateTween();
		slide.TweenProperty(gun, nameof(gun.Position).ToLower(), driver.GetCurrentPlayerGunPoint(), 0.4f);
		slide.TweenInterval(delay);
		slide.TweenCallback(Callable.From(() => {
			holder.GiveGun(gun);
		}));
		slide.Play();
	}
}
