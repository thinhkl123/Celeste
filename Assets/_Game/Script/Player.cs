using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Vector3 initalPos;

    private void Awake()
    {
        initalPos = transform.position;
    }

    private void Start()
    {
        LevelManager.Ins.OnLoadLevel += LevelManager_OnLoadLevel;
    }

    private void LevelManager_OnLoadLevel(object sender, System.EventArgs e)
    {
        OnInit();
    }

    private void OnInit()
    {
        transform.position = initalPos;
    }
}
