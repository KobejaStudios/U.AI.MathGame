using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Test : MonoBehaviour
{
    private void Start()
    {
        ServiceLocator.Get<INumberGeneratorService>().GetNumbers(257, 40, 12, EquationOperation.Addition);
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
        ServiceLocator.Get<INumberGeneratorService>().GetNumbers(sol, len, correct, EquationOperation.Addition);
    }
}
