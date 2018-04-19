using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Rnd = UnityEngine.Random;

public class PizzaModule : MonoBehaviour
{
    public KMSelectable Module;
    public KMSelectable[] BeltNodes;
    public KMSelectable[] PlateNodes;
    public GameObject[] Ingredients;
    public enum Ingredient {
        Tomatoes,
        Mushrooms,
        Olives,
        Unions
    }

    private List<Item> _itemsOnBelt;
    private List<Item> _itemsOnPlate;

    void Start()
    {
        _itemsOnBelt = Enumerable.Repeat((Item)null, BeltNodes.Length).ToList();
        _itemsOnPlate = Enumerable.Repeat((Item)null, PlateNodes.Length).ToList();

        for (int i = 0; i < BeltNodes.Length; i++)
        {
            var j = i;
            BeltNodes[i].OnInteract += delegate () { GrabItem(j); return false; };
        }

        for (int i = 0; i < PlateNodes.Length; i++)
        {
            var j = i;
            PlateNodes[i].OnInteract += delegate () { DropItem(j); return false; };
        }

        StartCoroutine(MoveBelt());
    }

    void Update()
    {
    }

    private IEnumerator MoveBelt()
    {
        while (true)
        {
            yield return null;

            // If the first node is empty, add something
            if (_itemsOnBelt[0] == null)
            {
                var i = Rnd.Range(0, Ingredients.Length);
                var item = new Item() { Ingredient = (Ingredient)i, Instance = Instantiate(Ingredients[i], BeltNodes[0].transform) };
                _itemsOnBelt[0] = item;
                Debug.Log(item.Ingredient.ToString() + " incoming.");
            }

            // If the nodes reached the starting point of the next node, reset the nodes and pass on all items to the next node
            if (BeltNodes[0].transform.localPosition.x > 4)
            {
                for (var i = BeltNodes.Length - 1; i >= 0; i--)
                {
                    // Remove stuff at the end of the belt
                    if (i == BeltNodes.Length - 1 && _itemsOnBelt[i] is Item)
                    {
                        Debug.Log(_itemsOnBelt[i].Ingredient.ToString() + " leaving.");
                        Destroy(_itemsOnBelt[i].Instance.gameObject);
                    }

                    // Pass on
                    if (i == 0)
                    {
                        _itemsOnBelt[i] = null;
                    }
                    else
                    {
                        _itemsOnBelt[i] = _itemsOnBelt[i - 1];
                        if (_itemsOnBelt[i] is Item)
                        {
                            _itemsOnBelt[i].Instance.transform.parent = BeltNodes[i].transform;
                            _itemsOnBelt[i].Instance.transform.localPosition = new Vector3(0, 0, 0);
                        }
                    }

                    // Reset position
                    BeltNodes[i].transform.localPosition = new Vector3(0, 0, 0);
                }
            }

            foreach (var beltNode in BeltNodes)
            {
                beltNode.transform.localPosition = new Vector3(
                    beltNode.transform.localPosition.x + Time.deltaTime * 2f,
                    beltNode.transform.localPosition.y,
                    beltNode.transform.localPosition.z
                );
            }
        }
    }

    private void GrabItem(int beltIndex)
    {
        if (_itemsOnBelt[beltIndex] is Item)
        {
            var plateIndex = -1;
            for (var i = 0; i < PlateNodes.Length; i++)
                if (_itemsOnPlate[i] == null)
                {
                    plateIndex = i;
                    break;
                }

            // No room!
            if (plateIndex == -1) return;

            // Move item from belt to plate
            _itemsOnPlate[plateIndex] = _itemsOnBelt[beltIndex];
            _itemsOnBelt[beltIndex].Instance.transform.parent = PlateNodes[plateIndex].transform;
            _itemsOnBelt[beltIndex].Instance.transform.localPosition = new Vector3(0, 0, 0);
            _itemsOnBelt[beltIndex] = null;
        }
    }

    private void DropItem(int plateIndex)
    {
        if (_itemsOnPlate[plateIndex] is Item)
        {
            // Drop item
            Destroy(_itemsOnPlate[plateIndex].Instance.gameObject);
            _itemsOnPlate[plateIndex] = null;
        }
    }

    class Item
    {
        public Ingredient Ingredient { get; set; }
        public GameObject Instance { get; set; }
    }
}