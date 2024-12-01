using Godot;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public partial class ItemManager : Node3D, IInteractableEntity {
	// Model collections data
	private static readonly string ITEM_MODEL_DIRECTORY = "res://Scenes/ItemModels/";
	private static readonly string ITEM_SCRIPT_PATH = "res://Scripts/ItemScripts/Items/";
	private static readonly Godot.Collections.Array<PackedScene> itemModels; 
	// can't export static variables, need to manually preload them
	static ItemManager() {
		itemModels = new();
		DirAccess modelDirectory = DirAccess.Open(ITEM_MODEL_DIRECTORY);
		if (modelDirectory == null) GD.PushError("Model directory configuration incorrect");
		var allModels = modelDirectory.GetFiles();
		foreach(string fileName in allModels) {
			PackedScene modelScene = ResourceLoader.Load<PackedScene>(ITEM_MODEL_DIRECTORY + fileName);
			if (modelScene != null) itemModels.Add(modelScene);
		}
	}

	// Hard coding the values of their respective models
	private static readonly Dictionary<string, string> descriptionKey = new() { // model name key, model description value
		{"Broccoli", "Restores 1 health"},
		{"Chemical", "50% Chance to take damage, 50% chance to heal"},
		{"Coffee", "Double the damage of the next dart"},
		{"Shield", "Blocks damage for this turn"},
		{"Ice Cream", "Skips this turn"},
		{"Finger", "Choose who goes next"},
		{"Slime", "Shoot someone else instead"},
		{"Dice", "Re-randomize chamber position"},
		{"Compass", "Reverse the turn order for this round"},
		{"Map", "Puts chamber position to safe position, fails if no safe position"}
	};
	private static readonly Dictionary<string, Script> scriptKey = new() { // model name key, associated script value, script needs to implement IItem
		{"Broccoli", ResourceLoader.Load<Script>(ITEM_SCRIPT_PATH + "Broccoli.cs")},
		{"Chemical", ResourceLoader.Load<Script>(ITEM_SCRIPT_PATH + "Chemical.cs")},
		{"Coffee", ResourceLoader.Load<Script>(ITEM_SCRIPT_PATH + "Coffee.cs")},
		{"Shield", ResourceLoader.Load<Script>(ITEM_SCRIPT_PATH + "Shield.cs")},
		{"Ice Cream", ResourceLoader.Load<Script>(ITEM_SCRIPT_PATH + "IceCream.cs")},
		{"Finger", ResourceLoader.Load<Script>(ITEM_SCRIPT_PATH + "Finger.cs")},
		{"Slime", ResourceLoader.Load<Script>(ITEM_SCRIPT_PATH + "Slime.cs")},
		{"Dice", ResourceLoader.Load<Script>(ITEM_SCRIPT_PATH + "Dice.cs")},
		{"Compass", ResourceLoader.Load<Script>(ITEM_SCRIPT_PATH + "Compass.cs")},
		{"Map", ResourceLoader.Load<Script>(ITEM_SCRIPT_PATH + "Map.cs")}
	};

	private static readonly string USE_ANIMATION = "Use";
	private static readonly string MODEL_NODE_NAME = "Model";
	private static readonly string SCRIPT_PLACEMENT_NODE_NAME = "ItemEffect";
	private static readonly string ANIMATION_PLAYER_NODE_NAME = "Animator";

	private GameDriver gameDriver;
	private AnimationPlayer animator;
	private Node scriptPlacementNode;
	private Player holder;
	private Node3D model;
	private string itemDescription;
	private string itemName;

	[Signal] public delegate void ItemUsedEventHandler();
	[Signal] public delegate void ItemSpawnedEventHandler();
	[Signal] public delegate void ItemDestroyedEventHandler();

	public override void _Ready() {
		gameDriver = GameDriver.Instance;
		scriptPlacementNode = GetNode(SCRIPT_PLACEMENT_NODE_NAME);
		animator = GetNode<AnimationPlayer>(ANIMATION_PLAYER_NODE_NAME);
		PrepareItem();
		EmitSignal(SignalName.ItemSpawned);
	}

    public string GetEntityName() {
        return itemName;
    }

    public Node3D GetModel() {
        return model;
    }

    public void Interact() {
		EmitSignal(SignalName.ItemUsed);
		animator.Play(USE_ANIMATION);
		animator.Connect(AnimationPlayer.SignalName.AnimationFinished, Callable.From((StringName name) => {
			if (name == USE_ANIMATION) DestroyItem();
			(scriptPlacementNode as IItem).Use(gameDriver, holder);
		}));
    }

	private void DestroyItem() {
		Visible = false;
		EmitSignal(SignalName.ItemDestroyed);
		
		// create delay to let audio finish
        CustomTimer deleteDelay = new() {
            Time = ItemSoundEffectManager.MAX_LENGTH,
            Repeatable = false
        };
		deleteDelay.Connect(CustomTimer.SignalName.Timeout, Callable.From(QueueFree));
        AddChild(deleteDelay);
		deleteDelay.Start();
	}

    private void PrepareItem() {
		int randomIndex = (int) (GD.Randi() % itemModels.Count);
		model = itemModels[randomIndex].Instantiate<Node3D>();
		itemName = Regex.Replace(model.Name, "(\\B[A-Z])", " $1"); // puts space in camel case
		itemDescription = descriptionKey[itemName];
		scriptPlacementNode.SetScript(scriptKey[itemName]);
		GetNode<Node3D>(MODEL_NODE_NAME).AddChild(model);
	}
}
