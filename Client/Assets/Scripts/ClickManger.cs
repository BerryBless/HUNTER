using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickManger : MonoBehaviour
{
    public Grid _grid;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPos = _grid.WorldToCell(mousePos);

            Vector3 woldPos= _grid.CellToWorld(cellPos)+ new Vector3(0.5f, 0.5f, 0);
            transform.position = woldPos;
                Debug.Log($"cellPos({cellPos}) , worldPos({woldPos})");
        }
    }
}
