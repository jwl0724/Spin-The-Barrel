using Godot;
using System.Net;
using System.Net.Sockets;
using System;
using System.Linq;


public partial class LobbyNetwork : Node
{
    private static LobbyNetwork _instance;

    public static LobbyNetwork Instance
    {
        get
        {
            if (_instance == null)
            {
                GD.PrintErr("LobbyNetwork instance is not initialized yet.");
            }
            return _instance;
        }
    }


    // These signals can be connected to by a UI lobby scene or the game scene.
    [Signal]
    public delegate void PlayerConnectedEventHandler(int peerId, Godot.Collections.Dictionary<string, string> playerInfo);
    [Signal]
    public delegate void PlayerDisconnectedEventHandler(int peerId);
    [Signal]
    public delegate void ServerDisconnectedEventHandler();
    [Signal]
    public delegate void RoomNumberGeneratedEventHandler(string roomNumber);

    private const int Port = 7000;
    private const string DefaultServerIP = "127.0.0.1"; // IPv4 localhost
    private const int MaxConnections = 4;
    private string roomCode = "1234";

    // This will contain player info for every player,
    // with the keys being each player's unique IDs.
    private Godot.Collections.Dictionary<long, Godot.Collections.Dictionary<string, string>> _players = new Godot.Collections.Dictionary<long, Godot.Collections.Dictionary<string, string>>();

    // This is the local player info. This should be modified locally
    // before the connection is made. It will be passed to every other peer.
    // For example, the value of "name" can be set to something the player
    // entered in a UI scene.
    private Godot.Collections.Dictionary<string, string> _playerInfo = new Godot.Collections.Dictionary<string, string>()
    {
        { "Name", "PlayerName" },
    };

    private Godot.Collections.Dictionary<string, ENetMultiplayerPeer> _rooms = new Godot.Collections.Dictionary<string, ENetMultiplayerPeer>();

    private int _playersLoaded = 0;// Track how many players have loaded the scene

    // Called when the node is ready, sets up callback listeners
    public override void _Ready()
    {

        base._Ready();
        GD.Print("ip: ", GetLocalIPAddress());


        _instance = this;
        GD.Print("LobbyNetwork is initialized.");

        GD.Print("Scene root node: " + GetTree().Root.Name);


        var sceneTree = GetTree();
        var lobbyNetworkNode = sceneTree.Root.GetNode("Level/LobbyNetwork");

        if (lobbyNetworkNode != null)
        {
            GD.Print("LobbyNetwork node found.");
        }
        else
        {
            GD.PrintErr("LobbyNetwork node not found.");
        }

        GD.Print("LobbyNetwork Ready");
        // Listen for player connections, disconnections, etc.
        Multiplayer.PeerConnected += OnPlayerConnected;
        Multiplayer.PeerDisconnected += OnPlayerDisconnected;
        Multiplayer.ConnectedToServer += OnConnectOk;
        Multiplayer.ConnectionFailed += OnConnectionFail;
        Multiplayer.ServerDisconnected += OnServerDisconnected;
        GD.Print("Multiplayer ready: " + (Multiplayer != null ? "Initialized" : "Not Initialized"));
        GD.Print("MultiplayerPeer ready: " + (Multiplayer.MultiplayerPeer != null ? "Initialized" : "Not Initialized"));


    }




    // Used for a client to connect to the game server
    public Error JoinGame(string code = "")
    {
        if (string.IsNullOrEmpty(code))
        {
            GD.PrintErr("Room code is empty.");
            return Error.Failed;
        }
        GD.PrintErr(code);
        GD.Print("Room code join: " + _rooms.Keys);
        if (!_rooms.ContainsKey(code))
        {

            GD.PrintErr("Invalid room code.");
            return Error.Failed;
        }
        else
        {
            GD.Print("Room code is valid.");
        }


        var peer = new ENetMultiplayerPeer();
        Error error = peer.CreateClient(GetLocalIPAddress(), Port);

        if (error != Error.Ok)
        {
            return error;
        }

        Multiplayer.MultiplayerPeer = peer;
        return Error.Ok;
    }

    // Used to create the game server
    public Error CreateGame()
    {
        GD.Print("HOST Creating game");
        roomCode = GenerateRoomCode();
        var peer = new ENetMultiplayerPeer();

        // string localIp = GetLocalIPAddress();

        // GD.Print("peer: " + (peer == null ? "null" : "initialized"));
        // GD.Print("roomCode: " + roomCode);

        Error error = peer.CreateServer(Port, MaxConnections);

        if (error != Error.Ok)
        {
            GD.PrintErr("Error creating server: " + error);
            return error;
        }

        // peer.Host.Compress(ENetConnection.CompressionMode.RangeCoder);

        GD.Print("Before setting Multiplayer.MultiplayerPeer");
        GD.Print("Multiplayer: " + (Multiplayer != null ? "Initialized" : "Not Initialized"));

        GD.Print("MultiplayerPeer: " + (Multiplayer.MultiplayerPeer != null ? "Initialized" : "Not Initialized"));

        Multiplayer.MultiplayerPeer = peer;
        GD.Print("After setting Multiplayer.MultiplayerPeer");

        if (Multiplayer.MultiplayerPeer == null)
        {
            GD.PrintErr("MultiplayerPeer is null.");
            return Error.Failed;
        }
        else
        {
            GD.Print("MultiplayerPeer is not null.");
        }
        _players[1] = _playerInfo;
        _rooms[roomCode] = peer;
        GD.Print("Room code: " + _rooms.Keys);
        GD.Print("Room code type: " + roomCode.GetType());
        EmitSignal(SignalName.PlayerConnected, 1, _playerInfo);
        return Error.Ok;
    }

    public string getRoomCode()
    {
        return roomCode;
    }

    private string GenerateRoomCode()
    {
        const string chars = "0123456789";
        var random = new Random();

        string roomCode;
        do
        {
            roomCode = new string(Enumerable.Repeat(chars, 4)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        } while (_rooms.ContainsKey(roomCode)); // Ensure uniqueness
        EmitSignal(nameof(RoomNumberGenerated), roomCode);
        GD.Print("Room code generated: " + roomCode);
        return roomCode;
    }

    // Removes the current MultiplayerPeer (disconnects)
    private void RemoveMultiplayerPeer()
    {
        Multiplayer.MultiplayerPeer = null;
    }

    // When the server decides to start the game from a UI scene,
    // do Rpc(Lobby.MethodName.LoadGame, filePath);
    [Rpc(CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    private void LoadGame(string gameScenePath)
    {
        GetTree().ChangeSceneToFile(gameScenePath);
    }

    // Every peer will call this when they have loaded the game scene.
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    private void PlayerLoaded()
    {
        if (Multiplayer.IsServer())
        {
            _playersLoaded += 1;
            if (_playersLoaded == _players.Count)
            {
                GetNode<Game>("/root/Game").StartGame();
                _playersLoaded = 0;
            }
        }
    }

    // When a peer connects, send them my player info.
    // This allows transfer of all desired data for each player, not only the unique ID.
    private void OnPlayerConnected(long id)
    {
        RpcId(id, MethodName.RegisterPlayer, _playerInfo);
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    private void RegisterPlayer(Godot.Collections.Dictionary<string, string> newPlayerInfo)
    {
        int newPlayerId = Multiplayer.GetRemoteSenderId();
        _players[newPlayerId] = newPlayerInfo;
        EmitSignal(SignalName.PlayerConnected, newPlayerId, newPlayerInfo);
    }

    private void OnPlayerDisconnected(long id)
    {
        _players.Remove(id);
        EmitSignal(SignalName.PlayerDisconnected, id);
    }

    public string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        GD.Print("ip");
        GD.PrintErr(host.AddressList == null ? "null" : "not null");
        throw new Exception("No network adapters with an IPv4 address in the system!");
    }

    private void OnConnectOk()
    {
        int peerId = Multiplayer.GetUniqueId();
        _players[peerId] = _playerInfo;
        EmitSignal(SignalName.PlayerConnected, peerId, _playerInfo);
    }

    private void OnConnectionFail()
    {
        Multiplayer.MultiplayerPeer = null;
    }

    private void OnServerDisconnected()
    {
        Multiplayer.MultiplayerPeer = null;
        _players.Clear();
        EmitSignal(SignalName.ServerDisconnected);
    }
}