using Godot;
using System;

public partial class MainMenu : MenuItem {
	// CHILD NODES
	private static readonly string BLUR_RECT_NODE_NAME = "Blur";
	private static readonly string BLUR_SHADER_PARAM = "amount";
	private ShaderMaterial blurEffect;

	// CONSTANTS
	private const float finalBlurAmount = 1f;
	private readonly Color opaqueColor = new(1, 1, 1, 1);
	private readonly Color transparentColor = new(1, 1, 1, 0);

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		Material blurMaterial = (GetNode(BLUR_RECT_NODE_NAME) as Control).Material;
		blurEffect = blurMaterial as ShaderMaterial;
		foreach(Control component in GetChildren()) {
			if (component.Name == BLUR_RECT_NODE_NAME) continue;
			component.Modulate = transparentColor;
			components.Add(component);
		}
		fadeInTime = 1.5f;
	}

    public override void ShowScreen() {
		base.ShowScreen();
		BlurBackground(true);
    }

    public override void HideScreen() {
		base.HideScreen();
		BlurBackground(false);
    }

	private void BlurBackground(bool doBlur) {
		if (!doBlur) {
			blurEffect.SetShaderParameter(BLUR_SHADER_PARAM, 0);
			return;
		} 
		Tween blurTween = CreateTween();
		blurTween.TweenMethod(Callable.From((float amount) => {
			blurEffect.SetShaderParameter(BLUR_SHADER_PARAM, amount);
		}), 0f, finalBlurAmount, fadeInTime);
		blurTween.Play();
	}
}
