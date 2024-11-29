using Godot;
using System;

public partial class PlayerUI : Control {
	public static readonly float DEAD_SCREEN_DELAY = 7;
	private static readonly string HEALTH_NODE_NAME = "Health";
	private static readonly string NAME_NODE_NAME = "Name";
	private static readonly string SPECTATOR_NODE_NAME = "Spectate";
	private static readonly string DEAD_SCREEN_NODE_NAME = "DeadScreen";
	private static readonly string STATUS_NODE_NAME = "Status";
	private static readonly string ROUND_TEXT_NODE_NAME = "Round";
	private static readonly Color opaqueColor = new(1, 1, 1, 1);
	private static readonly Color transparentColor = new(1, 1, 1, 0);
	private Player player;
	private Label statusText;
	private Label roundText;
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
		player.Connect(Player.SignalName.PlayerStartTurn, Callable.From(TurnAnimation));
		player.Connect(Player.SignalName.NewRound, Callable.From(UpdateUI));
		player.Connect(Player.SignalName.GameEnd, Callable.From(() => Visible = false));

		healthText = GetNode(HEALTH_NODE_NAME) as TextShake;
		nameText = GetNode(NAME_NODE_NAME) as TextShake;
		roundText = GetNode(ROUND_TEXT_NODE_NAME) as TextShake;
		spectatingText = GetNode<Label>(SPECTATOR_NODE_NAME);
		deadScreen = GetNode<Control>(DEAD_SCREEN_NODE_NAME);
		statusText = GetNode<Label>(STATUS_NODE_NAME);

		nameText.Text = player.PlayerName;
		spectatingText.Visible = false;
		deadScreen.Visible = false;
		deadScreen.Modulate = transparentColor;
		statusText.Visible = false;
		statusText.Scale = Vector2.Zero;
		statusText.Modulate = transparentColor;
	}

	private void ShakeUI() {
		healthText.Shake();
		nameText.Shake();
	}

	private void UpdateUI() {
		healthText.Text = $"Health: {player.Health}";
		roundText.Text = $"Round {GameDriver.Instance.Round}";
		if (player.IsDead) AnimateDeadScreen();
	}

	private void TurnAnimation() {
		const int displayTime = 1;
		statusText.Visible = true;
		statusText.Modulate = opaqueColor;
		Tween turnAnimation = CreateTween();
		turnAnimation.TweenProperty(statusText, nameof(Scale).ToLower(), Vector2.One, 0.25f);
		turnAnimation.TweenInterval(displayTime);
		turnAnimation.TweenProperty(statusText, nameof(Modulate).ToLower(), transparentColor, 0.6f);
		turnAnimation.TweenCallback(Callable.From(() => {
			statusText.Visible = false;
			statusText.Scale = Vector2.Zero;
		}));
		turnAnimation.Play();
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
