using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public interface IGameSystem
{
    void OnGameStateChanged(GameManager.GameState newState);
    void OnSceneLoaded(string sceneName);
    // add more common lifecycle events when needed
    // void OnPlayerSpawned(GameObject player);
    // void OnWaveStarted(int waveNumber);
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // ────────────────────────────────────────────────
    //  Core state & events
    // ────────────────────────────────────────────────

    public enum GameState
    {
        Boot,
        MainMenu,
        Loading,
        Playing,
        Paused,
        Voting,
        Cinematic,
        GameOver
    }

    public GameState CurrentState { get; private set; } = GameState.Boot;

    public event Action<GameState> OnGameStateChanged;

    private readonly List<IGameSystem> registeredSystems = new List<IGameSystem>();

    // ────────────────────────────────────────────────
    //  Lifecycle
    // ────────────────────────────────────────────────

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        CurrentState = GameState.Boot;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // ────────────────────────────────────────────────
    //  Registration – call this from Awake() of other managers
    // ────────────────────────────────────────────────

    public void RegisterSystem(IGameSystem system)
    {
        if (!registeredSystems.Contains(system))
        {
            registeredSystems.Add(system);
            // immediately inform about current state
            system.OnGameStateChanged(CurrentState);
        }
    }

    public void UnregisterSystem(IGameSystem system)
    {
        registeredSystems.Remove(system);
    }

    // ────────────────────────────────────────────────
    //  State machine
    // ────────────────────────────────────────────────

    public void SetState(GameState newState)
    {
        if (newState == CurrentState) return;

        var oldState = CurrentState;
        CurrentState = newState;

        // Notify EVERY registered system
        foreach (var system in registeredSystems)
        {
            try
            {
                system.OnGameStateChanged(newState);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        OnGameStateChanged?.Invoke(newState);

        // Built-in behavior for common states
        switch (newState)
        {
            case GameState.Playing:
            case GameState.Loading:
                Time.timeScale = 1f;
                break;
            case GameState.Paused:
            case GameState.Voting:
            case GameState.Cinematic:
                Time.timeScale = 0f;
                break;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Re-notify everyone about current state after scene load
        foreach (var system in registeredSystems)
        {
            system.OnSceneLoaded(scene.name);
        }
    }

    // ────────────────────────────────────────────────
    //  Convenience methods – can be called from anywhere
    // ────────────────────────────────────────────────

    public void Pause() => SetState(GameState.Paused);
    public void Resume() => SetState(GameState.Playing);
    public void StartVoting() => SetState(GameState.Voting);
    public void ShowGameOver() => SetState(GameState.GameOver);
}
