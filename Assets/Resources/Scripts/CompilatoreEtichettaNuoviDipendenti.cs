using Scripts.Logica;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.SmartFormat.Utilities;
using UnityEngine.UI;

public class CompilatoreEtichettaNuoviDipendenti : MonoBehaviour
{
    public Image foto;
    public TMP_Text nome;
    public Button selezionaButton;
    
    public void Compila(Dipendente dipendente)
    {
        foto.GetComponent<Image>().sprite = UnityEngine.Resources.Load<Sprite>("Images/Foto/" + dipendente.foto);
        nome.text = dipendente.nome;
        GameObject infoDipendente = transform.parent.parent.parent.parent.Find("InfoDipendente").gameObject;
        selezionaButton.onClick.AddListener(() => infoDipendente.GetComponent<VisualizzaInformazioniNuovoDipendente>().Compila(dipendente, gameObject));
    }
}
