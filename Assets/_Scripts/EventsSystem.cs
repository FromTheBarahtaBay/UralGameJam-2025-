using System;

public class EventsSystem
{
    public static event Action IsPlayerHited;
    public static void OnIsPlayerHited() => IsPlayerHited?.Invoke();

    public static event Action IsPlayerDead;
    public static void OnIsPlayerDead() => IsPlayerDead?.Invoke();

    public static event Action IsPlayerWin;
    public static void OnIsPlayerWin() => IsPlayerWin?.Invoke();

    public static event Action<int> IsToyFind;
    public static void OnIsToyFind(int value) => IsToyFind?.Invoke(value);

    public static event Action IsNewGame;
    public static void OnIsNewGame() => IsNewGame?.Invoke();
}