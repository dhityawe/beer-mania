using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : Singleton<ScoreManager>
{
    private int score = 0;
    private int highScore = 0;
    public static int HighScore {get => Instance.highScore;}
    public static int Score {get => Instance.score; private set { Instance.score = value; EventManager.Broadcast(new OnScoreChanged(Instance.score)); }}

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);

        if (PlayerPrefs.HasKey("HighScore"))
        {
            highScore = PlayerPrefs.GetInt("HighScore");
        }
    }

    private void OnEnable()
    {
        EventManager.AddListener<AddScore>(OnAddScore);
    }

    private void OnDisable()
    {
        EventManager.RemoveListener<AddScore>(OnAddScore);
    }

    private void OnAddScore(AddScore evt)
    {
        Score += evt.Score;

        if (Score > HighScore)
        {
            highScore = Score;
        }
    }

    public static void SaveHighScore()
    {
        PlayerPrefs.SetInt("HighScore", HighScore);
        PlayerPrefs.Save();
    }

    public static void ResetScore()
    {
        Score = 0;
    }
}