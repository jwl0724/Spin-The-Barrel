using Godot;

public partial class ItemSoundEffectManager : AudioStreamPlayer3D {
	public static readonly float MAX_LENGTH = 0.75f; // the longest audio clip in seconds
	[Export] private static readonly AudioStream spawnSoundEffect;
	[Export] private static readonly Godot.Collections.Array<AudioStream> itemUsedSoundEffect;
	[Export] private static readonly Godot.Collections.Array<AudioStream> itemDestroyedSoundEffect;
	private enum SoundType { SPAWN, USE, DESTROY }
	private ItemManager item;

	public override void _Ready() {
		item = Owner as ItemManager;
		item.Connect(ItemManager.SignalName.ItemSpawned, Callable.From(() => PlaySound(SoundType.SPAWN)));
		item.Connect(ItemManager.SignalName.ItemUsed, Callable.From(() => PlaySound(SoundType.USE)));
		item.Connect(ItemManager.SignalName.ItemDestroyed, Callable.From(() => PlaySound(SoundType.DESTROY)));
	}

	private void PlaySound(SoundType type) {
		switch (type) {
			case SoundType.SPAWN:
				Stream = spawnSoundEffect;
				Play();
				break;
			case SoundType.USE:
				Stream = itemUsedSoundEffect[(int) (GD.Randi() % itemUsedSoundEffect.Count)];
				Play();
				break;
			case SoundType.DESTROY:
				Stream = itemDestroyedSoundEffect[(int) (GD.Randi() % itemDestroyedSoundEffect.Count)];
				Play();
				break;
		}
	}
}
