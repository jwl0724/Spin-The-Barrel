using Godot;
using System;


// TODO: Add arrows that read input to cycle between models on click
public partial class CharacterSelector : Node3D {
	private static readonly string MODEL_MANAGER_NODE_NAME = "ModelManager";
	private static readonly string IDENTIFIER_LIGHT_NODE_NAME = "IdentifierLight";
	private LobbyModelManager modelManager;
	private SpotLight3D identifierLight;

	public override void _Ready() {
		modelManager = GetNode<LobbyModelManager>(MODEL_MANAGER_NODE_NAME);
		identifierLight = GetNode<SpotLight3D>(IDENTIFIER_LIGHT_NODE_NAME);
	}

	public void ActivateSelector(bool isRemotePlayer = true) {
		modelManager.StartSelector();
		identifierLight.Visible = !isRemotePlayer;
	}

	public void CloseSelector() {
		modelManager.EndSelector();
		identifierLight.Visible = false;
	}
}
