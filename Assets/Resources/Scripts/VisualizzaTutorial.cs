using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class VisualizzaTutorial : MonoBehaviour
{
    public GameObject ContentPanel;
    
    public GameObject TutorialPrefab;
    
    public Azienda azienda;
    
    private void OnEnable()
    {
        // Pulisci il pannello
        foreach (Transform child in ContentPanel.transform)
        {
            Destroy(child.gameObject);
        }

        // Aggiungi i tutorial non ancora visti
        foreach (var tutorial in azienda.tutorialFlags)
        {
            if(tutorial.Key == "aboutus1" || tutorial.Key == "aboutus2")
                continue;
            GameObject tutorialItem = Instantiate(TutorialPrefab, ContentPanel.transform);
            tutorialItem.GetComponentInChildren<TMP_Text>().text = LocalizationSettings.StringDatabase.GetLocalizedString("Tutorial", tutorial.Key + "TITOLO");
            tutorialItem.GetComponent<Button>().onClick.AddListener(() =>
            {
                // Disabilita il pannello dei tutorial
                gameObject.SetActive(false);
                // Mostra il tutorial selezionato
                azienda.tutorialPanel.MostraTutorial(tutorial.Key, () =>
                {
                    gameObject.SetActive(true);
                }, true, false);
            });
        }
    }
}
