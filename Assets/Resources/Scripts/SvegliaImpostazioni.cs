using UnityEngine;

public class SvegliaImpostazioni : MonoBehaviour
{
    
    public OptionCompiler optionCompiler;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        optionCompiler.CaricaOpzioni();
    }
}
