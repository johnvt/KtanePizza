﻿using System;
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
    public enum Ingredient
    {
        Tomatoes,
        Mushrooms,
        Olives,
        Onions,
        Basil,
        Mozzarella,
        BbqSauce,
        GrilledChickenBreast,
        RedOnions,
        Bacon,
        Cheddar,
        Jalapeño,
        Scampi,
        BellPeppers,
        Ham,
        Pepperoni,
        Mussels,
        Tuna,
        BlackOlives,
        Pineapple,
        ItalianSausage,
        GroundBeef
    }

    private Dictionary<string, Pizza> _pizzas;
    private List<Item> _itemsOnBelt;
    private List<Item> _itemsOnPlate;

    void Start()
    {
        _pizzas = new Dictionary<string, Pizza>()
        {
            { "Margherita", new Pizza() { Name = "Margherita", Ingredients = new List<Ingredient>() {
                Ingredient.Tomatoes, Ingredient.Basil, Ingredient.Mozzarella
            } } },
            { "BBQ Chicken", new Pizza() { Name = "BBQ Chicken", Ingredients = new List<Ingredient>() {
                Ingredient.BbqSauce, Ingredient.GrilledChickenBreast, Ingredient.RedOnions, Ingredient.Bacon, Ingredient.Mozzarella
            } } },
            { "Buffalo Chicken", new Pizza() { Name = "Buffalo Chicken", Ingredients = new List<Ingredient>() {
                Ingredient.BbqSauce, Ingredient.GrilledChickenBreast, Ingredient.RedOnions, Ingredient.Cheddar, Ingredient.Mozzarella
            } } },
            { "Strike", new Pizza() { Name = "Strike", Ingredients = new List<Ingredient>() {
                Ingredient.Pepperoni, Ingredient.Ham, Ingredient.BellPeppers, Ingredient.Scampi, Ingredient.Jalapeño, Ingredient.Mozzarella
            } } },
            { "Blow up", new Pizza() { Name = "Blow up", Ingredients = new List<Ingredient>() {
                Ingredient.Pepperoni, Ingredient.Ham, Ingredient.BellPeppers, Ingredient.Mussels, Ingredient.Jalapeño, Ingredient.Jalapeño, Ingredient.Mozzarella
            } } },
            { "Frutti di Mare", new Pizza() { Name = "Frutti di Mare", Ingredients = new List<Ingredient>() {
                Ingredient.Tuna, Ingredient.Scampi, Ingredient.Mussels, Ingredient.BlackOlives
            } } },
            { "Hawaii", new Pizza() { Name = "Hawaii", Ingredients = new List<Ingredient>() {
                Ingredient.Ham, Ingredient.Bacon, Ingredient.Pineapple, Ingredient.Mozzarella
            } } },
            { "Meat Lovers", new Pizza() { Name = "Meat Lovers", Ingredients = new List<Ingredient>() {
                Ingredient.Pepperoni, Ingredient.Ham, Ingredient.ItalianSausage, Ingredient.GroundBeef, Ingredient.Mozzarella
            } } },
            { "Veggie", new Pizza() { Name = "Veggie", Ingredients = new List<Ingredient>() {
                Ingredient.Mushrooms, Ingredient.BellPeppers, Ingredient.Onions, Ingredient.BlackOlives, Ingredient.Tomatoes, Ingredient.Mozzarella
            } } },
            { "Bacon Cheddar", new Pizza() { Name = "Bacon Cheddar", Ingredients = new List<Ingredient>() {
                Ingredient.GroundBeef, Ingredient.Bacon, Ingredient.Cheddar, Ingredient.Mozzarella
            } } },
            { "Tuna Delight", new Pizza() { Name = "Tuna Delight", Ingredients = new List<Ingredient>() {
                Tuna, Red Onions, Black Olives, Mozzarella Cheese
            } } },
            { "Quattro Stagioni", new Pizza() { Name = "Quattro Stagioni", Ingredients = new List<Ingredient>() {
                Artichokes, Tomatoes, Basil, Mushrooms, Ham, Mozzarella Cheese
            } } },
        };

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
            PlateNodes[i].OnInteract += delegate () { ReturnItem(j); return false; };
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
                var item = new Item()
                {
                    Ingredient = (Ingredient)i,
                    Instance = Instantiate(Ingredients[i], BeltNodes[0].transform),
                };
                _itemsOnBelt[0] = item;
                //Debug.Log(item.Ingredient.ToString() + " incoming.");
            }

            // If the nodes reached the starting point of the next node, reset the nodes and pass on all items to the next node
            if (BeltNodes[0].transform.localPosition.x > 4)
            {
                for (var i = BeltNodes.Length - 1; i >= 0; i--)
                {
                    // Remove stuff at the end of the belt
                    if (i == BeltNodes.Length - 1 && _itemsOnBelt[i] is Item)
                    {
                        //Debug.Log(_itemsOnBelt[i].Ingredient.ToString() + " leaving.");
                        Destroy(_itemsOnBelt[i].Instance.gameObject);
                    }

                    // Pass items on to the next node
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

                    // Reset node positions
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

    private IEnumerator MoveItem(Item item, Vector3 from, Vector3 to, float duration)
    {
        float elapsed = 0;
        while (elapsed < duration)
        {
            yield return null;
            elapsed += Time.deltaTime;
            item.Instance.transform.localPosition = Vector3.Lerp(
                from,
                to,
                Mathf.SmoothStep(0f, 1f, elapsed / duration)
            );
        }
    }

    private void GrabItem(int beltIndex)
    {
        if (_itemsOnBelt[beltIndex] != null)
        {
            // Find an empty spot
            var plateIndex = _itemsOnPlate.IndexOf(null);

            // No room!
            if (plateIndex == -1) return;

            // Move item from belt to plate
            _itemsOnPlate[plateIndex] = _itemsOnBelt[beltIndex];
            _itemsOnPlate[plateIndex].Instance.transform.parent = PlateNodes[plateIndex].transform;
            _itemsOnBelt[beltIndex] = null;
            StartCoroutine(MoveItem(_itemsOnPlate[plateIndex], _itemsOnPlate[plateIndex].Instance.transform.localPosition, new Vector3(0, 0, 0), .5f));
        }
    }

    private void ReturnItem(int plateIndex)
    {
        if (_itemsOnPlate[plateIndex] != null)
        {
            // Find an empty spot
            var beltIndex = _itemsOnBelt.IndexOf(null);

            // No room! (last spot is invalid, too close to leaving the belt and getting destroyed)
            if (beltIndex == -1 || beltIndex == _itemsOnBelt.Count - 1) return;

            // Move item from plate to belt
            _itemsOnBelt[beltIndex] = _itemsOnPlate[plateIndex];
            _itemsOnBelt[beltIndex].Instance.transform.parent = BeltNodes[beltIndex].transform;
            _itemsOnPlate[plateIndex] = null;
            StartCoroutine(MoveItem(_itemsOnBelt[beltIndex], _itemsOnBelt[beltIndex].Instance.transform.localPosition, new Vector3(0, 0, 0), .3f));
        }
    }

    class Item
    {
        public Ingredient Ingredient { get; set; }
        public GameObject Instance { get; set; }
    }

    class Pizza
    {
        public string Name { get; set; }
        public List<Ingredient> Ingredients { get; set; }
    }
}