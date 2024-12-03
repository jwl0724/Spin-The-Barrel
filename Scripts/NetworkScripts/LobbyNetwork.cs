using Godot;
using System;
using System.Collections.Generic;

public partial class LobbyNetwork : Node {
	public static LobbyNetwork Instance { get; private set; }
	public static readonly string DEFAULT_IP = "127.0.0.1"; // configure this to something else depending on the store (ex. SteamAPI)
	public static readonly int DEFAULT_PORT = 7000;
	private int MAX_PLAYERS = 4;

	[Signal] public delegate void ModelSwitchEventHandler();


	private LobbyDriver lobbyDriver;
	private GameDriver gameDriver;

	public MultiplayerApi multiplayer;
	LobbyNetwork() {
		Instance = this;
	}

	private PlayerInfo localPlayerInfo = null;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		multiplayer = GetTree().GetMultiplayer();
		lobbyDriver = LobbyDriver.Instance;
		gameDriver = GameDriver.Instance;

		multiplayer.PeerConnected += OnPlayerConnect;
		multiplayer.PeerDisconnected += OnPlayerDisconnect;
		multiplayer.ConnectedToServer += OnServerConnected;
		multiplayer.ConnectionFailed += OnServerFailConnect;
		multiplayer.ServerDisconnected += OnServerDisconnect;
	}

	public void HostGame() {
		StartServer();
		lobbyDriver.AddPlayer(localPlayerInfo);
		GD.Print("Started Server with ID: ", multiplayer.GetUniqueId());
	}

	public void JoinGame() {
		StartClient();
	}

	// called by both client and server
	public void CloseConnection() {
		Multiplayer.MultiplayerPeer.Close();
		Multiplayer.MultiplayerPeer = null;
		localPlayerInfo = null;
	}

	private void StartServer(int port = -1) {
		ENetMultiplayerPeer server = new();
		int usedPort = port == -1 ? DEFAULT_PORT : port;
		server.CreateServer(usedPort, MAX_PLAYERS);
		multiplayer.MultiplayerPeer = server;
		localPlayerInfo = new PlayerInfo(multiplayer.GetUniqueId()) {
			IsRemote = false
		};
	}

	private void StartClient(string ipAddress = "", int port = -1) {
		ENetMultiplayerPeer client = new();
		string ip = ipAddress == "" ? DEFAULT_IP : ipAddress;
		int usedPort = port == -1 ? DEFAULT_PORT : port;
		client.CreateClient(ip, usedPort);
		multiplayer.MultiplayerPeer = client;
		localPlayerInfo = new PlayerInfo(multiplayer.GetUniqueId()) {
			IsRemote = false
		};
	}

	// called when a peer joins a server
	private void OnPlayerConnect(long id) {
		GD.Print(multiplayer);
		RpcId(id, MethodName.AddPlayerToLobby, localPlayerInfo.Name);
	}

	// called when a client leaves a server
	private void OnPlayerDisconnect(long id) {
		// find disconnecting player, this is for lobby, have a separate handler for in-game
		PlayerInfo disconnectingPlayer = null;
		foreach(PlayerInfo player in LobbyDriver.Players) {
			if (player.NetworkID == id) disconnectingPlayer = player;
		}
		lobbyDriver.RemovePlayer(disconnectingPlayer);
	}

	// called when peer connects to a server successfully (only called by joinee)
	private void OnServerConnected() {
		lobbyDriver.AddPlayer(localPlayerInfo);
		GD.Print($"Connected As: {multiplayer.GetUniqueId()}");
	}

	// called when the server closes itself while connected
	private void OnServerDisconnect() {
		// add some UI thing saying the server closed, for not just print
		GD.Print("Server closed");
		LobbyDriver.Players.Clear();
		CloseConnection();
	}

	// called when failed to connect to server
	private void OnServerFailConnect() {
		// add some UI thing saying you failed to connect, for now just print
		GD.Print("Failed to connect to server");
		CloseConnection();
	}

	// called by everyone else in the game
	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	private void AddPlayerToLobby(string name) {
		lobbyDriver.AddPlayer(new PlayerInfo(multiplayer.GetRemoteSenderId(), name));
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = false, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	public void UpdatePlayerModelsRPC(int selector, int modelIndex) {
		EmitSignal(SignalName.ModelSwitch, selector, modelIndex);
	}
}
