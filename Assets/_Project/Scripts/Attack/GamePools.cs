using CherryFramework.SimplePool;

public static class GamePools 
{
    public static readonly SimplePool<AttackObject> Hitboxes = new();
}