using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using System.Collections;

public class CompilaTutorialPanel : MonoBehaviour
{
    public TMP_Text titolo;
    public TMP_Text testo;
    public Button chiudiButton;
    public Azienda azienda;
    public float fadeDuration = 0.3f; // Durata della dissolvenza in secondi

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

    public void MostraTutorial(string tit, bool mostraComunque = false)
    {
        if (azienda.flags.ContainsKey(tit) && (azienda.flags[tit] == false || mostraComunque == true))
        {
            // Imposta testo
            titolo.text = "Tutorial: " + 
                LocalizationSettings.StringDatabase.GetLocalizedString("Tutorial", tit + "TITOLO");
            testo.text = LocalizationSettings.StringDatabase.GetLocalizedString("Tutorial", tit + "TESTO");
            azienda.flags[tit] = true;

            // Avvia fade-in
            StartCoroutine(FadeIn());

            chiudiButton.onClick.RemoveAllListeners();

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




/*
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class CompilaTutorialPanel : MonoBehaviour
{

    public TMP_Text titolo;
    public TMP_Text testo;
    public Button chiudiButton;
    public Azienda azienda;
    

    public void MostraTutorial(string tit)
    {
        if (azienda.flags[tit] == false)
        {
            titolo.text = LocalizationSettings.StringDatabase.GetLocalizedString("Tutorial", tit + "TITOLO");
            testo.text = LocalizationSettings.StringDatabase.GetLocalizedString("Tutorial", tit + "TESTO");
            azienda.flags[tit] = true;
            gameObject.SetActive(true);
            if (tit == "introduzione1")
            {
                chiudiButton.onClick.RemoveAllListeners();
                chiudiButton.onClick.AddListener(() => MostraTutorial("introduzione2"));
            }
            else if(tit == "introduzione2")
            {
                chiudiButton.onClick.RemoveAllListeners();
                chiudiButton.onClick.AddListener(() => MostraTutorial("introduzione3"));
            }
            else if(tit == "introduzione3")
            {
                chiudiButton.onClick.RemoveAllListeners();
                chiudiButton.onClick.AddListener(() => MostraTutorial("introduzione4"));
            }
            else if(tit == "introduzione4")
            {
                chiudiButton.onClick.RemoveAllListeners();
                chiudiButton.onClick.AddListener(() => MostraTutorial("introduzione5"));
            }
            else if(tit == "introduzione5")
            {
                chiudiButton.onClick.RemoveAllListeners();
                chiudiButton.onClick.AddListener(() => MostraTutorial("introduzione6"));
            }
            else if(tit == "dipendenti1")
            {
                chiudiButton.onClick.RemoveAllListeners();
                chiudiButton.onClick.AddListener(() => MostraTutorial("dipendenti2"));
            }
            else if(tit == "progetti1")
            {
                chiudiButton.onClick.RemoveAllListeners();
                chiudiButton.onClick.AddListener(() => MostraTutorial("progetti2"));
            }
            else if(tit == "progetti2")
            {
                chiudiButton.onClick.RemoveAllListeners();
                chiudiButton.onClick.AddListener(() => MostraTutorial("progetti3"));
            }
            else if(tit == "dipartimenti1")
            {
                chiudiButton.onClick.RemoveAllListeners();
                chiudiButton.onClick.AddListener(() => MostraTutorial("dipartimenti2"));
            }
            else if(tit == "dipartimenti2")
            {
                chiudiButton.onClick.RemoveAllListeners();
                chiudiButton.onClick.AddListener(() => MostraTutorial("dipartimenti3"));
            }
            else
            {
                chiudiButton.onClick.RemoveAllListeners();
                chiudiButton.onClick.AddListener(() => gameObject.SetActive(false));
            }
        }
    }
}
*/