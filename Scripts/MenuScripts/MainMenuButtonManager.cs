using Godot;
using System;

public partial class MainMenuButtonManager : Control
{
	private static readonly string HOST_BUTTON_NODE_NAME = "Host";
	private static readonly string JOIN_BUTTON_NODE_NAME = "Join";
	private static readonly string QUIT_BUTTON_NODE_NAME = "Quit";
	private MainMenu mainMenu;
	private Button hostButton;
	private Button joinButton;
	private Button quitButton;
	private Lobby lobby;
	private Timer waitForLobbyTimer;

	public override void _Ready()
	{
		lobby = Lobby.Instance;
		mainMenu = Owner as MainMenu;
		hostButton = GetNode(HOST_BUTTON_NODE_NAME) as Button;
		joinButton = GetNode(JOIN_BUTTON_NODE_NAME) as Button;
		quitButton = GetNode(QUIT_BUTTON_NODE_NAME) as Button;


		waitForLobbyTimer = new Timer();
		AddChild(waitForLobbyTimer);
		waitForLobbyTimer.WaitTime = 0.1f;
		waitForLobbyTimer.OneShot = false;
		waitForLobbyTimer.Start();
		waitForLobbyTimer.Timeout += OnLobbyInitializationTimeout;

	}

	private void OnLobbyInitializationTimeout()
	{
		if (Lobby.Instance != null)
		{
			lobby = Lobby.Instance;
			GD.Print("Lobby initialized successfully!");

			hostButton.Connect(Button.SignalName.Pressed, Callable.From(() => MoveToLobby(true)));
			joinButton.Connect(Button.SignalName.Pressed, Callable.From(() => MoveToLobby(false)));
			quitButton.Connect(Button.SignalName.Pressed, Callable.From(() => MoveToQuit()));

			waitForLobbyTimer.Stop();
		}
		else
		{
			GD.PrintErr("Lobby instance is not initialized yet...");
		}
	}

	private void MoveToLobby(bool isHost)
	{
		if (isHost)
		{
			mainMenu.GoNextScreen(ScreenManager.ScreenState.HOST_LOBBY);
			lobby.CreateGame();

		}
		else
		{
			mainMenu.GoNextScreen(ScreenManager.ScreenState.JOIN_LOBBY);
			string serverIp = "127.0.0.1";
			lobby.JoinGame(serverIp);
		}
	}

	private void MoveToQuit()
	{
		mainMenu.GoNextScreen(ScreenManager.ScreenState.QUIT);
	}
}
