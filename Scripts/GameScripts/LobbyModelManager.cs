using Godot;
using System;

public partial class LobbyModelManager : ModelManager {
	private static readonly string dumpNodeName = "Models";
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
	
	public void StartSelector() {
		modelIndex = (int) (GD.Randi() % (modelCollection.Count + 1));
		(dumpNode.GetChild(modelIndex) as Node3D).Visible = true;
	}

	public void EndSelector() {
		(dumpNode.GetChild(modelIndex) as Node3D).Visible = false;
	}
}
