using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseUI : UICanvas
{
    [SerializeField] private Button continueBtn;
    [SerializeField] private Button restartBtn;
    [SerializeField] private Button homeBtn;

    private void Start()
    {
        continueBtn.onClick.AddListener(() =>
        {
            Close(0);
            GameManager.Ins.state = GameManager.GameState.Playing;
        });

        restartBtn.onClick.AddListener(() =>
        {
            Close(0);
            //LevelManager.Ins.LoadLevel();
            GameManager.Ins.ChangeToNewPlaying();
        });

        homeBtn.onClick.AddListener(() =>
        {
            Close(0);
            UIManager.Ins.OpenUI<TitleUI>();
            UIManager.Ins.GetUI<TitleUI>().UpdateVisual();
            GameManager.Ins.state = GameManager.GameState.WaitToStart;
        });
    }
}
