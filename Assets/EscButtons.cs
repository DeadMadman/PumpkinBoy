using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class EscButtons : MonoBehaviour
{
    private Image img;
    [SerializeField] private Sprite keyboard;
    [SerializeField] private Sprite gamepad;

    void Start()
    {
        img = GetComponent<Image>();
        bool isController = PlayerPrefs.GetInt("Controller") == 1;
        img.sprite = isController ? gamepad : keyboard;
    }
    
}
