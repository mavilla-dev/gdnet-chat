using Godot;

public partial class ServerConnectNode : Control {
	private Button _hostButton;
	private Button _joinButton;
	private LineEdit _ipAddress;
	private LineEdit _displayName;

	#region Node Lifecycle
	public override void _Ready() {
		_hostButton = GetNode<Button>("HostButton");
		_joinButton = GetNode<Button>("HBoxContainer/JoinButton");
		_ipAddress = GetNode<LineEdit>("HBoxContainer/LineEdit");
		_displayName = GetNode<LineEdit>("DisplayName");

		_hostButton.Pressed += OnHostPressed;
		_joinButton.Pressed += OnJoinPressed;

		// events
		NetworkManagerNode.Instance.MpConnectedToServer += OnMpConnectedToServer;
	}
	public override void _ExitTree() {
		NetworkManagerNode.Instance.MpConnectedToServer -= OnMpConnectedToServer;
	}
	#endregion Node Lifecycle

	private void OnHostPressed() {
		if (string.IsNullOrWhiteSpace(_displayName.Text)) return;

		var error = NetworkManagerNode.Instance.HandleHostGame(_displayName.Text.Trim());
		if (error == Error.Ok) {
			SceneManagerNode.Instance.ChangeSceneToChat();
		} else {
			GD.PrintErr($"Failed to create server - {error}");
		}
	}

	private void OnJoinPressed() {
		if (string.IsNullOrWhiteSpace(_displayName.Text)) return;

		var error = NetworkManagerNode.Instance.HandleJoinGame(new JoinGameEvent {
			NetAddress = _ipAddress.Text,
			Port = Constants.GAME_PORT,
			DisplayName = _displayName.Text.Trim(),
		});

		if (error == Error.Ok) {
			// Change scene happens after ConnectedToServer signal
		} else {
			GD.PrintErr($"Failed to join game - {error}");
		}
	}

	private void OnMpConnectedToServer() {
		SceneManagerNode.Instance.ChangeSceneToChat();
	}
}
