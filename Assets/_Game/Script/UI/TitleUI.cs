using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleUI : UICanvas
{
    [SerializeField] private Button quitBtn;
    [SerializeField] private Button playCtn;
    [SerializeField] private Button playNew;

    [SerializeField] private Image playCtnImg;
    [SerializeField] private Sprite canSpr;
    [SerializeField] private Sprite notCanSpr;

    private void Start()
    {
        quitBtn.onClick.AddListener(() =>
        {
            Application.Quit();
        });
    }

    public void UpdateVisual()
    {
        playNew.onClick.RemoveAllListeners();
        playNew.onClick.AddListener(() =>
        {
            Close(0);
            GameManager.Ins.ChangeToDialogue();
        });

        playCtn.onClick.RemoveAllListeners();

        int id = PlayerPrefs.GetInt(Constant.SAVEPOINT, -1);

        if (id != -1)
        {
            playCtnImg.sprite = canSpr;

            playCtn.onClick.AddListener(() =>
            {
                Close(0);
                GameManager.Ins.ChangeToContinuePlaying();
            });
        }
        else
        {
            playCtnImg.sprite = notCanSpr;
        }
    }
}
