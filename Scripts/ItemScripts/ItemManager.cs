using Godot;
using System;
using System.Collections.Generic;


// PLANNED ITEMS

// HEALING - HEAL BY 1 -> BROCCOLI

// GAMBLE HEALING - 50% DAMAGE 1, 25% HEAL 1, 25% FULL HEAL -> LAB BOTTLE

// DAMAGE BOOST - x2 NEXT BULLET -> COFFEE

// DAMAGE NULLIFIER - BLOCK BULLET -> WOOD SHIELD

// TURN SKIP - SKIP TURN -> ICE CREAM

// TURN CHOOSE - CHOOSE SOMEONE TO GO NEXT -> FOAM FINGER

// OFFENSE - ALLOW PLAYER TO SHOOT SOMEONE ELSE -> SLIME

// REROLL - ALLOW PLAYER TO RE-POSITION GUN -> DICE

// FLIP ORDER - GO BACKWARDS IN TURN -> RUBIKS CUBE

// FORTUNTE - ROLL CHAMBER TO GUARANTEED SAFE POSITION -> MAP

public partial class ItemManager : Node3D, IInteractableEntity {
	// Model collections data
	private static readonly string ITEM_SCRIPT_PATH = "res://Scripts/ItemScripts/Items";
	[Export] private static readonly Godot.Collections.Array<Node3D> itemModels;

	// Hard coding the values of their respective models
	private static readonly Dictionary<string, string> descriptionKey = new() { // model name key, model description value
		
	};
	private static readonly Dictionary<string, Script> scriptKey = new() { // model name key, associated script value, script needs to implement IItem

	};

	private static readonly string MODEL_NODE_NAME = "Model";
	private static readonly string SCRIPT_PLACEMENT_NODE_NAME = "ItemEffect";

	private GameDriver gameDriver;
	private Player holder;
	private string itemName;
	private string itemDescription;
	private Node scriptPlacementNode;
	private Node3D model;

	[Signal] public delegate void ItemUsedEventHandler();
	[Signal] public delegate void ItemSpawnedEventHandler();
	[Signal] public delegate void ItemDestroyedEventHandler();

	public override void _Ready() {
		gameDriver = GameDriver.Instance;
		scriptPlacementNode = GetNode(SCRIPT_PLACEMENT_NODE_NAME);
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
		(scriptPlacementNode as IItem).Use(gameDriver, holder);
    }

	public void DestroyItem() {
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
		model = itemModels[randomIndex];
		itemName = model.Name;
		itemDescription = descriptionKey[model.Name];
		scriptPlacementNode.SetScript(scriptKey[model.Name]);
		GetNode<Node3D>(MODEL_NODE_NAME).AddChild(model);
	}
}
