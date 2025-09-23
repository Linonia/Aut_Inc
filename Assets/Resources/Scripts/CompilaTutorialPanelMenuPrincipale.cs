using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using System.Collections;
using System;

public class CompilaTutorialPanelMenuPrincipale : MonoBehaviour
{
    public TMP_Text titolo;
    public TMP_Text testo;
    public Button chiudiButton;
    public VisualizzaTutorialMenuPrincipale visualizzaTutorialPanel;
    public float fadeDuration = 0.2f; // Durata della dissolvenza in secondi

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        // Prendi o aggiungi CanvasGroup
        canvasGroup = GetComponent<CanvasGroup>();
        if(canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        // Panel sempre attivo, ma invisibile e non interattivo
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public void MostraTutorial(string tit, Action chiudiAction = null, bool mostraComunque = false, bool continua = true)
    {
        if (visualizzaTutorialPanel.tutorialFlags.Contains(tit) && mostraComunque)
        {
            // Imposta testo
            titolo.text = LocalizationSettings.StringDatabase.GetLocalizedString("Tutorial", tit + "TITOLO");
            testo.text = LocalizationSettings.StringDatabase.GetLocalizedString("Tutorial", tit + "TESTO");

            // Avvia fade-in
            StartCoroutine(FadeIn());

            chiudiButton.onClick.RemoveAllListeners();

            if (continua)
            {
                // Sequenze dei tutorial
                string nextTutorial = tit switch
                {
                    "introduzione1" => "introduzione2",
                    "introduzione2" => "introduzione3",
                    "introduzione3" => "introduzione4",
                    "introduzione4" => "introduzione5",
                    "introduzione5" => "introduzione6",
                    "dipendenti1" => "dipendenti2",
                    "progetti1" => "progetti2",
                    "progetti2" => "progetti3",
                    "dipartimenti1" => "dipartimenti2",
                    "dipartimenti2" => "dipartimenti3",
                    _ => null
                };

                if (nextTutorial != null)
                {
                    chiudiButton.onClick.AddListener(() => MostraTutorial(nextTutorial));
                }
                else
                {
                    chiudiButton.onClick.AddListener(() => StartCoroutine(FadeOut()));
                }
            }
            else
            {
                chiudiButton.onClick.AddListener(() => StartCoroutine(FadeOut()));
            }
            
            if(chiudiAction != null)
                chiudiButton.onClick.AddListener(() => chiudiAction());
        }
    }

    private IEnumerator FadeIn()
    {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        float elapsed = 0;
        while (elapsed < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(0, 1, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    private IEnumerator FadeOut()
    {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        float elapsed = 0;
        while (elapsed < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(1, 0, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 0;
        // GameObject rimane attivo ma invisibile
    }
}
