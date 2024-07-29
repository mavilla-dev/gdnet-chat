using System;
using System.Text.Json;
using Godot;
using static Godot.MultiplayerApi;

public interface IRpcApi {
	public event Action<string> MessageReceived;

	public Error SendRPC<T>(string rpcName, T data);
	public Error SendRPC<T>(long playerId, string rpcName, T data);
	public Error SendRPC(string rpcName, params Variant[] args);
	public Error SendRPC(long playerId, string methodName, params Variant[] args);
	public Error SendServerRPC(string methodName, params Variant[] args);
}

public partial class RpcApiNode : Node, IRpcApi {
	public static IRpcApi Instance;
	public event Action<string> MessageReceived;

	#region Node Lifecycle
	public override void _Ready() {
		Instance = this;
	}
	#endregion Node Lifecycle

	#region RPC Extensions
	// Consider using JSONRPC
	public Error SendRPC<T>(string rpcName, T data) {
		var json = JsonSerializer.Serialize(data);
		return Rpc(rpcName, json);
	}
	public Error SendRPC<T>(long playerId, string rpcName, T data) {
		var json = JsonSerializer.Serialize(data);
		return RpcId(playerId, rpcName, json);
	}
	public Error SendRPC(string methodName, params Variant[] args) {
		return Rpc(methodName, args);
	}
	public Error SendRPC(long playerId, string methodName, params Variant[] args) {
		return RpcId(playerId, methodName, args);
	}
	public Error SendServerRPC(string methodName, params Variant[] args) {
		return RpcId(MultiplayerPeer.TargetPeerServer, methodName, args);
	}
	#endregion RPC Extensions

	[Rpc(RpcMode.AnyPeer, CallLocal = true)]
	public void SendMessageRpc(string msg) {
		MessageReceived?.Invoke($"[{Multiplayer.GetRemoteSenderId()}]: {msg}");
	}

	[Rpc(RpcMode.AnyPeer)]
	public void SendPlayerMyDataRpc(string json) {
		var playerData = JsonSerializer.Deserialize<PlayerData>(json);
		NetworkManagerNode.Instance.InvokePlayerConnected(playerData);
	}
}
