using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using System.Collections;
using System;

public class CompilaTutorialPanel : MonoBehaviour
{
    public TMP_Text titolo;
    public TMP_Text testo;
    public Button chiudiButton;
    public Azienda azienda;
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
        if (azienda.tutorialFlags.ContainsKey(tit) && (azienda.tutorialFlags[tit] == false || mostraComunque))
        {
            // Imposta testo
            titolo.text = LocalizationSettings.StringDatabase.GetLocalizedString("Tutorial", tit + "TITOLO");
            testo.text = LocalizationSettings.StringDatabase.GetLocalizedString("Tutorial", tit + "TESTO");
            azienda.tutorialFlags[tit] = true;

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
                    "dipartimenti1" => "dipartimenti2",
                    "dipartimenti2" => "dipartimenti3",
                    "dipartimenti3" => "dipartimenti4",
                    "dipendenti1" => "dipendenti2",
                    "dipendenti2" => "dipendenti3",
                    "nuoviDipendenti1" => null,   // fine sequenza dipendenti
                    "progetti1" => "progetti2",
                    "progetti2" => "progetti3",
                    "progetti3" => "progetti4",
                    "progetti4" => null,           // fine sequenza progetti
                    "nuoviProgetti1" => null,      // solo uno nuovoProgetti
                    "aboutus1" => "aboutus2",
                    "aboutus2" => "aboutus3",
                    "aboutus3" => null,            // ultimo aboutus
                    _ => null
                };


                if (nextTutorial != null)
                {
                    chiudiButton.onClick.AddListener(() => MostraTutorial(nextTutorial, null, mostraComunque, continua));
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
