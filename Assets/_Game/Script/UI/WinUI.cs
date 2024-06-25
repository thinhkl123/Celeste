using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinUI : UICanvas
{
    [SerializeField] private Button homeBtn;
    [SerializeField] private TextMeshProUGUI timeResultText;
    [SerializeField] private TextMeshProUGUI bestResultText;

    private void Start()
    {
        homeBtn.onClick.AddListener(() =>
        {
            Close(0);
            UIManager.Ins.OpenUI<TitleUI>();
            UIManager.Ins.GetUI<TitleUI>().UpdateVisual();
        });
    }

    public void UpdateResult()
    {
        PlayerPrefs.SetInt(Constant.SAVEPOINT, -1);
        PlayerPrefs.SetFloat(Constant.SAVEPOINT_X, -10);
        PlayerPrefs.SetFloat(Constant.SAVEPOINT_Y, -10);
        PlayerPrefs.SetFloat(Constant.SAVEPOINT_Z, -10);
        PlayerPrefs.SetInt(Constant.DIAMONDID, -1);

        int minute = GameManager.Ins.GetTimeMinute();
        int second = GameManager.Ins.GetTimeSecond();
        int time = minute*60 + second;
        timeResultText.text = string.Format("{0:00}:{1:00}", minute, second);
        int bestResult = PlayerPrefs.GetInt("BestTime", 0);
        if (bestResult == 0)
        {
            PlayerPrefs.SetInt("BestTime", time);
            bestResult = time;
        }
        else if (bestResult > time) 
        {
            PlayerPrefs.SetInt("BestTime", time);
            bestResult = time;
        }
        minute = (int) bestResult / 60;
        second = (int) bestResult % 60;
        bestResultText.text = string.Format("{0:00}:{1:00}", minute, second);
    }
}
