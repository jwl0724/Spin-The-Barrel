using Godot;

public partial class PlayerModelManager : ModelManager {
	private Player player;
	private Node3D model = null;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		player = Owner as Player;
		player.Connect(Player.SignalName.PlayerReset, Callable.From(() => OnPlayerReset()));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
	}

	// Call when multiplayer is implemented, and sync the model with the server, for RemotePlayer
	public void SetRotation(Vector3 newRotation) {
		if (player.IsRemotePlayer) Rotation = newRotation;
	}

	public void SetModel(int modelIndex) {
		model ??= modelCollection[modelIndex].Instantiate<Node3D>();
		AddChild(model);
	}

	private void OnPlayerReset() {
		model.QueueFree();
		model = null;
	}
}
