using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LevelGameplayUtility))]
public abstract class LevelGameplay : MonoBehaviour
{  
    [Range(2, 10)] [SerializeField] protected int timePerGuess = 10;
    [SerializeField] protected Sound wrongGuessSound = null;
    [SerializeField] protected Sound correctGuessSound = null;
    protected LevelGameplayUtility GameplayUtility { get; private set; }

    #region SETUP
    private void Awake()
    {
        GameplayUtility = GetComponent<LevelGameplayUtility>();
        AudioManager.AddSound(wrongGuessSound);
        AudioManager.AddSound(correctGuessSound);
    }
    protected virtual void OnEnable()
    {
        
    }
    protected virtual void OnDisable()
    {
        GameplayUtility.StartGameEvent -= StartGame;
        GameplayUtility.Timer.TimerExpiredEvent -= OnTimerExpired;
        GameplayUtility.Timer.CountdownCompletedEvent -= OnCountdownCompleted;
    }
    protected virtual void Start()
    {
        GameplayUtility.StartGameEvent += StartGame;
        GameplayUtility.Timer.TimerExpiredEvent += OnTimerExpired;
        GameplayUtility.Timer.CountdownCompletedEvent += OnCountdownCompleted;
        GameplayUtility.OnStartGamePressed();
    }
    protected virtual void SetupLevel()
    {
        StopAllCoroutines();       
        GameplayUtility.Timer.Initialize(timePerGuess);
        GameplayUtility.ScoreSystem.Initialize();
        GameMenu.Open();
    }
    #endregion

    protected abstract void StartGameLoop();
    private void StartGame()
    {        
        SetupLevel();
        
        StartGameLoop();
    }
    protected virtual void OnTimerExpired() { }
    protected virtual void OnCountdownCompleted() { }

    #region HELPER METHODS
    public abstract void PauseGame();
    public abstract void ResumeGame();
    public abstract bool IsLevelComplete();
    public abstract bool IsLevelPassed();
    #endregion
}