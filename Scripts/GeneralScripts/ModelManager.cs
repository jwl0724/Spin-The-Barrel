using Godot;
using System.Linq;
using System.Text.RegularExpressions;

public abstract partial class ModelManager : Node3D {
	protected readonly static Godot.Collections.Array<PackedScene> modelCollection = new();
	private static readonly Godot.Collections.Array<string> modelNames = new();
	private static readonly string modelDirectoryPath = "res://Scenes/PlayerModels/";
	static ModelManager() {
		DirAccess modelDirectory = DirAccess.Open(modelDirectoryPath);
		if (modelDirectory == null) GD.PushError("Model directory configuration incorrect");
		var allModels = modelDirectory.GetFiles();
		foreach(string fileName in allModels) {
			PackedScene modelScene = ResourceLoader.Load<PackedScene>(modelDirectoryPath + fileName);
			if (modelScene != null) {
				modelNames.Add(fileName.Replace(".tscn", ""));
				modelCollection.Add(modelScene);
			}
		}
	}
	
	public int GetModelCount() {
		return modelCollection.Count;
	}

	public string GetModelName(int modelIndex) {
		string modelFileName = modelNames[modelIndex];
		string actualName = Regex.Replace(modelFileName, "(\\B[A-Z])", " $1");
		return actualName;
	}
}
