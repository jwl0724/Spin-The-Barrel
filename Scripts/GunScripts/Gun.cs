using Godot;
using System.Linq;

public partial class Gun : Node3D, IInteractableEntity {
	// SIGNALS
	[Signal] public delegate void OnShootEventHandler();
	[Signal] public delegate void SpinBarrelEventHandler();
	[Signal] public delegate void GunResetEventHandler();
	[Signal] public delegate void NewHolderEventHandler();
	[Signal] public delegate void PickedUpEventHandler();
	[Signal] public delegate void DroppedEventHandler();

	// STATIC CONSTANTS
	private static readonly string HITBOX_NODE_NAME = "HitBox";
	private static readonly string MODEL_MANAGER_NODE_NAME = "Model";

	// VARIABLES
	public Player Holder { get; private set; } = null;
	public bool InAimMode { get; private set; } = false;
	private LookBox hitbox;
	private GunModelManager model;
	public static readonly int DEFAULT_DAMAGE = 1;
    private readonly bool[] chamber = new bool[6];
	private int currentDamage = DEFAULT_DAMAGE;
	private int chamberIndex;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		chamberIndex = (int) (GD.Randi() % chamber.Length);
		hitbox = GetNode<LookBox>(HITBOX_NODE_NAME);
		model = GetNode<GunModelManager>(MODEL_MANAGER_NODE_NAME);
	}

	public void UpdateGunState(Godot.Collections.Array<bool> chamberArray, int chamberIndex, int damage, Player holder) {
		int currentBulletsInChamber = 0, newBulletsInChamber = 0;
		foreach(bool bullet in chamber) {
			if (bullet) currentBulletsInChamber++;
		}
		for(int i = 0; i < chamberArray.Count; i++) {
			chamber[i] = chamberArray[i];
			if (chamber[i]) newBulletsInChamber++;
		}
		// update is called on new turns 
		if (currentBulletsInChamber < newBulletsInChamber && newBulletsInChamber != 1) 
			model.PlaySpinBarrel();
		this.chamberIndex = chamberIndex;
		currentDamage = damage;
		Holder = holder;
	}

	public void DoubleGunDamage() {
		currentDamage *= 2;
	}

	public void Shoot(Player player = null) {
		if (model.IsPlayingAnimation) return;
		BroadcastShoot();
		bool hasBullet = chamber[chamberIndex++];
		if (chamberIndex >= chamber.Length) chamberIndex = 0;
		model.PlayShoot(player, Callable.From(() => {
			EmitSignal(SignalName.OnShoot, hasBullet);
			if (!hasBullet) {
				Holder.EndTurn();
				return;
			}
			if (player != null) player.DamagePlayer(currentDamage);
			else Holder.DamagePlayer(currentDamage);
			Drop();
		}));
	}

	public void ShootAnimationOnly(Player player = null) {
		model.PlayShoot(player, Callable.From(() => {
			EmitSignal(SignalName.OnShoot, chamber[chamberIndex]);
			Drop();
		}));
	}

	public void EnterAimMode() {
		// model uses InAimMode to check if repeated calls are made, need to be called first
		model.EnterAimMode();
		InAimMode = true;
	}

	public void ExitAimMode() {
		model.ExitAimMode();
		InAimMode = false;
	}

	public void LoadRound() {
		if (model.IsPlayingAnimation) return;
		var unloadedIndices = Enumerable.Range(0, chamber.Length).Where(i => !chamber[i]).ToList();
		if (unloadedIndices.Count == 0) return;

		int randomIndex = unloadedIndices[(int) (GD.Randi() % unloadedIndices.Count)];
		chamber[randomIndex] = true;
		model.PlaySpinBarrel();
		BroadcastGunState();
		EmitSignal(SignalName.SpinBarrel);
	}

    public void PickUp() {
		hitbox.ProcessMode = ProcessModeEnum.Disabled;
		Tween liftTween = CreateTween();
		const float tweenTime = 0.2f, liftHeight = 0.25f;
		liftTween.TweenProperty(this, nameof(Position).ToLower(), Position + Vector3.Up * liftHeight, tweenTime);
		liftTween.TweenCallback(Callable.From(() => {
			EmitSignal(SignalName.PickedUp);
			Holder.UpdateGunHold(true);
		}));
		model.PlayPickUp(tweenTime);
		liftTween.Play();
		BroadcastAnimation(true);
	}

	public void PlayInteractAnimation(bool isPickupEvent) {
		const float tweenTime = 0.2f;
		if (isPickupEvent) model.PlayPickUp(tweenTime);
		else model.PlayDrop(tweenTime);
		Tween heightTween = CreateTween();
		float heightFactor = isPickupEvent ? 0.25f : -0.25f;
		heightTween.TweenProperty(this, nameof(Position).ToLower(), Position + Vector3.Up * heightFactor, tweenTime);
		heightTween.Play();
	}

	public void Drop() {
		if (model.IsPlayingAnimation) return;
		Tween dropTween = CreateTween();
		const float tweenTime = 0.2f, liftHeight = 0.25f;
		dropTween.TweenProperty(this, nameof(Position).ToLower(), Position + Vector3.Down * liftHeight, tweenTime);
		dropTween.TweenCallback(Callable.From(() => {
			EmitSignal(SignalName.Dropped);
			Holder.UpdateGunHold(false);
			hitbox.ProcessMode = ProcessModeEnum.Inherit;
		}));
		model.PlayDrop(tweenTime);
		dropTween.Play();
		Holder.DropGun();
		BroadcastAnimation(false);
	}

	public void RollToSafe() {
		var unloadedIndices = Enumerable.Range(0, chamber.Length).Where(i => !chamber[i]).ToList();
		if (unloadedIndices.Count == 0) return;
		int randomIndex = unloadedIndices[(int) (GD.Randi() % unloadedIndices.Count)];
		chamberIndex = randomIndex;
		model.SpinBarrelOnly();
	}

	public void Reroll() {
		chamberIndex = (int) (GD.Randi() % chamber.Length);
		model.SpinBarrelOnly();
	}

	public void SetupNewTurn(Player newHolder) {
		ExitAimMode();
		Holder = newHolder;
		BroadcastGunState();
		EmitSignal(SignalName.NewHolder, newHolder);
	}

	public void SetupNewRound(Player startingPlayer) {
		ExitAimMode();
		Holder = startingPlayer;
		ResetChamber();
		chamberIndex = (int) (GD.Randi() % chamber.Length);
		chamber[(int) (GD.Randi() % chamber.Length)] = true;
		model.SpinBarrelOnly();
		BroadcastGunState();
		EmitSignal(SignalName.SpinBarrel);
		EmitSignal(SignalName.NewHolder, startingPlayer);
	}

	private void ResetChamber() {
		InAimMode = false;
		for (int i = 0; i < chamber.Length; i++) {
			chamber[i] = false;
		}
		currentDamage = DEFAULT_DAMAGE;
	}

    public string GetEntityName() {
        return "Pick Up"; // what's going to be shown on the name text
    }

    public Node3D GetModel() {
		const string modelNodeName = "Model";
        return GetNode<Node3D>(modelNodeName);
    }

    public void Interact() {
		Holder?.PickUpGun();
    }

	private void BroadcastGunState()  {
		// need to notify damage, chamberIndex, chamber
		GameNetwork network = GameNetwork.Instance;
		Godot.Collections.Array sendArray = new();
		foreach(bool bullet in chamber) sendArray.Add(bullet);
		network.Rpc(GameNetwork.MethodName.BroadcastGunState, sendArray, chamberIndex, currentDamage, Holder.NetworkID);
	}

	private void BroadcastAnimation(bool isPickupEvent) {
		GameNetwork network = GameNetwork.Instance;
		network.Rpc(GameNetwork.MethodName.BroadcastGunAnimation, isPickupEvent);
	}

	private void BroadcastShoot() {
		GameNetwork network = GameNetwork.Instance;
		network.Rpc(GameNetwork.MethodName.BroadcastShootAnimation);
	}
}
