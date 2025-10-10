using System;

public enum TileLayer { Ground = 0, Empty = 1, Wall = 2 }
public enum EffectType { Slow, Fire, Poison }
public enum ObjectType { Empty, Tree, Wall }

[Serializable]
public struct Int2 { public int x, y; public Int2(int x, int y) { this.x = x; this.y = y; } }