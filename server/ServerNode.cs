using System.Collections.Generic;
using Godot;

public partial class ServerNode : Node {
	public Dictionary<long, PlayerData> Players = new();

	public Error HostGame() {
		var peer = new ENetMultiplayerPeer();
		var err = peer.CreateServer(Constants.GAME_PORT);

		if (err == Error.Ok) {
			GD.Print("Server hosted");
			Multiplayer.MultiplayerPeer = peer;
		} else {
			GD.PrintErr("Failed to create server");
		}

		return err;
	}
}
