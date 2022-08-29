using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorializer : MonoBehaviour
{
    [Header("Controller sprites")]
    [SerializeField] private Sprite bodyMovementSprite;
    [SerializeField] private Sprite headMovementSprite;
    [SerializeField] private Sprite throwSprite;
    
    [Header("Keyboard sprites")]
    [SerializeField] private Sprite kbodyMovementSprite;
    [SerializeField] private Sprite kheadMovementSprite;
    [SerializeField] private Sprite kthrowSprite;
    
    private SpriteRenderer spriteRenderer;
    [SerializeField] private bool isController = true;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        isController = PlayerPrefs.GetInt("Controller") == 1;
        ShowHeadSprite();
    }

    public void ShowBodySprite()
    {
        if (isController)
        {
            spriteRenderer.sprite = bodyMovementSprite;
        }
        else
        {
            spriteRenderer.sprite = kbodyMovementSprite;
        }
    }
    public void ShowThrowSprite()
    {
        if (isController)
        {
            spriteRenderer.sprite = throwSprite;
        }
        else
        {
            spriteRenderer.sprite = kthrowSprite;
        }
    }
    public void ShowHeadSprite()
    {
        if (isController)
        {
            spriteRenderer.sprite = headMovementSprite;
        }
        else
        {
            spriteRenderer.sprite = kheadMovementSprite;
        }
    }
}
