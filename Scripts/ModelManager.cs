using Godot;
using System.Text.RegularExpressions;

public abstract partial class ModelManager : Node3D {
	protected readonly static Godot.Collections.Array<PackedScene> modelCollection = new();
	private static readonly string modelDirectoryPath = "res://Scenes/PlayerModels";

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		DirAccess modelDirectory = DirAccess.Open(modelDirectoryPath);
		if (modelDirectory == null) GD.PushError("Model directory configuration incorrect");
		modelDirectory.ListDirBegin();
		string fileName = modelDirectory.GetNext();
		while(fileName != "") {
			if (modelDirectory.CurrentIsDir()) GD.PushError("Model directory should not have any directories inside it");
			else modelCollection.Add(ResourceLoader.Load<PackedScene>(modelDirectoryPath + fileName));
		}
	}
	
	public int GetModelCount() {
		return modelCollection.Count;
	}

	public string GetModelName(int modelIndex) {
		string modelFileName = modelCollection[modelIndex].ResourceName;
		string actualName = Regex.Replace(modelFileName, "(\\B[A-Z])", " $1");
		return actualName;
	}
}
