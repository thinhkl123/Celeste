using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (GameManager.Ins.IsFinishCollect())
            {
                GameManager.Ins.state = GameManager.GameState.GameOver;
                UIManager.Ins.OpenUI<WinUI>();
            }
        }
    }
}
