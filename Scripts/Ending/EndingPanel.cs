using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndingPanel : MonoBehaviour
{

    public CanvasGroup canvasGroup; 

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void StartEndingOutEffect()
    {
        gameObject.SetActive(true);
        StartCoroutine(EndingOutCoroutine());
    } 

    private IEnumerator EndingOutCoroutine()
    {
        float fadeAlpha = 0.0f;
        while (fadeAlpha < 1.0f)
        {
            fadeAlpha += 0.005f;
            yield return new WaitForSeconds(0.01f);
            canvasGroup.alpha = fadeAlpha; 
        }
    }

    public void StartEndingInEffect()
    {
        gameObject.SetActive(true);
        StartCoroutine(EndingInCoroutine());
    }

    private IEnumerator EndingInCoroutine()
    {

        float fadeAlpha = 1.0f;
        while (fadeAlpha > 0.0f)
        {
            fadeAlpha -= 0.005f;
            yield return new WaitForSeconds(0.01f);
            canvasGroup.alpha = fadeAlpha;
        }
    }
    
}

