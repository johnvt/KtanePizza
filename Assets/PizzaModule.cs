using System;
using System.Collections;
using System.Collections.Generic;
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

    private Item[] _itemsOnBelt = new Item[3] { null, null, null };

    void Start()
    {
        for (int i = 0; i < BeltNodes.Length; i++)
        {
            var j = i;
            BeltNodes[i].OnInteract += delegate () { GrabItem(j); return false; };
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
                Debug.Log("Adding " + item.Ingredient.ToString() + ".");
            }

            // If the nodes reached the starting point of the next node, reset the nodes and pass on all items to the next node
            if (BeltNodes[0].transform.localPosition.x > -2)
            {

            }
            foreach (var item in _itemsOnBelt)
            {
                item.Instance.transform.localPosition = new Vector3(
                    item.Instance.transform.localPosition.x + Time.deltaTime * 5f,
                    item.Instance.transform.localPosition.y,
                    item.Instance.transform.localPosition.z
                );
            }

            // Remove stuff at the end of the belt
            for (var i = 0; i < _itemsOnBelt.Count; i++)
            {
                if (_itemsOnBelt[i].Instance.transform.localPosition.x > 6)
                {
                    Debug.Log("Removing " + _itemsOnBelt[i].Ingredient.ToString() + " because it reached the end of the belt, there are " + (_itemsOnBelt.Count - 1) + " items on the belt now.");
                    Destroy(_itemsOnBelt[i].Instance.gameObject);
                    _itemsOnBelt.RemoveAt(i);
                    UpdateChildren();
                }
            }
        }
    }

    private void GrabItem(int nodeIndex)
    {
        if (_itemsOnBelt[nodeIndex] != null)
        {
            _itemsOnBelt[nodeIndex].Instance.transform.parent = PlateNodes[0].transform;
            _itemsOnBelt[nodeIndex].Instance.transform.localPosition = new Vector3(0, 0, 0);
            _itemsOnBelt[nodeIndex] = null;
        }
    }

    class Item
    {
        public Ingredient Ingredient { get; set; }
        public GameObject Instance { get; set; }
    }
}