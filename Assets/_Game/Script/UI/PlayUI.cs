using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayUI : UICanvas
{
    [Header("Health")]
    [SerializeField] private List<Image> heartImgList;
    [SerializeField] private Sprite liveImg;
    [SerializeField] private Sprite deadImg;
    [Header("Diamond")]
    [SerializeField] private TextMeshProUGUI diamondText;
    [Header("Time")]
    [SerializeField] private TextMeshProUGUI timeText;

    public void ResetHealth()
    {
        for (int i=0; i<heartImgList.Count; i++)
        {
            heartImgList[i].sprite = liveImg;
        }
    }

    public void DecreaseHeart(int value)
    {
        heartImgList[value].sprite = deadImg;
    }

    public void UpdateDiamond(int value, int valueMax)
    {
        diamondText.text = $"{value}/{valueMax}";
    }

    public void UpdateTimeText(int minutes, int second)
    {
        timeText.text = string.Format("{0:00}:{1:00}", minutes, second);
    }
}
