using Godot;

public interface ISceneManager {
	public void ChangeSceneToChat();
}

public partial class SceneManagerNode : Node, ISceneManager {
	[Export] private PackedScene ChatScene;

	public static ISceneManager Instance;

	public override void _Ready() {
		Instance = this;
	}

	public void ChangeSceneToChat() {
		GetTree().ChangeSceneToPacked(ChatScene);
	}
}
