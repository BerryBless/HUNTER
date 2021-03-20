using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PathController : MonoBehaviour
{
    TextMeshPro _textMeshPro;
    // Start is called before the first frame update
    private void Awake()
    {
        _textMeshPro = gameObject.FindChild<TextMeshPro>();
    }

    
    public void ChangePathText(string text)
    {
        _textMeshPro.text = text;
    }
}
