using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Scripts.Logica;

public class DropdownColorato : TMP_Dropdown
{
    public Dictionary<int, (Reparto, Team, string)> coloriTuple;

    protected override void Start()
    {
        base.Start();

        // Aggancio al pulsante che apre la lista
        var button = GetComponentInChildren<Button>();
        if (button != null)
            button.onClick.AddListener(() => StartCoroutine(ColoraLista()));
    }

    private IEnumerator ColoraLista()
    {
        // Aspetta un frame per far generare la lista
        yield return null;

        var colori = coloriTuple
            .OrderBy(kv => kv.Key)
            .Select(kv => kv.Value.Item3)
            .ToList();

        // La lista viene generata sotto il "template"
        var content = template.Find("Viewport/Content");
        if (content == null) yield break;

        for (int i = 0; i < content.childCount; i++)
        {
            var bgTransform = content.GetChild(i).Find("Item Background");
            if (bgTransform != null)
            {
                var bg = bgTransform.GetComponent<Image>();
                if (bg != null && i < colori.Count)
                {
                    if (ColorUtility.TryParseHtmlString(colori[i], out var parsedColor))
                        bg.color = parsedColor;
                    else
                        bg.color = new Color32(245, 245, 245, 255); // fallback
                }
            }
        }
    }
}