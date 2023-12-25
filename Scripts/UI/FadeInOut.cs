using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeInOut : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Image image;

    private void OnEnable()
    {
        PlayerController.playerDeathEvent += StartFadeOutEffect;
        PlayerController.playerRespawnEvent += StartFadeInEffect;
    }

    private void OnDestroy()
    {
        PlayerController.playerDeathEvent -= StartFadeOutEffect;
        PlayerController.playerRespawnEvent -= StartFadeInEffect;
    }

    public void StartFadeOutEffect()
    {
        panel.SetActive(true);
        StartCoroutine(FadeOutCoroutine());
    }

    private IEnumerator FadeOutCoroutine()
    {
        float fadeAlpha = 0.0f;
        while (fadeAlpha < 1.0f)
        {
            fadeAlpha += 0.01f;
            yield return new WaitForSeconds(0.01f);
            image.color = new Color(0, 0, 0, fadeAlpha);
        }
    }

    public void StartFadeInEffect()
    {
        panel.SetActive(true);
        StartCoroutine(FadeInCoroutine());
    }

    private IEnumerator FadeInCoroutine()
    {
        float fadeAlpha = 1.0f;
        while (fadeAlpha > 0.0f)
        {
            fadeAlpha -= 0.01f;
            yield return new WaitForSeconds(0.01f);
            image.color = new Color(0, 0, 0, fadeAlpha);
        }

        panel.SetActive(false);
    }
}
