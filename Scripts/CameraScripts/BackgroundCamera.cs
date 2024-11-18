using Godot;
using System;

public partial class BackgroundCamera : Camera3D {
	private static readonly Vector3 TABLE_POSITION = new(0.486f, 1.097f, -2.918f);
	private GameManager manager;
	public override void _Ready() {
		// TODO: fix null reference exception
		manager = GameManager.Instance;
		manager.Connect(GameManager.SignalName.GameStateChanged, Callable.From((GameManager.GameState state) => OnStateChanged(state)));
		Current = true;
	}

	public override void _Process(double delta) {

	}

	private void OnStateChanged(GameManager.GameState state) {
		if (state == GameManager.GameState.MAIN_MENU) {
			Current = true;
		}
	}
}
