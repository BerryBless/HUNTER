using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabLoadTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Managers.Resource.Instantiate("Map/Map_001");
        Managers.Resource.Instantiate("Creatures/Player/Player");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
