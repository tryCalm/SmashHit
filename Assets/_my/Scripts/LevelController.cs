using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    [SerializeField] private GameObject _startPanel;
    private void Awake()
    {
        _startPanel.SetActive(true);
        Time.timeScale = 0;
    }

    public void StartTime()
    {
        Time.timeScale = 1;
    }
}
