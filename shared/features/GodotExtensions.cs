using Godot;

public static class GodotExtensions {
	/// <summary>
	/// Calls Free() on all node children
	/// </summary>
	public static void FreeChildren(this Node node) {
		foreach (var child in node.GetChildren()) {
			child.Free();
		}
	}
}
