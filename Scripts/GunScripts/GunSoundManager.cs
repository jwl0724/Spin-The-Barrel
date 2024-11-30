using Godot;

public partial class GunSoundManager : AudioStreamPlayer3D {
	[Export] private AudioStream shootSoundEffect;
	[Export] private AudioStream blankSoundEffect;
	[Export] private AudioStream barrelSpinSoundEffect;
	private Gun gun;

	public override void _Ready() {
		gun = Owner as Gun;
		gun.Connect(Gun.SignalName.OnShoot, Callable.From((bool hasBullet) => OnShoot(hasBullet)));
		gun.Connect(Gun.SignalName.SpinBarrel, Callable.From(() => OnSpinBarrel()));
	}

	private void OnShoot(bool hasBullet) {
		Stream = hasBullet ? shootSoundEffect : blankSoundEffect;
		PitchScale = (float) GD.RandRange(0.95, 1.05);
		Play();
	}

	private void OnSpinBarrel() {
		PitchScale = (float) GD.RandRange(0.95, 1.05);
		Stream = barrelSpinSoundEffect;
		Play();
	}
}
