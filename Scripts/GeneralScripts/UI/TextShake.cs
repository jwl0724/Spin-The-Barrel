using Godot;
using System;

public partial class TextShake : Label {
	[Export] private int shakeDegrees = 20;
	[Export] private float shakeForTime = 0.2f;

	public void Shake() {
		float rotateAmount = Mathf.DegToRad(shakeDegrees), time = shakeForTime / 3;
		Tween shakeTween = CreateTween();
		shakeTween.TweenProperty(this, nameof(Rotation).ToLower(), rotateAmount, time);
		shakeTween.TweenProperty(this, nameof(Rotation).ToLower(), -rotateAmount, time);
		shakeTween.TweenProperty(this, nameof(Rotation).ToLower(), 0, time);
		shakeTween.Play();
	}
}
