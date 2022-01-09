using System.Collections.Generic;

public class Pattern
{
    public float Difficulty;
    public readonly Queue<LaserInfo> ProjectileQueue = new();
}