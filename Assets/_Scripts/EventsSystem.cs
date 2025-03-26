using System;

public class EventsSystem
{
    public static event Action IsPlayerHited;
    public static void OnIsPlayerHited() => IsPlayerHited?.Invoke();

    public static event Action IsPlayerDead;
    public static void OnIsPlayerDead() => IsPlayerDead?.Invoke();

    public static event Action IsNewGame;
    public static void OnIsNewGame() => IsNewGame?.Invoke();
}