using Godot;
using System;
using System.Linq;

public partial class LobbyModelManager : ModelManager {
	private static readonly string randomName = "Random";
	private static readonly string dumpNodeName = "Models";
	private static readonly string nameDisplayNodeName = "ModelName";
	private int modelIndex;
	private Node3D dumpNode;
	private Node3D nameNode;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		dumpNode = GetNode<Node3D>(dumpNodeName);
		nameNode = Owner.GetNode<Node3D>(nameDisplayNodeName);
		nameNode.Visible = false;
		foreach(PackedScene model in modelCollection) {
			Node3D modelNode = model.Instantiate<Node3D>();
			modelNode.Visible = false;
			dumpNode.AddChild(modelNode);
		}
	}

	public void NextModel() {
		(dumpNode.GetChild(modelIndex) as Node3D).Visible = false;
		if (++modelIndex == modelCollection.Count + 1) modelIndex = 0;
		(dumpNode.GetChild(modelIndex) as Node3D).Visible = true;
		SetNameText();
	}

	public void PreviousModel() {
		(dumpNode.GetChild(modelIndex) as Node3D).Visible = false;
		if (--modelIndex < 0) modelIndex = modelCollection.Count;
		(dumpNode.GetChild(modelIndex) as Node3D).Visible = true;
		SetNameText();
	}

	public int GetSelected() {
		// account for extra random mesh
		return modelIndex - 1;
	}
	
	public void StartSelector() {
		modelIndex = (int) (GD.Randi() % (modelCollection.Count + 1));
		(dumpNode.GetChild(modelIndex) as Node3D).Visible = true;
		ShowSelected(modelIndex);
		nameNode.Visible = true;
		SetNameText();
	}

	public void EndSelector() {
		(dumpNode.GetChild(modelIndex) as Node3D).Visible = false;
		nameNode.Visible = false;
	}

	public void ShowSelected(int index) {
		foreach(Node3D model in dumpNode.GetChildren().Cast<Node3D>()) {
			if (model == dumpNode.GetChild(index)) {
				model.Visible = true;
				modelIndex = model.GetIndex();
				SetNameText();
			}
			else model.Visible = false;
		}
	}

	private void SetNameText() {
		TextMesh text = nameNode.GetChild<MeshInstance3D>(0).Mesh as TextMesh;
		if (modelIndex == 0) text.Text = randomName;
		else text.Text = GetModelName(modelIndex - 1);
	}
}
