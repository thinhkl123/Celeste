using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    public event EventHandler OnLoadLevel;
    public event EventHandler OnLoadNewLevel;
    public event EventHandler OnLoadContinueLevel;

    [SerializeField] private List<Diamond> diamondList;

    public void LoadLevel()
    { 
        OnLoadLevel?.Invoke(this, EventArgs.Empty);
    }

    public void LoadNewLevel()
    {
        PlayerPrefs.SetInt(Constant.SAVEPOINT, -1);
        PlayerPrefs.SetFloat(Constant.SAVEPOINT_X, -10);
        PlayerPrefs.SetFloat(Constant.SAVEPOINT_Y, -10);
        PlayerPrefs.SetFloat(Constant.SAVEPOINT_Z, -10);
        PlayerPrefs.SetInt(Constant.DIAMONDID, -1);

        foreach (Diamond diamond in diamondList)
        {
            diamond.gameObject.SetActive(true);
        }

        OnLoadNewLevel?.Invoke(this, EventArgs.Empty);
    }

    public void LoadContinueLevel()
    {
        int idx = PlayerPrefs.GetInt(Constant.DIAMONDID, -1);
        int temp = 0;

        foreach (Diamond diamond in diamondList)
        {
            temp++;

            if (temp <= idx)
            {
                diamond.gameObject.SetActive(false);
            }
            else
            {
                diamond.gameObject.SetActive(true);
            }
        }

        OnLoadContinueLevel?.Invoke(this, EventArgs.Empty);
    }
}
