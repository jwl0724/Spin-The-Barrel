using Godot;
using System;

public partial class DeadCamera : Camera3D {
	private GameDriver driver;
	public override void _Ready() {
		driver = GameDriver.Instance;
		driver.Connect(GameDriver.SignalName.NewTurn, Callable.From((Player newPlayer) => SwitchCamera(newPlayer)));
		driver.Connect(GameDriver.SignalName.NewRound, Callable.From((Player newPlayer) => SwitchCamera(newPlayer)));
	}

	private void SwitchCamera(Player nextPlayer) {
		Vector3 originalRotation = Rotation;
		LookAt(nextPlayer.GlobalPosition);
		Vector3 finalRotation = Rotation;
		Rotation = originalRotation;
		Tween tween = CreateTween();
		tween.SetEase(Tween.EaseType.Out);
		tween.TweenProperty(this, nameof(Rotation).ToLower(), finalRotation, 1.5f);
		tween.Play();
	}
}
