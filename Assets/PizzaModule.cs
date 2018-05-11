using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KmHelper;
using Rnd = UnityEngine.Random;

/// <summary>
/// TODO:
/// - no duplicate pizza+customer
/// - stop queue and orders on solve
/// - check needed on failed order??
/// </summary>
public class PizzaModule : MonoBehaviour
{
    public KMBombInfo Bomb;
    public KMSelectable Module;
    public KMSelectable Order;
    public GameObject NumServed;
    public KMSelectable[] BeltNodes;
    public KMSelectable[] PlateNodes;
    public GameObject[] Ingredients;

    private int _numToServe = 3;
    private int _numServed = 0;
    private int _numOtherIngredients = 6;
    private float _beltSpeed = 2f;
    private float _chanceToAddItem = .8f;
    private float _minWaitForNextOrder = 30f;
    private float _maxWaitForNextOrder = 60f;
    private float _minOrderDuration = 50f;
    private float _maxOrderDuration = 60f;

    private int _moduleId;
    private static int _moduleIdCounter = 1;

    private Dictionary<Ingredient, string> _ingredientNames;
    private Dictionary<Pizza?, PizzaRecipe> _pizzaRecipes;
    private List<Item> _itemsOnBelt;
    private List<Item> _itemsOnPlate;
    private Queue<Ingredient> _queuedIngredients = new Queue<Ingredient>();
    private Pizza? _pizza;
    private Customer? _customer;
    private List<Ingredient> _needed = new List<Ingredient>();

    private enum Ingredient
    {
        Artichokes, Bacon, Basil, BbqSauce, BellPeppers, BlackOlives, Cheddar, GrilledChickenBreast, Ham, ItalianSausage, Jalapeño, Mozzarella, Mushrooms, Mussels, Pepperoni, Pineapple, RedOnions, Scampi, Tomatoes, Tuna
    }
    private enum Pizza
    {
        Margherita, BbqChicken, BuffaloChicken, Adventure, IScream, FruttiDiMare, Hawaii, MeatLovers, Veggie, BaconCheddar, TunaDelight, QuattroStagioni
    }
    private enum Customer
    {
        Bob, Carlo, Clair, Frank, Frédérique, Ingrid, Melissa, Natasha, Sandy, Sigmund, Tyrone
    };

    void Start()
    {
        _moduleId = _moduleIdCounter++;

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
            { Ingredient.Tuna, "Tuna" }
        };
        _pizzaRecipes = new Dictionary<Pizza?, PizzaRecipe>()
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

        Order.OnInteract += delegate () { ServeOrder(); return false; };

        StartCoroutine(MoveBelt());
        StartCoroutine(PlaceOrders());
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
                if (Rnd.Range(0f, 1f) < _chanceToAddItem)
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
                    beltNode.transform.localPosition.x + Time.deltaTime * _beltSpeed,
                    beltNode.transform.localPosition.y,
                    beltNode.transform.localPosition.z
                );
            }
        }
    }

    private IEnumerator PlaceOrders()
    {
        RemoveOrder();

        while (true)
        {
            yield return new WaitForSeconds(.1f);
            if (_customer == null)
            {
                yield return new WaitForSeconds(Rnd.Range(_minWaitForNextOrder, _maxWaitForNextOrder));
                StartCoroutine(PlaceOrder());
            }
        }

    }

    private IEnumerator PlaceOrder()
    {
        // Random order
        _pizza = (Pizza)Rnd.Range(0, Enum.GetValues(typeof(Pizza)).Length);
        _customer = (Customer)Rnd.Range(0, Enum.GetValues(typeof(Customer)).Length);

        Order.transform.Find("Plane").GetComponent<Renderer>().enabled = true;
        Order.transform.Find("for").GetComponent<Renderer>().enabled = true;
        Order.transform.Find("Timer").GetComponent<Renderer>().enabled = true;
        Order.transform.Find("Text").GetComponent<Renderer>().enabled = true;
        Order.transform.Find("Text").GetComponent<TextMesh>().text = _pizzaRecipes[_pizza].Name + "\n" + _customer;

        _needed = _pizzaRecipes[_pizza].Ingredients;
        int i;

        switch (_customer)
        {
            case Customer.Bob:

                // Replace meat, fish and dairy products. No BOB: replace with Bell peppers. Unlit: replace with Tomatoes. Lit: replace with Mushrooms.
                for (i = _needed.Count - 1; i >= 0; i--)
                {
                    if (new Ingredient[] {
                        Ingredient.Bacon,
                        Ingredient.Cheddar,
                        Ingredient.GrilledChickenBreast,
                        Ingredient.Ham,
                        Ingredient.ItalianSausage,
                        Ingredient.Mozzarella,
                        Ingredient.Mussels,
                        Ingredient.Pepperoni,
                        Ingredient.Scampi,
                        Ingredient.Tuna
                    }.Contains(_needed[i]))
                    {
                        if (Bomb.IsIndicatorOff(Indicator.BOB))
                        {
                            _needed[i] = Ingredient.Tomatoes;
                        }
                        else if (Bomb.IsIndicatorOn(Indicator.BOB))
                        {
                            _needed[i] = Ingredient.Mushrooms;
                        }
                        else
                        {
                            _needed[i] = Ingredient.BellPeppers;
                        }
                    }
                }
                break;

            case Customer.Carlo:

                // Lit CAR: replace order with Margherita. Remove Pineapple. Replace Pepperoni with Bell peppers.
                if (Bomb.IsIndicatorOn(Indicator.CAR))
                {
                    _needed = _pizzaRecipes[Pizza.Margherita].Ingredients;
                }
                else
                {
                    for (i = _needed.Count - 1; i >= 0; i--)
                    {
                        if (_needed[i] == Ingredient.Pineapple)
                        {
                            _needed.RemoveAt(i);
                        }
                        else if (_needed[i] == Ingredient.Pepperoni)
                        {
                            _needed[i] = Ingredient.BellPeppers;
                        }
                    }
                }
                break;

            case Customer.Clair:

                // Remove all meat and fish. But if there’s an unlit CLR, don’t remove Bacon. If there’s a lit CLR, double up on Bacon.
                for (i = _needed.Count - 1; i >= 0; i--)
                {
                    if (new Ingredient[] {
                        Ingredient.GrilledChickenBreast,
                        Ingredient.Ham,
                        Ingredient.ItalianSausage,
                        Ingredient.Mussels,
                        Ingredient.Pepperoni,
                        Ingredient.Scampi,
                        Ingredient.Tuna
                    }.Contains(_needed[i]))
                    {
                        _needed.RemoveAt(i);
                    }
                    else if (_needed[i] == Ingredient.Bacon)
                    {
                        if (!Bomb.IsIndicatorPresent(Indicator.CLR))
                        {
                            _needed.RemoveAt(i);
                        }
                        else if (Bomb.IsIndicatorOn(Indicator.CLR))
                        {
                            _needed.Add(Ingredient.Bacon);
                        }
                    }
                }
                break;

            case Customer.Frank:

                // Remove BBQ Sauce. No FRK: replace with Tomatoes. Lit FRK: replace with Basil and Tomatoes.
                for (i = _needed.Count - 1; i >= 0; i--)
                {
                    if (_needed[i] == Ingredient.BbqSauce)
                    {
                        _needed.RemoveAt(i);
                        if (!Bomb.IsIndicatorPresent(Indicator.FRK))
                        {
                            _needed.Add(Ingredient.Tomatoes);
                        }
                        else if (Bomb.IsIndicatorOn(Indicator.FRK))
                        {
                            _needed.Add(Ingredient.Basil);
                            _needed.Add(Ingredient.Tomatoes);
                        }
                    }
                }
                break;

            case Customer.Frédérique:

                // No Red onions. No FRQ: replace with Cheddar. Lit FRQ: replace with Italian sausage.
                for (i = _needed.Count - 1; i >= 0; i--)
                {
                    if (_needed[i] == Ingredient.RedOnions)
                    {
                        _needed.RemoveAt(i);
                        if (!Bomb.IsIndicatorPresent(Indicator.FRQ))
                        {
                            _needed.Add(Ingredient.Cheddar);
                        }
                        else if (Bomb.IsIndicatorOn(Indicator.FRQ))
                        {
                            _needed.Add(Ingredient.ItalianSausage);
                        }
                    }
                }
                break;

            case Customer.Ingrid:

                // Replace Tomatoes with Red onions and the other way around. Unlit IND: replace Bell peppers with Mushrooms. Lit IND: replace Mushrooms with Bell peppers.
                for (i = _needed.Count - 1; i >= 0; i--)
                {
                    if (_needed[i] == Ingredient.RedOnions)
                    {
                        _needed[i] = Ingredient.Tomatoes;
                    }
                    else if (_needed[i] == Ingredient.Tomatoes)
                    {
                        _needed[i] = Ingredient.RedOnions;
                    }
                    else if (Bomb.IsIndicatorOff(Indicator.IND) && _needed[i] == Ingredient.BellPeppers)
                    {
                        _needed[i] = Ingredient.Mushrooms;
                    }
                    else if (Bomb.IsIndicatorOn(Indicator.IND) && _needed[i] == Ingredient.Mushrooms)
                    {
                        _needed[i] = Ingredient.BellPeppers;
                    }
                }
                break;

            case Customer.Melissa:

                // No Jalapeño or Pepperoni. No MSA: add one Pineapple. Lit MSA: add two Pineapple.
                for (i = _needed.Count - 1; i >= 0; i--)
                {
                    if (new Ingredient[] { Ingredient.Jalapeño, Ingredient.Pepperoni }.Contains(_needed[i]))
                    {
                        _needed.RemoveAt(i);
                    }
                }
                if (!Bomb.IsIndicatorPresent(Indicator.MSA))
                {
                    _needed.Add(Ingredient.Pineapple);
                }
                else if (Bomb.IsIndicatorOn(Indicator.MSA))
                {
                    _needed.Add(Ingredient.Pineapple);
                    _needed.Add(Ingredient.Pineapple);
                }
                break;

            case Customer.Natasha:

                // No NSA: add a Jalapeño. Lit NSA: add two Jalapeño.
                if (!Bomb.IsIndicatorPresent(Indicator.NSA))
                {
                    _needed.Add(Ingredient.Jalapeño);
                }
                else if (Bomb.IsIndicatorOn(Indicator.NSA))
                {
                    _needed.Add(Ingredient.Jalapeño);
                    _needed.Add(Ingredient.Jalapeño);
                }
                break;

            case Customer.Sandy:

                // No SND: no cheese. Lit SND: double up on cheese.
                for (i = _needed.Count - 1; i >= 0; i--)
                {
                    if (new Ingredient[] { Ingredient.Mozzarella, Ingredient.Cheddar }.Contains(_needed[i]))
                    {
                        if (!Bomb.IsIndicatorPresent(Indicator.SND))
                        {
                            _needed.RemoveAt(i);
                        }
                        else if (Bomb.IsIndicatorOn(Indicator.SND))
                        {
                            _needed.Add(_needed[i]);
                        }
                    }
                }
                break;

            case Customer.Sigmund:

                // No Italian Sausage or Mussels. Lit SIG: no fish. Unlit SIG, no meat.
                for (i = _needed.Count - 1; i >= 0; i--)
                {
                    if (new Ingredient[] { Ingredient.ItalianSausage, Ingredient.Mussels }.Contains(_needed[i]))
                    {
                        _needed.RemoveAt(i);
                    }

                    if (Bomb.IsIndicatorOn(Indicator.SIG)
                        && new Ingredient[] { Ingredient.Scampi, Ingredient.Tuna }.Contains(_needed[i]))
                    {
                        _needed.RemoveAt(i);
                    }
                    else if (Bomb.IsIndicatorOff(Indicator.SIG)
                        && new Ingredient[] { Ingredient.Bacon, Ingredient.GrilledChickenBreast, Ingredient.Ham, Ingredient.Pepperoni }.Contains(_needed[i]))
                    {
                        _needed.RemoveAt(i);
                    }
                }
                break;

            case Customer.Tyrone:

                // Lit TRN and no batteries: ignore the order
                // More special handling in CheckPlate()
                if (Bomb.IsIndicatorOn(Indicator.TRN) && Bomb.GetBatteryCount() == 0)
                {
                    _needed.Clear();
                }
                break;

        }

        RefreshQueuedIngredients();

        var neededLog = _needed.Select(x => _ingredientNames[x]).ToList();
        if (_customer == Customer.Tyrone)
        {
            neededLog.Add(Bomb.IsIndicatorPresent(Indicator.TRN) ? "anything to fill up" : "any meat or fish to fill up");
        }
        Debug.LogFormat("[Pizza #{0}] Incoming order: {1} for {2}. We need {3}.",
            _moduleId,
            _pizzaRecipes[_pizza].Name,
            _customer,
            String.Join(", ", neededLog.ToArray()));

        var elapsed = 0f;
        var duration = Rnd.Range(_minOrderDuration, _maxOrderDuration);
        while (elapsed < duration)
        {
            yield return null;
            elapsed += Time.deltaTime;
            Order.transform.Find("Timer").GetComponent<TextMesh>().text = Math.Ceiling(duration - elapsed).ToString();
        }

        RemoveOrder();
        EmptyPlate();
    }

    private IEnumerator MoveItem(Item item, Vector3 from, Vector3 to, float duration)
    {
        GetComponent<KMSelectable>().AddInteractionPunch(.1f);

        float elapsed = 0;
        while (elapsed < duration)
        {
            yield return null;

            if (item.Instance == null) yield break;
            elapsed += Time.deltaTime;
            item.Instance.transform.localPosition = Vector3.Lerp(
                from,
                to,
                Mathf.SmoothStep(0f, 1f, elapsed / duration)
            );
        }
    }

    private void AddItemToBelt()
    {
        // Refresh queued ingredients
        if (_queuedIngredients.Count == 0)
        {
            RefreshQueuedIngredients();
        }

        // Add item to belt
        _itemsOnBelt[0] = new Item()
        {
            Ingredient = _queuedIngredients.Peek(),
            Instance = Instantiate(Ingredients[(int)_queuedIngredients.Dequeue()], BeltNodes[0].transform),
        };
    }

    private void RefreshQueuedIngredients()
    {
        // Start with the needed ingredients
        var ingredients = _needed.Count > 0 ? new List<Ingredient>(_needed) : new List<Ingredient>();

        // And some random OTHER stuff
        Ingredient ingredient;
        for (var i = 0; i < _numOtherIngredients; i++)
        {
            do
            {
                ingredient = (Ingredient)Rnd.Range(0, Enum.GetValues(typeof(Ingredient)).Length);
            }
            while (_needed.Contains(ingredient));

            ingredients.Add(ingredient);
        }

        ingredients.Shuffle();
        _queuedIngredients = new Queue<Ingredient>(ingredients);
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

    private void ServeOrder()
    {
        if (_customer == null) return;

        if (CheckPlate())
        {
            GetComponent<KMSelectable>().AddInteractionPunch(.1f);

            Debug.LogFormat("[Pizza #{0}] {1} wanted a {2}. {1} got a {2}. {1} happy!",
                _moduleId,
                _customer,
                _pizzaRecipes[_pizza].Name);
            _numServed++;
            NumServed.transform.GetComponent<TextMesh>().text = _numServed.ToString() + "/" + _numToServe.ToString();
            if (_numServed == _numToServe)
            {
                GetComponent<KMBombModule>().HandlePass();
            }
        }
        else
        {
            Debug.LogFormat("[Pizza #{0}] {1} wanted a {2}. You used these ingredients: {3}. That’s not what {1} wanted. {1} sad.",
                _moduleId,
                _customer,
                _pizzaRecipes[_pizza].Name,
                String.Join(", ", _itemsOnPlate.Where(x => x is Item).Select(x => _ingredientNames[x.Ingredient]).ToArray()));
            GetComponent<KMBombModule>().HandleStrike();
        }
        RemoveOrder();
        EmptyPlate();
    }

    private void EmptyPlate()
    {
        for (var i = 0; i < _itemsOnPlate.Count; i++)
        {
            if (_itemsOnPlate[i] is Item)
            {
                Destroy(_itemsOnPlate[i].Instance.gameObject);
                _itemsOnPlate[i] = null;
            }
        }
    }

    private bool CheckPlate()
    {
        var have = _itemsOnPlate.Where(item => item is Item).Select(item => item.Ingredient).ToList();

        // Special handling for Tyrone
        if (_customer == Customer.Tyrone)
        {
            // His plate should always be full
            if (have.Count < PlateNodes.Length)
            {
                return false;
            }

            // Lit TRN and no batteries: ignore the order
            if (!(Bomb.IsIndicatorOn(Indicator.TRN) && Bomb.GetBatteryCount() == 0))
            {
                // Otherwise check each needed ingredient and remove it from the checklist
                for (var i = 0; i < _needed.Count; i++)
                {
                    if (have.IndexOf(_needed[i]) == -1)
                    {
                        return false;
                    }
                    else
                    {
                        have.RemoveAt(have.IndexOf(_needed[i]));
                    }
                }
            }

            // No TRN: can only be filled up with meat or fish
            if (!Bomb.IsIndicatorPresent(Indicator.TRN))
            {
                foreach (var ingredient in have)
                {
                    if (!(new Ingredient[] {
                        Ingredient.Bacon,
                        Ingredient.GrilledChickenBreast,
                        Ingredient.Ham,
                        Ingredient.ItalianSausage,
                        Ingredient.Mussels,
                        Ingredient.Pepperoni,
                        Ingredient.Scampi,
                        Ingredient.Tuna
                    }.Contains(ingredient)))
                    {
                        return false;
                    }
                }
            }
        }

        // Check if ALL and NOTHING BUT the needed ingredients for the ordered pizza are on the plate
        else
        {
            if (!_needed.OrderBy(x => x).SequenceEqual(have.OrderBy(x => x)))
            {
                return false;
            }
        }

        return true;
    }

    private void RemoveOrder()
    {
        _customer = null;
        _pizza = null;
        Order.transform.Find("Plane").GetComponent<Renderer>().enabled = false;
        Order.transform.Find("Text").GetComponent<Renderer>().enabled = false;
        Order.transform.Find("for").GetComponent<Renderer>().enabled = false;
        Order.transform.Find("Timer").GetComponent<Renderer>().enabled = false;
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
}