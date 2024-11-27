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
	[Signal] public delegate void PlayerHoldGunEventHandler();
	[Signal] public delegate void PlayerInteractEventHandler();

	// CONSTANTS
	public static readonly int DEFAULT_PLAYER_HEATLH = 4;
	public static readonly float SHOOT_DELAY_TIME = 0.75f;

	// VARIABLES
	private CustomTimer timer = new();
	private bool hasDoubleDamage = false; // TODO: add this effect when items are added
	public float MouseSensitivity { get; set; } = 0.05f;
	public int Health { get; private set; } = DEFAULT_PLAYER_HEATLH;
	public bool IsDead { get; private set; } = false;
	public string PlayerName { get; private set; }
	private Gun gun;
	public Gun Gun { set => gun = value; }
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
		SetProcessInput(!IsRemotePlayer);
		timer.Time = SHOOT_DELAY_TIME;
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	public override void _Process(double delta) {

	}

	public override void _Input(InputEvent inputEvent) {
		if (Input.IsActionPressed(ProjectInputs.INTERACT)) {
			EmitSignal(SignalName.PlayerInteract);
		}
	}

	public void SetPlayerInfo(PlayerInfo playerData) {
		Name = playerData.Name;
		IsRemotePlayer = playerData.IsRemote;
	}

	public void SetModel(int modelIndex = -1) {
		int index = modelIndex;
		if (modelIndex == -1) index = (int) (GD.Randi() % ModelManager.GetModelCount());
		ModelManager.SetModel(index);
	}

	public void SpinBarrel() {
		EmitSignal(SignalName.PlayerHoldGun);
		gun.LoadRound();
		// TODO: connect the a signal listener to gun such that it will know when the animation is done playing
	}

	public async void Shoot(Player player = null) {
		EmitSignal(SignalName.PlayerHoldGun, gun.GlobalPosition);
		timer.Start();
		await ToSignal(timer, CustomTimer.SignalName.Timeout);
		if (!gun.Shoot()) return;
		int damage = hasDoubleDamage ? Gun.DEFAULT_DAMAGE * 2 : Gun.DEFAULT_DAMAGE;
		if (player == null) DamagePlayer(damage);
		else player.DamagePlayer(damage);
	}

	public void DamagePlayer(int amount) {
		Health -= amount;
		if (Health < 0) {
			IsDead = true;
			EmitSignal(SignalName.PlayerDied);

		} else EmitSignal(SignalName.PlayerHurt);
	}

	public void ResetPlayerState() {
		Health = DEFAULT_PLAYER_HEATLH;
		IsDead = false;
		EmitSignal(SignalName.PlayerReset);
	}
}
