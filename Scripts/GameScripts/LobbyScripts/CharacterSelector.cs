using Godot;
using System;


public partial class CharacterSelector : Node3D {
	[Signal] public delegate void ModelSwitchEventHandler();
	private static readonly string MODEL_MANAGER_NODE_NAME = "ModelManager";
	private static readonly string IDENTIFIER_LIGHT_NODE_NAME = "IdentifierLight";
	private static readonly string LEFT_ARROW_NODE_NAME = "LeftArrow";
	private static readonly string RIGHT_ARROW_NODE_NAME = "RightArrow";
	private LobbyModelManager modelManager;
	private SpotLight3D identifierLight;
	private Clickable3D leftArrow;
	private Clickable3D rightArrow;

	public override void _Ready() {
		modelManager = GetNode<LobbyModelManager>(MODEL_MANAGER_NODE_NAME);
		identifierLight = GetNode<SpotLight3D>(IDENTIFIER_LIGHT_NODE_NAME);
		leftArrow = GetNode<Clickable3D>(LEFT_ARROW_NODE_NAME);
		rightArrow = GetNode<Clickable3D>(RIGHT_ARROW_NODE_NAME);

		leftArrow.Connect(Clickable3D.SignalName.AreaClicked, Callable.From(() => OnArrowClicked(true)));
		rightArrow.Connect(Clickable3D.SignalName.AreaClicked, Callable.From(() => OnArrowClicked(false)));
		leftArrow.ProcessMode = ProcessModeEnum.Disabled;
		rightArrow.ProcessMode = ProcessModeEnum.Disabled;

		GameNetwork.Instance.Connect(GameNetwork.SignalName.ModelSwitch, Callable.From((int selector, int modelIndex) => {
			if (selector == GetIndex()) modelManager.ShowSelected(modelIndex + 1);
		}));
	}

	public void ActivateSelector(bool isRemotePlayer = true) {
		SetProcessInput(!isRemotePlayer);
		modelManager.StartSelector();
		identifierLight.Visible = !isRemotePlayer;
		if (!isRemotePlayer) {
			leftArrow.ProcessMode = ProcessModeEnum.Inherit;
			rightArrow.ProcessMode = ProcessModeEnum.Inherit;
		}
		leftArrow.Visible = !isRemotePlayer;
		rightArrow.Visible = !isRemotePlayer;
		GameNetwork.Instance.Rpc(GameNetwork.MethodName.UpdatePlayerModels, 
			LobbyDriver.Players[GetIndex()].NetworkID, modelManager.GetSelected());
		EmitSignal(SignalName.ModelSwitch, GetIndex(), modelManager.GetSelected());
	}

	public void CloseSelector() {
		SetProcessInput(false);
		modelManager.EndSelector();
		identifierLight.Visible = false;
		leftArrow.ProcessMode = ProcessModeEnum.Disabled;
		rightArrow.ProcessMode = ProcessModeEnum.Disabled;
		leftArrow.Visible = false;
		rightArrow.Visible = false;
	}

	private void OnArrowClicked(bool isLeftClicked) {
		if (isLeftClicked) modelManager.PreviousModel();
		else modelManager.NextModel();
		GameNetwork.Instance.Rpc(GameNetwork.MethodName.UpdatePlayerModels, 
			LobbyDriver.Players[GetIndex()].NetworkID, modelManager.GetSelected());
		EmitSignal(SignalName.ModelSwitch, GetIndex(), modelManager.GetSelected());
	}
}
