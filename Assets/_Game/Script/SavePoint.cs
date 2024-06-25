using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour
{
    [SerializeField] private Transform savePoint;
    [SerializeField] private int id;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController.savePoint = this.savePoint.position;
            SaveDataPoint();
        }
    }

    private void SaveDataPoint()
    {
        PlayerPrefs.SetInt(Constant.SAVEPOINT, id);
        PlayerPrefs.SetFloat(Constant.SAVEPOINT_X, savePoint.position.x);
        PlayerPrefs.SetFloat(Constant.SAVEPOINT_Y, savePoint.position.y);
        PlayerPrefs.SetFloat(Constant.SAVEPOINT_Z, savePoint.position.z);
    }
}
