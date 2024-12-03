using Godot;
using System;
using System.Collections.Generic;

public partial class GameNetwork : Node {
	public static GameNetwork Instance { get; private set; }
	public static readonly string DEFAULT_IP = "127.0.0.1"; // configure this to something else depending on the store (ex. SteamAPI)
	public static readonly int DEFAULT_PORT = 7000;
	private int MAX_PLAYERS = 4;

	[Signal] public delegate void ModelSwitchEventHandler();


	private LobbyDriver lobbyDriver;
	private GameDriver gameDriver;

	public MultiplayerApi MultiplayerAPIObject { get; private set; }
	GameNetwork() {
		Instance = this;
	}

	private PlayerInfo localPlayerInfo = null;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		MultiplayerAPIObject = GetTree().GetMultiplayer();
		lobbyDriver = LobbyDriver.Instance;
		gameDriver = GameDriver.Instance;

		MultiplayerAPIObject.PeerConnected += OnPlayerConnect;
		MultiplayerAPIObject.PeerDisconnected += OnPlayerDisconnect;
		MultiplayerAPIObject.ConnectedToServer += OnServerConnected;
		MultiplayerAPIObject.ConnectionFailed += OnServerFailConnect;
		MultiplayerAPIObject.ServerDisconnected += OnServerDisconnect;
	}

	public void HostGame() {
		StartServer();
		lobbyDriver.AddPlayer(localPlayerInfo);
		GD.Print("Started Server with ID: ", MultiplayerAPIObject.GetUniqueId());
	}

	public void JoinGame() {
		StartClient();
	}

	// called by both client and server
	public void CloseConnection() {
		MultiplayerAPIObject.MultiplayerPeer.Close();
		MultiplayerAPIObject.MultiplayerPeer = null;
		LobbyDriver.Players.Clear();
		localPlayerInfo = null;
		// put some sort of dialog box to show that the connection was lost
		ScreenManager.Instance.NotifyEnd(ScreenManager.ScreenState.MAIN_MENU);
	}

	private void StartServer(int port = -1) {
		ENetMultiplayerPeer server = new();
		int usedPort = port == -1 ? DEFAULT_PORT : port;
		server.CreateServer(usedPort, MAX_PLAYERS);
		MultiplayerAPIObject.MultiplayerPeer = server;
		localPlayerInfo = new PlayerInfo(MultiplayerAPIObject.GetUniqueId()) {
			IsRemote = false
		};
	}

	private void StartClient(string ipAddress = "", int port = -1) {
		ENetMultiplayerPeer client = new();
		string ip = ipAddress == "" ? DEFAULT_IP : ipAddress;
		int usedPort = port == -1 ? DEFAULT_PORT : port;
		client.CreateClient(ip, usedPort);
		MultiplayerAPIObject.MultiplayerPeer = client;
		localPlayerInfo = new PlayerInfo(MultiplayerAPIObject.GetUniqueId()) {
			IsRemote = false
		};
	}

	// called when a peer joins a server
	private void OnPlayerConnect(long id) {
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
		GD.Print($"Connected As: {MultiplayerAPIObject.GetUniqueId()}");
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

	// LOBBY FUNCTIONS

	// called by on join
	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	private void AddPlayerToLobby(string name) {
		lobbyDriver.AddPlayer(new PlayerInfo(MultiplayerAPIObject.GetRemoteSenderId(), name));
	}

	// called when model is switched by any peer
	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = false, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	public void UpdatePlayerModels(long networkID, int modelIndex) {
		int selector = 0;
		foreach(PlayerInfo player in LobbyDriver.Players) {
			if (player.NetworkID == networkID) {
				selector = LobbyDriver.Players.IndexOf(player);
				break;
			}
		}
		EmitSignal(SignalName.ModelSwitch, selector, modelIndex);
	}

	// IN-GAME FUNCTIONS
	
	// called by host only to start game
	[Rpc(MultiplayerApi.RpcMode.Authority, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	public void HostStartGame() {
		// send host player list to everyone to use
		var array = NetworkHelperFunctions.ConvertPlayersToNetwork(LobbyDriver.Players);
		Rpc(MethodName.ClientStartGame, array);
	}

	// called by clients after host does rpc call
	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	private void ClientStartGame(Godot.Collections.Array<Godot.Collections.Array> data) {
		var playerList = NetworkHelperFunctions.ConvertPlayersFromNetwork(data);
		// standardize the order using the host's list
		playerList[0].IsRemote = true; // host sends their list, they will always be the first on the list
		LobbyDriver.Players.Clear();
		foreach (PlayerInfo player in playerList) {
			LobbyDriver.Players.Add(player);
			if (player.NetworkID == localPlayerInfo.NetworkID) 
				player.IsRemote = false; 
		}
		ScreenManager.Instance.NotifyEnd(ScreenManager.ScreenState.IN_GAME);
	}

	// called when a player input is made
	[Rpc(MultiplayerApi.RpcMode.AnyPeer, TransferMode = MultiplayerPeer.TransferModeEnum.Unreliable)]
	public void SyncRotation(long rotatorID, Vector3 rotation) {
		foreach(Player player in GameDriver.Players) {
			if (player.NetworkID == rotatorID) player.SetRotation(rotation);
		}
	}
}
