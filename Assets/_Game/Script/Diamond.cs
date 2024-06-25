using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Diamond : MonoBehaviour
{
    [SerializeField] private int id;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManager.Ins.AddDiamond();
            SetDiamondId();
            gameObject.SetActive(false);
        }
    }

    private void SetDiamondId()
    {
        PlayerPrefs.SetInt(Constant.DIAMONDID, id);
    }
}
