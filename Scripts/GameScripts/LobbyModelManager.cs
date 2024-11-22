using Godot;
using System;

public partial class LobbyModelManager : ModelManager {
	private static readonly string dumpNodeName = "Models";
	private int identifier;
	private int modelIndex;
	private Node3D dumpNode;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		Node3D dumpNode = GetNode<Node3D>(dumpNodeName);
		foreach(PackedScene model in modelCollection) {
			Node3D modelNode = model.Instantiate<Node3D>();
			modelNode.Visible = false;
			dumpNode.AddChild(modelNode);
		}
		LobbyDriver.Instance.Connect(LobbyDriver.SignalName.PlayerJoinLobby, Callable.From(() => OnLobbyJoin()));
		LobbyDriver.Instance.Connect(LobbyDriver.SignalName.PlayerLeaveLobby, Callable.From(() => OnLobbyLeave()));
		SetIdentifier();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		
	}

	public void NextModel() {
		(dumpNode.GetChild(modelIndex) as Node3D).Visible = false;
		if (++modelIndex == modelCollection.Count) modelIndex = 0;
		(dumpNode.GetChild(modelIndex) as Node3D).Visible = true;
	}

	public void PreviousModel() {
		(dumpNode.GetChild(modelIndex) as Node3D).Visible = false;
		if (--modelIndex < 0) modelIndex = modelCollection.Count - 1;
		(dumpNode.GetChild(modelIndex) as Node3D).Visible = true;
	}

	public int GetSelected() {
		// account for extra random mesh
		return modelIndex - 1;
	}

	private void OnLobbyJoin() {
		if (LobbyDriver.Instance.PlayerCount - 1 != identifier) return;
		modelIndex = (int) (GD.Randi() % (modelCollection.Count + 1));
		(dumpNode.GetChild(modelIndex) as Node3D).Visible = true;
		// TODO: add more code when lobby driver is more implemented
	}

	private void OnLobbyLeave() {
		if (LobbyDriver.Instance.PlayerCount + 1 != identifier) return;
		(dumpNode.GetChild(modelIndex) as Node3D).Visible = false;
		// TODO: add more code when lobby driver is more implemented
	}

	private void SetIdentifier() {
		string spawnPointName = GetParent().Name;
		identifier = (int) char.GetNumericValue(spawnPointName[^1]);
	}
}
