using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_SpriteAnimation : MonoBehaviour
{
    public Image mImage;
    public Sprite[] sprites;
    public float frameRate = 30.0f;

    public float currentTimeLeft = 0.0f;
    public int currentFrame = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if(currentTimeLeft <= 0.0f)
        {
            currentTimeLeft = (Application.targetFrameRate / frameRate) * Time.deltaTime;
            AdvanceFrame();
        }

        currentTimeLeft -= Time.deltaTime;
    }

    void AdvanceFrame()
    {
        currentFrame++;

        if (currentFrame > sprites.Length-1)
            currentFrame = 0;

        mImage.sprite = sprites[currentFrame];
    }
}
