using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PizzaModule : MonoBehaviour
{
    public GameObject ConveyorBelt;
    public KMSelectable[] IngredientPrefabs;
    private List<KMSelectable> _ingredientsOnBelt = new List<KMSelectable>();

    void Start()
    {
        for (var i = 0; i < IngredientPrefabs.Length; i++)
        {
            var ingredient = Instantiate(IngredientPrefabs[i], ConveyorBelt.transform);
            ingredient.transform.localPosition = new Vector3(
                ingredient.transform.localPosition.x + i * 4,
                ingredient.transform.localPosition.y,
                ingredient.transform.localPosition.z
            );
            _ingredientsOnBelt.Add(ingredient);
        }
    }

    void Update()
    {
        //StartCoroutine(ConveyorBelt());
    }

    private IEnumerator RunConveyorBelt()
    {
        throw new NotImplementedException();
    }
}
