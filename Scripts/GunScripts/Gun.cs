using Godot;
using System.Linq;

public partial class Gun : Node3D {
	// SIGNALS
	[Signal] public delegate void OnShootEventHandler();
	[Signal] public delegate void SpinBarrelEventHandler();
	[Signal] public delegate void GunResetEventHandler();
	[Signal] public delegate void NewHolderEventHandler();

	// VARIABLES
	public Player Holder { get; private set; }
	public static readonly int DEFAULT_DAMAGE = 1;
    private readonly bool[] chamber = new bool[6];
	private int chamberIndex;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		// TODO: create a main game loop that emits signal of new round and game over, where gun needs to listen to the main game loop
		chamberIndex = (int) GD.Randi() % chamber.Length;
		GameDriver.Instance.Connect(GameDriver.SignalName.NewTurn, Callable.From((Player holder) => {
			Holder = holder;
			EmitSignal(SignalName.NewHolder);
		}));
	}

	public bool Shoot() {
		bool hasBullet = chamber[chamberIndex++];
		if (chamberIndex >= chamber.Length) chamberIndex = 0;
		EmitSignal(SignalName.OnShoot, hasBullet);
		return hasBullet;
	}

	public void LoadRound() {
		var excludedCurrentIndexList = Enumerable.Range(0, chamber.Length).Where(num => num != chamberIndex).ToList();
		int randomIndex = excludedCurrentIndexList[(int) GD.Randi() % excludedCurrentIndexList.Count];
		chamber[randomIndex] = true;
		EmitSignal(SignalName.SpinBarrel);
	}

	private void OnNewRound() {
		ResetChamber();
		int chamberIndex = (int) GD.Randi() % chamber.Length;
		chamber[(int) GD.Randi() % chamber.Length] = true;
		EmitSignal(SignalName.SpinBarrel);
	}

	// TODO: Add more stuff once main game loop is finished
	private void OnGameEnd() {
		ResetChamber();
		EmitSignal(SignalName.GunReset);
	}

	private void ResetChamber() {
		for (int i = 0; i < chamber.Length; i++) {
			chamber[i] = false;
		}
	}
}
