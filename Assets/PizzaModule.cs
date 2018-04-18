using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rnd = UnityEngine.Random;

public class PizzaModule : MonoBehaviour
{
    public KMSelectable Module;
    public GameObject Belt;
    public GameObject Plate;
    public KMSelectable[] Ingredients;
    public enum Ingredient {
        Tomatoes,
        Mushrooms,
        Olives,
        Unions
    }

    private List<Item> _itemsOnBelt = new List<Item>();

    void Start()
    {
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

            // If there's nothing on the belt, add something
            if (_itemsOnBelt.Count == 0)
            {
                var i = Rnd.Range(0, Ingredients.Length);
                var item = new Item() { Ingredient = (Ingredient)i, Selectable = Instantiate(Ingredients[i], Belt.transform) };
                var selectable = item.Selectable.GetComponent<KMSelectable>();
                selectable.OnInteract += delegate () { GrabItem(item); return false; };
                selectable.Parent = Module;
                _itemsOnBelt.Add(item);
                UpdateChildren();
                Debug.Log("Adding " + item.Ingredient.ToString() + ", there are " + _itemsOnBelt.Count + " items on the belt now.");
            }

            // Move the belt (actually everything on it)
            foreach (var item in _itemsOnBelt)
            {
                item.Selectable.transform.localPosition = new Vector3(
                    item.Selectable.transform.localPosition.x + Time.deltaTime * 5f,
                    item.Selectable.transform.localPosition.y,
                    item.Selectable.transform.localPosition.z
                );
            }

            // Remove stuff at the end of the belt
            for (var i = 0; i < _itemsOnBelt.Count; i++)
            {
                if (_itemsOnBelt[i].Selectable.transform.localPosition.x > 6)
                {
                    Debug.Log("Removing " + _itemsOnBelt[i].Ingredient.ToString() + " because it reached the end of the belt, there are " + (_itemsOnBelt.Count - 1) + " items on the belt now.");
                    Destroy(_itemsOnBelt[i].Selectable.gameObject);
                    _itemsOnBelt.RemoveAt(i);
                    UpdateChildren();
                }
            }
        }
    }

    private void UpdateChildren()
    {
        Module.Children = new KMSelectable[_itemsOnBelt.Count];
        for (var i = 0; i < _itemsOnBelt.Count; i++)
        {
            Module.Children[i] = _itemsOnBelt[i].Selectable.GetComponent<KMSelectable>();
        }
        Module.UpdateChildren();
    }

    private void GrabItem(Item item)
    {
        item.Selectable.transform.parent = Plate.transform;
        item.Selectable.transform.localPosition = new Vector3(-6 + Plate.transform.childCount, 0, 0);
        for (var i = 0; i < _itemsOnBelt.Count; i++)
        {
            if (_itemsOnBelt[i] == item) _itemsOnBelt.RemoveAt(i);
        }
    }

    class Item
    {
        public Ingredient Ingredient { get; set; }
        public KMSelectable Selectable { get; set; }
    }
}