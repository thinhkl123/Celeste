using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public enum GameState
    {
        WaitToStart,
        Dialogue,
        Playing,
        Pause,
        GameOver,
    }

    public GameState state;

    private int heartCount = 5;

    private int diamondCountMax = 10;
    private int diamondCount = 0;

    private float playStartTime;
    private float playCurTime;

    private int isShowDialogue;

    private void Awake()
    {
        state = GameState.WaitToStart;
    }

    private void Start()
    {
        UIManager.Ins.OpenUI<PlayUI>();
        UIManager.Ins.GetUI<PlayUI>().ResetHealth();
        UIManager.Ins.GetUI<PlayUI>().UpdateDiamond(0, diamondCountMax);

        UIManager.Ins.OpenUI<TitleUI>();
        UIManager.Ins.GetUI<TitleUI>().UpdateVisual();

        LevelManager.Ins.OnLoadLevel += LevelManager_OnLoadLevel;
    }

    private void LevelManager_OnLoadLevel(object sender, System.EventArgs e)
    {
        heartCount = 5;
        diamondCount = 0;
        UIManager.Ins.GetUI<PlayUI>().ResetHealth();
        UIManager.Ins.GetUI<PlayUI>().UpdateDiamond(0, diamondCountMax);
    }

    private void Update()
    {
        switch (state)
        {
            case GameState.WaitToStart:
                break;
            case GameState.Dialogue:
                break;
            case GameState.Playing:
                playCurTime += Time.deltaTime;
                int minute = (int)(playCurTime / 60);
                int second = (int)(playCurTime % 60);
                UIManager.Ins.GetUI<PlayUI>().UpdateTimeText(minute, second);

                PlayerPrefs.SetFloat(Constant.TIME, playCurTime);

                if (Input.GetKeyUp(KeyCode.Escape))
                {
                    state = GameState.Pause;
                    UIManager.Ins.OpenUI<PauseUI>();
                }

                break;
            case GameState.Pause:
                break;
            case GameState.GameOver:
                break;
        }
    }

    public void ChangeToDialogue()
    {
        UIManager.Ins.OpenUI<DialogueUI>();
        UIManager.Ins.GetUI<DialogueUI>().StartDialogue();
        state = GameState.Dialogue;
    }

    public void ChangeToNewPlaying()
    {
        SetStartNewTime();
        state = GameState.Playing;
        LevelManager.Ins.LoadLevel();
        LevelManager.Ins.LoadNewLevel();
    }

    public void ChangeToContinuePlaying()
    {
        SetStartContinueTime();
        state = GameState.Playing;
        LevelManager.Ins.LoadLevel();
        LevelManager.Ins.LoadContinueLevel();
    }

    public void AddDiamond()
    {
        diamondCount++;
        UIManager.Ins.GetUI<PlayUI>().UpdateDiamond(diamondCount, diamondCountMax);
    }

    public void DecreaseHeart()
    {
        heartCount--;
        if (heartCount >= 0)
        {
            UIManager.Ins.GetUI<PlayUI>().DecreaseHeart(heartCount);
        }

        if (heartCount == 0)
        {
            Debug.Log("Dead");
            UIManager.Ins.OpenUI<LostUI>();
            state = GameState.GameOver;
        }
    }

    public bool IsAlive()
    {
        return heartCount > 0;
    }

    public bool IsFinishCollect()
    {
        return diamondCount == diamondCountMax;
    }

    public void SetStartNewTime()
    {
        playStartTime = Time.time;
        playCurTime = 0;
    }

    public void SetStartContinueTime()
    {
        playStartTime = PlayerPrefs.GetFloat(Constant.TIME);
        playCurTime = playStartTime;
    }

    public int GetTimeMinute()
    {
        int minute = (int)(playCurTime / 60);
        return minute;
    }

    public int GetTimeSecond()
    {
        int second = (int)(playCurTime % 60);
        return second;
    }
}
