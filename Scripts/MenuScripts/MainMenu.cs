using Godot;
using System;

public partial class MainMenu : Control, IMenu {
	// CHILD NODES
	private static readonly string BLUR_RECT_NODE_NAME = "Blur";
	private static readonly string BLUR_SHADER_PARAM = "amount";
	private ShaderMaterial blurEffect;

	// CONSTANTS
	private const float finalBlurAmount = 1f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		Material blurMaterial = (GetNode(BLUR_RECT_NODE_NAME) as Control).Material;
		blurEffect = blurMaterial as ShaderMaterial;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
	}

    public void ShowScreen() {
		if (Visible == true) return;
		Visible = true;
        SetProcessInput(true);
		BlurBackground(true);
    }

    public void HideScreen() {
		if (Visible == false) return;
		Visible = false;
		SetProcessInput(false);
		BlurBackground(false);
    }

    public string GetName() {
        return this.Name;
    }

	private void BlurBackground(bool doBlur) {
		if (!doBlur) {
			blurEffect.SetShaderParameter(BLUR_SHADER_PARAM, 0);
			return;
		} 
		Tween blurTween = CreateTween();
		blurTween.TweenMethod(Callable.From((float amount) => {
			GD.Print(amount);
			blurEffect.SetShaderParameter(BLUR_SHADER_PARAM, amount);
		}), 0f, finalBlurAmount, 2.5f);
		blurTween.Play();
	}
	
}
