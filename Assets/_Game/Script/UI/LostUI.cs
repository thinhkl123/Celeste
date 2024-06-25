using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LostUI : UICanvas
{
    [SerializeField] private Button reStartBtn;
    [SerializeField] private Button homeBtn;

    private void Start()
    {
        reStartBtn.onClick.AddListener(() =>
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
