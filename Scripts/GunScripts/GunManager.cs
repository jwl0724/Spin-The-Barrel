using Godot;
using System;

public partial class GunManager : Node {
	private static readonly Vector3 GUN_START_POSITION = new(0.525f, 0.976f, -2.945f);
	private static readonly string GUN_NODE_NAME = "Gun";
	private GameDriver driver;
	private Gun gun;
	
	public override void _Ready() {
		gun = GetNode<Gun>(GUN_NODE_NAME);
		driver = GetParent<GameDriver>();
		driver.Connect(GameDriver.SignalName.NewTurn, Callable.From((Player holder) => OnNewTurn(holder)));
		driver.Connect(GameDriver.SignalName.NewRound, Callable.From((Player holder) => OnNewRound(holder)));
		driver.Connect(GameDriver.SignalName.GameOver, Callable.From(OnGameOver));
		driver.Connect(GameDriver.SignalName.BackToMenu, Callable.From(OnExiting));
	}

	private void OnNewTurn(Player holder) {
		const float delay = 0.3f;
		Tween slide = CreateTween();
		slide.TweenProperty(gun, nameof(gun.Position).ToLower(), driver.GetCurrentPlayerGunPoint(), 0.4f);
		slide.TweenInterval(delay);
		slide.TweenCallback(Callable.From(() => {
			holder.GiveGun(gun);
			gun.SetupNewTurn(holder);
		}));
		slide.Play();
	}

	private void OnNewRound(Player startingPlayer) {
		const float delay = 0.3f;
		Tween slide = CreateTween();
		slide.TweenProperty(gun, nameof(gun.Position).ToLower(), driver.GetCurrentPlayerGunPoint(), 0.4f);
		slide.TweenInterval(delay);
		slide.TweenCallback(Callable.From(() => {
			startingPlayer.GiveGun(gun);
			gun.SetupNewRound(startingPlayer);
		}));
		slide.Play();
	}

	// hides the gun in the post-game winner screen
	private void OnGameOver() {
		gun.Visible = false;
	}

	// resets the gun position back to the middle of the table
	private void OnExiting() {
		gun.Visible = true;
		gun.Position = GUN_START_POSITION;
		(gun.GetModel() as GunModelManager).ResetRotation();
	}
}
