using Godot;
using System;

public partial class MainMenu : Control, IMenu {
	// CHILD NODES
	private static readonly string BLUR_RECT_NODE_NAME = "Blur";
	private static readonly string BLUR_SHADER_PARAM = "amount";
	private ShaderMaterial blurEffect;
	private Godot.Collections.Array<Control> components = new();

	// CONSTANTS
	private const float finalBlurAmount = 1f;
	private const float fadeInTime = 2.5f;
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
	}

    public void ShowScreen() {
		if (Visible == true) return;
		Visible = true;
        SetProcessInput(true);
		BlurBackground(true);
		foreach(var component in components) FadeIn(component, fadeInTime);
    }

    public void HideScreen() {
		if (Visible == false) return;
		Visible = false;
		SetProcessInput(false);
		BlurBackground(false);
		foreach(var component in components) FadeOut(component, fadeInTime);
    }

    public string GetName() {
        return this.Name;
    }

	public void GoNextScreen(ScreenManager.ScreenState newState) {
		(GetParent() as MenuManager).NotifyChange(newState);
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

	private void FadeIn(Control node, float time) {
		Tween fadeTween = CreateTween();
		fadeTween.TweenProperty(node, nameof(Modulate).ToLower(), opaqueColor, time);
		fadeTween.Play();
	}

	private void FadeOut(Control node, float time) {
		Tween fadeTween = CreateTween();
		fadeTween.TweenProperty(node, nameof(Modulate).ToLower(), transparentColor, time);
		fadeTween.Play();
	}
}
