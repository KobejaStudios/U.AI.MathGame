using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Test : MonoBehaviour
{
    private void Start()
    {
        ServiceLocator.Get<INumberSetGenerator>().GetNumberSet(257, 40, 12);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            GetSet();
        }
    }

    [ContextMenu("get set")]
    private void GetSet()
    {
        var sol = Random.Range(100, 400);
        var len = Random.Range(2, 50);
        var remainder = len % 2;
        len -= remainder;
        var correct = len / 2;
        ServiceLocator.Get<INumberSetGenerator>().GetNumberSet(sol, len, correct);
    }
}
