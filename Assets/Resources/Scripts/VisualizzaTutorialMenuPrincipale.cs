using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class VisualizzaTutorialMenuPrincipale : MonoBehaviour
{
    public GameObject ContentPanel;
    
    public GameObject TutorialPrefab;
    
    public CompilaTutorialPanelMenuPrincipale tutorialPanelObject;
    
    private void OnEnable()
    {
        // Pulisci il pannello
        foreach (Transform child in ContentPanel.transform)
        {
            Destroy(child.gameObject);
        }

        // Aggiungi i tutorial
        foreach (var tutorial in tutorialPanelObject.tutorialFlags)
        {
            if(tutorial == "aboutus1" || tutorial == "aboutus2" || tutorial == "aboutus3")
                continue;
            GameObject tutorialItem = Instantiate(TutorialPrefab, ContentPanel.transform);
            tutorialItem.GetComponentInChildren<TMP_Text>().text = LocalizationSettings.StringDatabase.GetLocalizedString("Tutorial", tutorial + "TITOLO");
            tutorialItem.GetComponent<Button>().onClick.AddListener(() =>
            {
                // Disabilita il pannello dei tutorial
                gameObject.SetActive(false);
                // Mostra il tutorial selezionato
                tutorialPanelObject.MostraTutorial(tutorial, () =>
                {
                    gameObject.SetActive(true);
                }, true, false);
            });
        }
    }
}