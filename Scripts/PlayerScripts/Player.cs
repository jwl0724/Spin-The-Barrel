using Godot;
using System;

public partial class Player : Node3D, IInteractableEntity {
	// EXPORTED VARIABLES
	[Export] public bool IsRemotePlayer; // Player set to false, RemotePlayer set to true
	[Export] public PlayerModelManager ModelManager;

	// SIGNALS
	[Signal] public delegate void PlayerHurtEventHandler();
	[Signal] public delegate void PlayerDiedEventHandler();
	[Signal] public delegate void PlayerResetEventHandler();
	[Signal] public delegate void PlayerStartTurnEventHandler();
	[Signal] public delegate void PlayerEndTurnEventHandler();
	[Signal] public delegate void PlayerPickUpGunEventHandler();
	[Signal] public delegate void PlayerDropGunEventHandler();
	[Signal] public delegate void PlayerInteractEventHandler();
	[Signal] public delegate void NewRoundEventHandler();
	[Signal] public delegate void GameEndEventHandler();

	// CONSTANTS
	public static readonly int DEFAULT_PLAYER_HEATLH = 4;
	public static readonly float SHOOT_DELAY_TIME = 0.75f;

	// VARIABLES
	private bool hasDoubleDamage = false; // TODO: add this effect when items are added
	public float MouseSensitivity { get; set; } = 0.05f;
	public int Health { get; private set; } = DEFAULT_PLAYER_HEATLH;
	public bool IsDead { get; private set; } = false;
	public string PlayerName { get; private set; }
	public bool CanShootOther { get; private set; } = false; // TODO: add this effect when items are added
	public Gun NerfGun { get; private set; } = null;
	private GameDriver driver;
	private int selectedModel = -1;
	public int SelectedModel {
		get => selectedModel;
		set => selectedModel = (0 <= value && value < ModelManager.GetModelCount()) ? value : -1;
	}

	// INTERFACE IMPLEMENTATIONS
	public string GetEntityName() => PlayerName;
	public Node3D GetModel() { 
		return GetNode<Node3D>("Model");
	}
	public void Interact() {
		// TODO: see what this will do later on, maybe just leave empty?
	}

	public override void _Ready() {
		Input.MouseMode = Input.MouseModeEnum.Captured;
		driver = GameDriver.Instance;
		driver.Connect(GameDriver.SignalName.GameOver, Callable.From(() => {
			Input.MouseMode = Input.MouseModeEnum.Visible;
			SetProcessInput(false);
			EmitSignal(SignalName.GameEnd);
		}));
		driver.Connect(GameDriver.SignalName.NewRound, Callable.From((Player holder) => {
			if (holder != this) NerfGun = null;
			EmitSignal(SignalName.NewRound);
		}));
		driver.Connect(GameDriver.SignalName.NewTurn, Callable.From((Player holder) => {
			if (holder != this) NerfGun = null;
		}));
	}

	public override void _Input(InputEvent inputEvent) {
		if (Input.IsActionPressed(ProjectInputs.INTERACT)) {
			EmitSignal(SignalName.PlayerInteract);
		}
	}

	public void SetPlayerInfo(PlayerInfo playerData) {
		PlayerName = playerData.Name;
		IsRemotePlayer = playerData.IsRemote;
		SetProcessInput(!IsRemotePlayer);
		int modelIndex = playerData.ChosenModel >= 0 ? playerData.ChosenModel : (int) (GD.Randi() % ModelManager.GetModelCount());
		ModelManager.SetModel(modelIndex);
	}

	// called on start of new turn to give gun to whoever's turn it is
	public void GiveGun(Gun gun) {
		NerfGun = gun;
		EmitSignal(SignalName.PlayerStartTurn);
	}

	// called when player looks at gun, physically pick up the gun
	public void PickUpGun() {
		if (NerfGun == null) return;
		if (!IsRemotePlayer) Input.MouseMode = Input.MouseModeEnum.Visible;
		NerfGun.PickUp();
	}

	public void DropGun() {
		if (NerfGun == null) return;
		if (!IsRemotePlayer) Input.MouseMode = Input.MouseModeEnum.Captured;
		if (IsRemotePlayer) NerfGun.Drop();
	}

	// this functions is intended to be called in the callback function in gun to let player know the animation is done
	public void UpdateGunHold(bool wasPickedUp) {
		if (wasPickedUp) EmitSignal(SignalName.PlayerPickUpGun);
		else EmitSignal(SignalName.PlayerDropGun);
	}

	// this function is intended to be called by the network interface, emulates player input for shoot
	public void Shoot(Player player = null) {
		if (IsRemotePlayer) NerfGun.Shoot(player);
	}

	// propagate the call to the driver that player was shot
	public void EndTurn() {
		driver.EndTurn();
	}

	public void DamagePlayer(int amount) {
		Health -= amount;
		if (Health <= 0) {
			IsDead = true;
			Input.MouseMode = Input.MouseModeEnum.Visible;
			EmitSignal(SignalName.PlayerDied);

		} else EmitSignal(SignalName.PlayerHurt);
		driver.EndRound();
	}

	// used when items are implemented? depends on how much time left
	public void HealPlayer(int amount) {
		// will allow for negative healing (RNG healing items)
		Health += amount;
		if (Health <= 0) {
			IsDead = true;
			EmitSignal(SignalName.PlayerDied);
		}
	}

	private void ResetPlayerState() {
		Health = DEFAULT_PLAYER_HEATLH;
		IsDead = false;
		EmitSignal(SignalName.PlayerReset);
	}
}
