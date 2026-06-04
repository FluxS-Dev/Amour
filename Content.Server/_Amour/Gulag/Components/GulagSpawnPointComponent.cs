namespace Content.Server._Amour.Gulag.Components;

/// <summary>
/// Marks a map editor point that gulag workers may spawn at.
/// </summary>
[RegisterComponent, Access(typeof(GulagSystem))]
public sealed partial class GulagSpawnPointComponent : Component;
