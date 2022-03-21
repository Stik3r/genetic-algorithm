using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectMapBorder : MonoBehaviour
{
    public static Transform selected;
    private void OnMouseDown()
    {
        if(transform.name.Contains("MapPrefab"))
        {
            if(selected != null)
            {
                selected.GetChild(4).gameObject.SetActive(false);
            }
            selected = transform;
            transform.GetChild(4).gameObject.SetActive(true);
        }
    }
}
