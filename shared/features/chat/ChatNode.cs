using System.Collections.Generic;
using Godot;

public partial class ChatNode : Control {
	private VBoxContainer _membersContainer;
	private Button _sendButton;
	private LineEdit _messageLineEdit;
	private VBoxContainer _messageContainer;
	private Label _clientNameLabel;

	private readonly Dictionary<long, Label> _chatters = new();
	private int _count = 0;

	#region Node Lifecycle
	public override void _Ready() {
		_membersContainer = GetNode<VBoxContainer>("HBoxContainer/Left/Panel/MarginContainer/VBoxContainer/Members");
		_sendButton = GetNode<Button>("HBoxContainer/Right/Panel/MarginContainer/VBoxContainer/HBoxContainer/SendButton");
		_messageLineEdit = GetNode<LineEdit>("HBoxContainer/Right/Panel/MarginContainer/VBoxContainer/HBoxContainer/Message");
		_messageContainer = GetNode<VBoxContainer>("HBoxContainer/Right/Panel/MarginContainer/VBoxContainer/MessageContainer");
		_clientNameLabel = GetNode<Label>("HBoxContainer/Right/Panel/MarginContainer/VBoxContainer/HBoxContainer/ClientName");

		_membersContainer.FreeChildren();
		_clientNameLabel.Text = NetworkManagerNode.Instance.PlayerData.DisplayName;
		InitConnectedPlayers();

		// signals
		_sendButton.Pressed += OnSendButtonPressed;
		Multiplayer.ServerDisconnected += () => GetTree().Quit();
		// events
		RpcApiNode.Instance.MessageReceived += OnMessageReceived;
		NetworkManagerNode.Instance.PlayerConnected += OnPlayerConnected;
		NetworkManagerNode.Instance.PlayerDisconnected += OnPlayerDisconnected;
	}
	public override void _ExitTree() {
		RpcApiNode.Instance.MessageReceived -= OnMessageReceived;
		NetworkManagerNode.Instance.PlayerConnected -= OnPlayerConnected;
		NetworkManagerNode.Instance.PlayerDisconnected -= OnPlayerDisconnected;
	}
	#endregion Node Lifecycle

	private void InitConnectedPlayers() {
		OnPlayerConnected(NetworkManagerNode.Instance.PlayerData);
		var players = NetworkManagerNode.Instance.ConnectedPlayers.Values;
		foreach (var p in players) {
			OnPlayerConnected(p);
		}
	}

	private void OnPlayerDisconnected(PlayerData data) {
		var label = _chatters[data.Id];
		_chatters.Remove(data.Id);
		// _membersContainer.RemoveChild(label);
		label.Free();
	}
	private void OnPlayerConnected(PlayerData data) {
		var label = new Label {
			Text = data.DisplayName
		};

		if (data.Id == Multiplayer.GetUniqueId()) {
			label.Text += " (you)";
		}

		_chatters.Add(data.Id, label);
		_membersContainer.AddChild(label);
	}

	private void OnSendButtonPressed() {
		var trimMsg = _messageLineEdit.Text.Trim();
		if (string.IsNullOrWhiteSpace(trimMsg)) {
			return;
		}

		var err = RpcApiNode.Instance.SendRPC(nameof(RpcApiNode.SendMessageRpc), _messageLineEdit.Text);
		if (err == Error.Ok) {
			_messageLineEdit.Text = string.Empty;
		} else {
			GD.PrintErr(err);
		}
	}
	private void OnMessageReceived(string msg) {
		var label = new Label {
			Text = msg
		};
		_messageContainer.AddChild(label);
	}
}
