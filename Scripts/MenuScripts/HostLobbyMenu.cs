using Godot;
using System;

public partial class HostLobbyMenu : MenuItem {
	private static readonly string DIALOG_BOX_NODE_NAME = "DialogBox";
	private static readonly string START_BUTTON_NODE_NAME = "Start";
	private static readonly string STATUS_TEXT_NODE_NAME = "StatusText";
	private static readonly string NOT_HOST_TEXT_NODE_NAME = "JoinText";
	private static readonly float DOT_TIMER_TIME = 0.25f;
	private static readonly int MIN_PLAYERS = 1; // change this to 1 to test gameplay
	private LobbyDriver lobbyDriver = LobbyDriver.Instance;
	private CustomTimer dotTimer = new();
	private Label statusText;
	private Label notHostText;
	private Button startButton;

	public override void _Ready() {
		// connect signals
		(GetNode(DIALOG_BOX_NODE_NAME) as DialogBox).Connect(DialogBox.SignalName.DialogBoxAction, Callable.From(
			() => GoNextScreen(ScreenManager.ScreenState.MAIN_MENU)
		));
		// set up dot timer
		dotTimer.Connect(CustomTimer.SignalName.Timeout, Callable.From(() => UpdateStatusText()));
		dotTimer.Time = DOT_TIMER_TIME;
		dotTimer.Repeatable = true;
		AddChild(dotTimer);
		dotTimer.Start();

		// connect signals to lobby driver
		lobbyDriver.Connect(LobbyDriver.SignalName.PlayerJoinLobby, Callable.From(() => OnPlayerJoin()));
		lobbyDriver.Connect(LobbyDriver.SignalName.PlayerLeaveLobby, Callable.From(() => OnPlayerLeave()));

		startButton = GetNode<Button>(START_BUTTON_NODE_NAME);
		statusText = GetNode<Label>(STATUS_TEXT_NODE_NAME);
		notHostText = GetNode<Label>(NOT_HOST_TEXT_NODE_NAME);
	}

	private void UpdateStatusText() {
		string text = statusText.Text;
		if (text.EndsWith("...")) text = text.Substring(0, text.Length - 3);
		else text += ".";
		statusText.Text = text;
	}

	// going to assume that this is called when hosts joins
	private void OnPlayerJoin() {
		if (LobbyDriver.Players.Count < MIN_PLAYERS) return;
		dotTimer.Stop();
		statusText.Text = lobbyDriver.IsHost ? "Game Ready to Start" : "Waiting For Host to Start";
		startButton.Visible = lobbyDriver.IsHost;
		notHostText.Visible = !lobbyDriver.IsHost;
	}

	private void OnPlayerLeave() {
		statusText.Text = "Waiting For Players";
		dotTimer.Start();
		startButton.Disabled = LobbyDriver.Players.Count >= 2;
	}
}
