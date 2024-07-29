# gdnet-chat
Simple Chat system leveraging Godot 4.2.2 High level Multiplayer API. Built using C# Dotnet only

## High level notes
We leverage both signals and c# events mainly to avoid needed to create a GodotObject. There seems to be a bug at the moment trying to convert objects back and forth when using signals therefore we use events when we want to pass more than primitives through what would have been a signal.