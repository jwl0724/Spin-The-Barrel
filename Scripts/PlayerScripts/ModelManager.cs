using Godot;
using System.Text.RegularExpressions;

public partial class ModelManager : Node3D {
	[Export] private Godot.Collections.Array<PackedScene> modelCollection;
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

	public int GetModelCount() {
		return modelCollection.Count;
	}

	public string GetModelName(int modelIndex) {
		string modelFileName = modelCollection[modelIndex].ResourceName;
		string actualName = Regex.Replace(modelFileName, "(\\B[A-Z])", " $1");
		return actualName;
	}

	public void SetModel(int modelIndex) {
		if (model == null) modelCollection[modelIndex].Instantiate<Node3D>();
	}

	private void OnPlayerReset() {
		model.QueueFree();
		model = null;
	}
}
