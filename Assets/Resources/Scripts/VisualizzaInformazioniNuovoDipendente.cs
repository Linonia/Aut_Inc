using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Scripts.Logica;
using TMPro;
using UnityEngine.Localization.Settings;

public class VisualizzaInformazioniNuovoDipendente : MonoBehaviour
{
    Dipendente dipendente;
    
    public TMP_Text nome;
    public Image foto;
    public TMP_Text descrizione;

    public Button assumiButton;

    public GameObject clearPanel;

    public Azienda azienda;

    public void OnEnable()
    {
        Clear();
    }

    public void Compila(Dipendente dipendente, GameObject infoPanel)
    {
        clearPanel.SetActive(false);
        this.dipendente = dipendente;
        foto.GetComponent<Image>().sprite = UnityEngine.Resources.Load<Sprite>("Images/Foto/" + dipendente.foto);
        nome.text = dipendente.nome;

        var des = "";
        for(int i = 0; i < dipendente.codiciDescrizioni.Length; i++)
        {
            string codice = dipendente.codiciDescrizioni[i];
            des += "- " + LocalizationSettings.StringDatabase.GetLocalizedString("Descrizioni", codice);
            if(i != dipendente.codiciDescrizioni.Length - 1)
                des += "\n";
        }
        // Setta la descrizione
        descrizione.text = des;

        
        assumiButton.gameObject.SetActive(true);
        assumiButton.onClick.RemoveAllListeners();
        assumiButton.onClick.AddListener(() =>
        {
            Destroy(infoPanel);
            azienda.OnAssumiDipendente(this.dipendente, Clear);
        });

    }
    
    public void Clear()
    {
        //dipendente = null;
        //nome.text = LocalizationSettings.StringDatabase.GetLocalizedString("TextTranslation", "nome");
        //foto.GetComponent<Image>().sprite = null;
        //// foto.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Foto/placeholder");
        //
        //// Clear descriptions
        //for(int i = 0; i < 5; i++)
        //{
        //    gameObject.transform.Find("Des" + (i + 1)).GetComponent<TMP_Text>().text = "--";
        //}
        //assumiButton.gameObject.SetActive(false);
        clearPanel.SetActive(true);
    }
    
}
