using System;
using System.Collections.Generic;
using Godot;

public interface INetworkManager {
	public event Action MpConnectedToServer;
	public event Action<PlayerData> PlayerConnected;
	public event Action<PlayerData> PlayerDisconnected;

	public Error HandleJoinGame(JoinGameEvent e);
	public Error HandleHostGame(string displayName);
  public void InvokePlayerConnected(PlayerData playerData);

  public PlayerData PlayerData { get; }
	public Dictionary<long, PlayerData> ConnectedPlayers { get; }
}

public partial class NetworkManagerNode : Node, INetworkManager {
	// Interface not really needed here but it helps cut down
	// intellisense/auto-complete options
	public static INetworkManager Instance { get; private set; }

	public event Action MpConnectedToServer;
	public event Action<PlayerData> PlayerConnected;
	public event Action<PlayerData> PlayerDisconnected;

	public PlayerData PlayerData { get; set; }
	public Dictionary<long, PlayerData> ConnectedPlayers { get; set; } = new();

	public override void _Ready() {
		Instance = this;
		AddChild(new RpcApiNode());

		// signals
		Multiplayer.ConnectedToServer += () => MpConnectedToServer?.Invoke();
		Multiplayer.PeerConnected += OnPeerConnected;
		Multiplayer.PeerDisconnected += OnPeerDisconnected;
	}

	public Error HandleHostGame(string displayName) {
		var server = new ServerNode();
		AddChild(server);
		var error = server.HostGame();

		PlayerData = new PlayerData {
			DisplayName = displayName,
			Id = Multiplayer.GetUniqueId(),
		};

		return error;
	}
	public Error HandleJoinGame(JoinGameEvent e) {
		var client = new ClientNode();
		AddChild(client);
		var error = client.JoinGame(e);

		PlayerData = new PlayerData {
			DisplayName = e.DisplayName,
			Id = Multiplayer.GetUniqueId(),
		};

		return error;
	}
	public void InvokePlayerConnected(PlayerData playerData) {
		ConnectedPlayers.Add(playerData.Id, playerData);
		PlayerConnected?.Invoke(playerData);
	}

	private void OnPeerConnected(long id) {
		var error = RpcApiNode.Instance.SendRPC(id, nameof(RpcApiNode.SendPlayerMyDataRpc), PlayerData);
		if (error == Error.Ok) return;

		GD.PrintErr(error);
	}

	private void OnPeerDisconnected(long id) {
		PlayerDisconnected?.Invoke(ConnectedPlayers[id]);
		ConnectedPlayers.Remove(id);
	}
}
