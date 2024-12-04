using Godot;
using System;

public abstract partial class MenuItem : Control {
	private readonly Color opaqueColor = new(1, 1, 1, 1);
	private readonly Color transparentColor = new(1, 1, 1, 0);
	protected Godot.Collections.Array<Control> components = new();
	protected float fadeInTime;

	public void GoNextScreen(ScreenManager.ScreenState newState) {
		(GetParent() as MenuManager).NotifyChange(newState);
	}

	public string GetName() {
		return this.Name;
	}

	public virtual void ShowScreen() {
		if (Visible == true) return;
		Visible = true;
		SetProcessInput(true);
		foreach(var component in components) FadeIn(component, fadeInTime);
	}
	
	public virtual void HideScreen() {
		if (Visible == false) return;
		Visible = false;
		SetProcessInput(false);
		foreach(var component in components) FadeOut(component, fadeInTime);
	}
	
	protected void FadeIn(Control node, float time) {
		Tween fadeTween = CreateTween();
		fadeTween.TweenProperty(node, nameof(Modulate).ToLower(), opaqueColor, time);
		fadeTween.Play();
	}

	protected void FadeOut(Control node, float time) {
		Tween fadeTween = CreateTween();
		fadeTween.TweenProperty(node, nameof(Modulate).ToLower(), transparentColor, time);
		fadeTween.Play();
	}
}
