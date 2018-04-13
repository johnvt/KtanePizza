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
    public KMSelectable TestIngredient;
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
                item.Selectable.OnInteract += delegate () { GrabItem(item); return false; };
                item.Selectable.Parent = Module;
                _itemsOnBelt.Add(item);
                var children = new KMSelectable[_itemsOnBelt.Count];
                for (i = 0; i < _itemsOnBelt.Count; i++)
                {
                    children[i] = _itemsOnBelt[i].Selectable;
                }
                Module.Children = children;
                Module.UpdateChildren();
                Debug.Log("Adding " + item.Ingredient.ToString() + ", there are " + _itemsOnBelt.Count + " items on the belt now.");
            }

            // Move the belt (actually everything on it)
            foreach (var ingredient in _itemsOnBelt)
            {
                ingredient.Selectable.transform.localPosition = new Vector3(
                    ingredient.Selectable.transform.localPosition.x + Time.deltaTime * 5f,
                    ingredient.Selectable.transform.localPosition.y,
                    ingredient.Selectable.transform.localPosition.z
                );
            }

            // Remove stuff at the end of the belt
            if (_itemsOnBelt[0].Selectable.transform.localPosition.x > 6)
            {
                Debug.Log("Removing " + _itemsOnBelt[0].Ingredient.ToString() + " because it reached the end of the belt, there are " + (_itemsOnBelt.Count - 1) + " items on the belt now.");
                Destroy(_itemsOnBelt[0].Selectable.gameObject);
                _itemsOnBelt.RemoveAt(0);
            }

        }
    }

    private void GrabItem(Item item)
    {
        item.Selectable.transform.parent = Plate.transform;
        item.Selectable.transform.position = new Vector3(0, 0, 0);
    }

    class Item
    {
        public Ingredient Ingredient { get; set; }
        public KMSelectable Selectable { get; set; }
    }
}