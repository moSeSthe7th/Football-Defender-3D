using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointBar : MonoBehaviour
{

    RectTransform rect;
    public Vector3 posInRealWorld;


    // Start is called before the first frame update
    void Start()
    {
        rect = GetComponent<RectTransform>();
        
    }

    // Update is called once per frame
    void Update()
    {
        posInRealWorld = Camera.main.ScreenToWorldPoint(new Vector3((rect.anchorMax.x - rect.anchorMin.x) / 2f, (rect.anchorMax.y - rect.anchorMin.y) / 2f, Camera.main.nearClipPlane));
    }
}
