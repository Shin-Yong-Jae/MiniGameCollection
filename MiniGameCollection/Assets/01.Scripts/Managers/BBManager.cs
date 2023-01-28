using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BBManager : MonoBehaviour
{
    private int nowStage = 1;

    #region Unity Method
    private void Awake()
    {
        InitStage();
    }
    #endregion

    private void InitStage()
    {
        if (PlayerPrefs.HasKey("BB_Stage") == false)
        {
            PlayerPrefs.SetInt("BB_Stage", 1);
        }
        else
        {
            nowStage = PlayerPrefs.GetInt("BB_Stage");
        }
    }
}
