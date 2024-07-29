using Godot;

public partial class ClientNode : Node {

	public Error JoinGame(JoinGameEvent ev) {
		var peer = new ENetMultiplayerPeer();
		var err = peer.CreateClient(ev.NetAddress, ev.Port);

		if (err == Error.Ok) {
			GD.Print("Client created");
			Multiplayer.MultiplayerPeer = peer;
		} else {
			GD.PrintErr("Failed to create Client");
		}

		return err;
	}
}
