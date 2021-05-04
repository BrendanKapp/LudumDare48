using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranslationController : MonoBehaviour
{
    [SerializeField]
    private TextAsset textData;
    private Dictionary<string, string> data;

    private void Start()
    {
        LoadData();
    }
    private void LoadData()
    {
        
    }

    public void GetString(string stringId)
    {

    }
}
