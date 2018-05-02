using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Rnd = UnityEngine.Random;

public class PizzaModule : MonoBehaviour
{
    public KMSelectable Module;
    public KMSelectable Order;
    public KMSelectable[] BeltNodes;
    public KMSelectable[] PlateNodes;
    public GameObject[] Ingredients;
    public enum Ingredient
    {
        Artichokes, Bacon, Basil, BbqSauce, BellPeppers, BlackOlives, Cheddar, GrilledChickenBreast, Ham, ItalianSausage, Jalapeño, Mozzarella, Mushrooms, Mussels, Pepperoni, Pineapple, RedOnions, Scampi, Tomatoes, Tuna
    }
    public enum Pizza
    {
        Margherita, BbqChicken, BuffaloChicken, Adventure, IScream, FruttiDiMare, Hawaii, MeatLovers, Veggie, BaconCheddar, TunaDelight, QuattroStagioni
    }
    public enum Customer
    {
        Bob, Carlo, Clair, Frank, Frédérique, Ingrid, Melissa, Natasha, Sandy, Sigmund, Tyrone
    };

    private Dictionary<Ingredient, string> _ingredientNames;
    private Dictionary<Pizza, PizzaRecipe> _pizzaRecipes;
    private List<Item> _itemsOnBelt;
    private List<Item> _itemsOnPlate;
    private Queue<Ingredient> _queuedIngredients = new Queue<Ingredient>();
    private Pizza _pizza;
    private Customer _customer;

    void Start()
    {
        Order.transform.Find("Text").GetComponent<TextMesh>().text = "";

        _ingredientNames = new Dictionary<Ingredient, string>()
        {
            { Ingredient.Artichokes, "Artichokes" },
            { Ingredient.Bacon, "Bacon" },
            { Ingredient.Basil, "Basil" },
            { Ingredient.BbqSauce, "BBQ sauce" },
            { Ingredient.BellPeppers, "Bell peppers" },
            { Ingredient.BlackOlives, "Black olives" },
            { Ingredient.Cheddar, "Cheddar" },
            { Ingredient.GrilledChickenBreast, "Grilled chicken breast" },
            { Ingredient.Ham, "Ham" },
            { Ingredient.ItalianSausage, "Italian sausage" },
            { Ingredient.Jalapeño, "Jalapeño" },
            { Ingredient.Mozzarella, "Mozzarella" },
            { Ingredient.Mushrooms, "Mushrooms" },
            { Ingredient.Mussels, "Mussels" },
            { Ingredient.Pepperoni, "Pepperoni" },
            { Ingredient.Pineapple, "Pineapple" },
            { Ingredient.RedOnions, "Red onions" },
            { Ingredient.Scampi, "Scampi" },
            { Ingredient.Tomatoes, "Tomatoes" },
            { Ingredient.Tuna, "Tuna"}
        };
        _pizzaRecipes = new Dictionary<Pizza, PizzaRecipe>()
        {
            { Pizza.Margherita, new PizzaRecipe() { Name = "Margherita", Ingredients = new List<Ingredient>() {
                Ingredient.Tomatoes, Ingredient.Basil, Ingredient.Mozzarella
            } } },
            { Pizza.BbqChicken, new PizzaRecipe() { Name = "BBQ Chicken", Ingredients = new List<Ingredient>() {
                Ingredient.BbqSauce, Ingredient.GrilledChickenBreast, Ingredient.RedOnions, Ingredient.Bacon, Ingredient.Mozzarella
            } } },
            { Pizza.BuffaloChicken, new PizzaRecipe() { Name = "Buffalo Chicken", Ingredients = new List<Ingredient>() {
                Ingredient.BbqSauce, Ingredient.GrilledChickenBreast, Ingredient.RedOnions, Ingredient.Cheddar
            } } },
            { Pizza.Adventure, new PizzaRecipe() { Name = "Adventure", Ingredients = new List<Ingredient>() {
                Ingredient.Pepperoni, Ingredient.ItalianSausage, Ingredient.Artichokes, Ingredient.Scampi, Ingredient.Jalapeño, Ingredient.Cheddar
            } } },
            { Pizza.IScream, new PizzaRecipe() { Name = "I Scream", Ingredients = new List<Ingredient>() {
                Ingredient.Pepperoni, Ingredient.GrilledChickenBreast, Ingredient.BellPeppers, Ingredient.Mussels, Ingredient.Jalapeño, Ingredient.Jalapeño
            } } },
            { Pizza.FruttiDiMare, new PizzaRecipe() { Name = "Frutti di Mare", Ingredients = new List<Ingredient>() {
                Ingredient.Tuna, Ingredient.Scampi, Ingredient.Mussels, Ingredient.BlackOlives
            } } },
            { Pizza.Hawaii, new PizzaRecipe() { Name = "Hawaii", Ingredients = new List<Ingredient>() {
                Ingredient.Ham, Ingredient.Bacon, Ingredient.Pineapple, Ingredient.Mozzarella
            } } },
            { Pizza.MeatLovers, new PizzaRecipe() { Name = "Meat Lovers", Ingredients = new List<Ingredient>() {
                Ingredient.Pepperoni, Ingredient.Ham, Ingredient.ItalianSausage, Ingredient.Mozzarella
            } } },
            { Pizza.Veggie, new PizzaRecipe() { Name = "Veggie", Ingredients = new List<Ingredient>() {
                Ingredient.Mushrooms, Ingredient.BellPeppers, Ingredient.Artichokes, Ingredient.BlackOlives, Ingredient.Tomatoes
            } } },
            { Pizza.BaconCheddar, new PizzaRecipe() { Name = "Bacon Cheddar", Ingredients = new List<Ingredient>() {
                Ingredient.ItalianSausage, Ingredient.Bacon, Ingredient.Cheddar, Ingredient.Cheddar
            } } },
            { Pizza.TunaDelight, new PizzaRecipe() { Name = "Tuna Delight", Ingredients = new List<Ingredient>() {
                Ingredient.Tuna, Ingredient.RedOnions, Ingredient.BlackOlives, Ingredient.Mozzarella
            } } },
            { Pizza.QuattroStagioni, new PizzaRecipe() { Name = "Quattro Stagioni", Ingredients = new List<Ingredient>() {
                Ingredient.Artichokes, Ingredient.Tomatoes, Ingredient.Basil, Ingredient.Mushrooms, Ingredient.Ham, Ingredient.Mozzarella
            } } },
        };

        // Random order
        _pizza = (Pizza)Rnd.Range(0, Enum.GetValues(typeof(Pizza)).Length);
        _customer = (Customer)Rnd.Range(0, Enum.GetValues(typeof(Customer)).Length);
        Order.transform.Find("Text").GetComponent<TextMesh>().text = _pizzaRecipes[_pizza].Name + "\n" + _customer;

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

        Order.OnInteract += delegate () { CheckOrder(); return false; };

        StartCoroutine(MoveBelt());
    }

    void Update()
    {
    }

    private IEnumerator MoveBelt()
    {
        bool maybeAddSomething = true;

        while (true)
        {
            yield return null;

            // If the first node is empty, maybe add something
            if (maybeAddSomething)
            {
                if (Rnd.Range(0, 4) != 0)
                {
                    AddItemToBelt();
                }
                maybeAddSomething = false;
            }

            // If the nodes reached the starting point of the next node, reset the nodes and pass on all items to the next node
            if (BeltNodes[0].transform.localPosition.x > 4)
            {
                for (var i = BeltNodes.Length - 1; i >= 0; i--)
                {
                    // Remove stuff at the end of the belt
                    if (i == BeltNodes.Length - 1 && _itemsOnBelt[i] is Item)
                    {
                        Destroy(_itemsOnBelt[i].Instance.gameObject);
                    }

                    // Pass items on to the next node
                    if (i == 0)
                    {
                        _itemsOnBelt[i] = null;
                        maybeAddSomething = true;
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
                    beltNode.transform.localPosition.x + Time.deltaTime * 4f,
                    beltNode.transform.localPosition.y,
                    beltNode.transform.localPosition.z
                );
            }
        }
    }

    private void AddItemToBelt()
    {
        // Refresh queued ingredients
        if (_queuedIngredients.Count == 0)
        {
            var ingredients = new List<Ingredient>();
            foreach (var pizzaRecipe in _pizzaRecipes)
            {
                foreach (var ingredient in pizzaRecipe.Value.Ingredients)
                {
                    ingredients.Add(ingredient);
                }
            }

            ingredients.Shuffle();
            _queuedIngredients = new Queue<Ingredient>(ingredients);
        }

        // Add item to belt
        _itemsOnBelt[0] = new Item()
        {
            Ingredient = _queuedIngredients.Peek(),
            Instance = Instantiate(Ingredients[(int)_queuedIngredients.Dequeue()], BeltNodes[0].transform),
        };
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

    // Check if ALL and NOTHING BUT the needed ingredients for the ordered pizza (_pizza) are on the plate (_itemsOnPlate)
    private void CheckOrder()
    {
        var ingredientsOnPlate = _itemsOnPlate.Where(item => item is Item).Select(item => item.Ingredient).ToList();
        var neededIngredients = _pizzaRecipes[_pizza].Ingredients;

        // TESTING: always replace Mozzarella with Ham
        for (var i = 0; i < neededIngredients.Count; i++)
        {
            if (neededIngredients[i] == Ingredient.Mozzarella)
            {
                neededIngredients[i] = Ingredient.Ham;
            }
        }

        switch (_customer)
        {
            case Customer.Bob:
                break;
            case Customer.Carlo:
                break;
            case Customer.Clair:
                break;
            case Customer.Frank:
                break;
            case Customer.Frédérique:
                break;
            case Customer.Ingrid:
                break;
            case Customer.Melissa:
                break;
            case Customer.Natasha:
                break;
            case Customer.Sandy:
                break;
            case Customer.Sigmund:
                break;
            case Customer.Tyrone:
                break;

        }

        if (
            ingredientsOnPlate.Count == neededIngredients.Count &&
            ingredientsOnPlate.All(neededIngredients.Contains) &&
            neededIngredients.All(ingredientsOnPlate.Contains)
        )
        {
            GetComponent<KMBombModule>().HandlePass();
        }
        else
        {
            GetComponent<KMBombModule>().HandleStrike();
        }
    }

    class Item
    {
        public Ingredient Ingredient { get; set; }
        public GameObject Instance { get; set; }
    }

    class PizzaRecipe
    {
        public string Name { get; set; }
        public List<Ingredient> Ingredients { get; set; }
    }
}

static class MyExtensions
{
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Rnd.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static void Remove<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Rnd.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}