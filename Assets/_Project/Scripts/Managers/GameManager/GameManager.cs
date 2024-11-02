using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private int lives = 3;
    public int Lives { get { return lives; } private set { lives = value; EventManager.Broadcast(new OnLiveChanged(lives)); } }

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }
    
    private void OnEnable()
    {
        EventManager.AddListener<OnLiveLost>(OnLiveLost);
        EventManager.AddListener<OnRestartGame>(OnRestartGame);
        EventManager.AddListener<OnRushHour>(OnRushHour);
    }

    private void OnDisable()
    {
        EventManager.RemoveListener<OnLiveLost>(OnLiveLost);
        EventManager.RemoveListener<OnRestartGame>(OnRestartGame);
        EventManager.RemoveListener<OnRushHour>(OnRushHour);
    }

    private void OnRushHour(OnRushHour evt)
    {
        Time.timeScale = evt.IsRushHour ? 3 : 1f;
    }

    private void OnLiveLost(OnLiveLost evt)
    {
        Lives -= evt.Lives;

        if (Lives <= 0)
        {
            EventManager.Broadcast(new OnGameOver());
        }
    }

    private void OnRestartGame(OnRestartGame evt)
    {
        Lives = 3;
        Time.timeScale = 1f;
    }
}
