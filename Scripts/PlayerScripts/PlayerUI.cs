using Godot;
using System;

public partial class PlayerUI : Control {
	public static readonly float DEAD_SCREEN_DELAY = 7;
	private static readonly string HEALTH_NODE_NAME = "Health";
	private static readonly string NAME_NODE_NAME = "Name";
	private static readonly string SPECTATOR_NODE_NAME = "Spectate";
	private static readonly string DEAD_SCREEN_NODE_NAME = "Spectate";
	private static readonly Color opaqueColor = new(1, 1, 1, 1);
	private static readonly Color transparentColor = new(1, 1, 1, 0);
	private Player player;
	private TextShake healthText;
	private TextShake nameText;
	private Label spectatingText;
	private Control deadScreen;
	public override void _Ready() {
		player = Owner as Player;
		player.Connect(Player.SignalName.PlayerHurt, Callable.From(() => {
			ShakeUI();
			UpdateUI();
		}));
		player.Connect(Player.SignalName.PlayerReset, Callable.From(UpdateUI));
		player.Connect(Player.SignalName.PlayerDied, Callable.From(UpdateUI));

		healthText = GetNode(HEALTH_NODE_NAME) as TextShake;
		nameText = GetNode(NAME_NODE_NAME) as TextShake;
		nameText.Text = player.PlayerName;
		spectatingText = GetNode<Label>(SPECTATOR_NODE_NAME);
		spectatingText.Visible = false;
		deadScreen = GetNode<Control>(DEAD_SCREEN_NODE_NAME);
		deadScreen.Visible = false;
		deadScreen.Modulate = transparentColor;
	}

	private void ShakeUI() {
		healthText.Shake();
		nameText.Shake();
	}

	private void UpdateUI() {
		healthText.Text = $"Health: {player.Health}";
		if (player.IsDead) AnimateDeadScreen();
	}

	private void AnimateDeadScreen() {
		deadScreen.Visible = true;
		float waitTime = DEAD_SCREEN_DELAY / 3;
		Tween deathTween = CreateTween();
		deathTween.TweenProperty(deadScreen, nameof(Modulate).ToLower(), opaqueColor, waitTime);
		deathTween.TweenInterval(waitTime);
		deathTween.TweenProperty(deadScreen, nameof(Modulate).ToLower(), transparentColor, waitTime);
		deathTween.TweenCallback(Callable.From(() => deadScreen.Visible = false));
		deathTween.Play();
	}
}
