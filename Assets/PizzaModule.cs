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
    public enum Ingredient
    {
        Artichokes, Bacon, Basil, BbqSauce, BellPeppers, BlackOlives, Cheddar, GrilledChickenBreast, GroundBeef, Ham, ItalianSausage, Jalapeño, Mozzarella, Mushrooms, Mussels, Olives, Onions, Pepperoni, Pineapple, RedOnions, Scampi, Tomatoes, Tuna
    }
    public enum Pizza
    {
        Margherita, BbqChicken, BuffaloChicken, Adventure, IScream, FruttiDiMare, Hawaii, MeatLovers, Veggie, BaconCheddar, TunaDelight, QuattroStagioni
    }

    private Dictionary<Ingredient, string> _ingredientNames;
    private Dictionary<Pizza, PizzaRecipe> _pizzaRecipes;
    private List<Item> _itemsOnBelt;
    private List<Item> _itemsOnPlate;
    private List<Customer> _customers;

    void Start()
    {
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
            { Ingredient.GroundBeef, "Ground beef" },
            { Ingredient.Ham, "Ham" },
            { Ingredient.ItalianSausage, "Italian sausage" },
            { Ingredient.Jalapeño, "Jalapeño" },
            { Ingredient.Mozzarella, "Mozzarella" },
            { Ingredient.Mushrooms, "Mushrooms" },
            { Ingredient.Mussels, "Mussels" },
            { Ingredient.Olives, "Olives" },
            { Ingredient.Onions, "Onions" },
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
                Ingredient.BbqSauce, Ingredient.GrilledChickenBreast, Ingredient.RedOnions, Ingredient.Cheddar, Ingredient.Mozzarella
            } } },
            { Pizza.Adventure, new PizzaRecipe() { Name = "Adventure", Ingredients = new List<Ingredient>() {
                Ingredient.Pepperoni, Ingredient.Ham, Ingredient.BellPeppers, Ingredient.Scampi, Ingredient.Jalapeño, Ingredient.Mozzarella
            } } },
            { Pizza.IScream, new PizzaRecipe() { Name = "I Scream", Ingredients = new List<Ingredient>() {
                Ingredient.Pepperoni, Ingredient.Ham, Ingredient.BellPeppers, Ingredient.Mussels, Ingredient.Jalapeño, Ingredient.Jalapeño, Ingredient.Mozzarella
            } } },
            { Pizza.FruttiDiMare, new PizzaRecipe() { Name = "Frutti di Mare", Ingredients = new List<Ingredient>() {
                Ingredient.Tuna, Ingredient.Scampi, Ingredient.Mussels, Ingredient.BlackOlives
            } } },
            { Pizza.Hawaii, new PizzaRecipe() { Name = "Hawaii", Ingredients = new List<Ingredient>() {
                Ingredient.Ham, Ingredient.Bacon, Ingredient.Pineapple, Ingredient.Mozzarella
            } } },
            { Pizza.MeatLovers, new PizzaRecipe() { Name = "Meat Lovers", Ingredients = new List<Ingredient>() {
                Ingredient.Pepperoni, Ingredient.Ham, Ingredient.ItalianSausage, Ingredient.GroundBeef, Ingredient.Mozzarella
            } } },
            { Pizza.Veggie, new PizzaRecipe() { Name = "Veggie", Ingredients = new List<Ingredient>() {
                Ingredient.Mushrooms, Ingredient.BellPeppers, Ingredient.Onions, Ingredient.BlackOlives, Ingredient.Tomatoes, Ingredient.Mozzarella
            } } },
            { Pizza.BaconCheddar, new PizzaRecipe() { Name = "Bacon Cheddar", Ingredients = new List<Ingredient>() {
                Ingredient.GroundBeef, Ingredient.Bacon, Ingredient.Cheddar, Ingredient.Mozzarella
            } } },
            { Pizza.TunaDelight, new PizzaRecipe() { Name = "Tuna Delight", Ingredients = new List<Ingredient>() {
                Ingredient.Tuna, Ingredient.RedOnions, Ingredient.BlackOlives, Ingredient.Mozzarella
            } } },
            { Pizza.QuattroStagioni, new PizzaRecipe() { Name = "Quattro Stagioni", Ingredients = new List<Ingredient>() {
                Ingredient.Artichokes, Ingredient.Tomatoes, Ingredient.Basil, Ingredient.Mushrooms, Ingredient.Ham, Ingredient.Mozzarella
            } } },
        };

        _customers = new List<Customer>() {
            new Customer { Name = "Bob", Description = "<b>Bob</b> is a vegan. There is no pleasing Bob. Don’t be like Bob. No meat, fish or dairy products. If there’s an unlit BOB, replace them with Tomatoes. If there’s a lit BOB, replace them with Mushrooms." },
            new Customer { Name = "Carlo", Description = "<b>Carlo</b> is Italian. Never put Pineapple on his pizza. Also, replace Pepperoni with Bell Peppers*. If there’s a lit CAR, replace the order with a Margherita.<br/><small>*) in Italy, Bell Peppers are called Peperoni (one p)</small>" },
            new Customer { Name = "Clair", Description = "<b>Clair</b> is vegetarian*. Most of the time. As a general rule, remove all meat and fish. If there’s an unlit CLR, don’t remove Bacon. If there’s a lit CLR, double up on Bacon.<br/><small>*) she does eat cheese</small>" },
            new Customer { Name = "Frank", Description = "<b>Frank</b> is diabetic. Remove BBQ Sauce or his foot will fall off. If there’s no FRK, replace with Tomatoes. If there’s a lit FRK, replace with Basil and Tomatoes." },
            new Customer { Name = "Frédérique", Description = "<b>Frédérique</b> is allergic to Onions and Red onions. If there’s an unlit FRQ, replace with Cheddar. If there’s a lit FRQ, replace them with Italian sausage." },
            new Customer { Name = "Ingrid", Description = "<b>Ingrid</b> is ... strange. Always replace Onions with Red onions and the other way around. If there’s an unlit IND, replace Bell peppers with Mushrooms. If there’s a lit IND, replace Mushrooms with Bell peppers." },
            new Customer { Name = "Melissa", Description = "<b>Melissa</b> is 10 years old. No Jalapeño. If there’s no MSA, add one Pineapple. If there’s a lit MSA, add two Pineapple." },
            new Customer { Name = "Natasha", Description = "<b>Natasha</b> likes to spice things up. If there is no NSA, add a Jalapeño. If there is a lit NSA, add two Jalapeño." },
            new Customer { Name = "Sandy", Description = "<b>Sandy</b> is lactose intolerant. If there’s no SND, no cheese. If there is an SND, she has taken her lactose pills. If it's unlit, just leave the cheese. If it's lit, double up on cheese." },
            new Customer { Name = "Sigmund", Description = "<b>Sigmund</b> should not get any Italian Sausage or Mussels. Otherwise he’ll try to convince you of having a castration complex. If there’s a lit SIG, no fish. If there’s an unlit SIG, no meat." },
            new Customer { Name = "Tyrone", Description = "<b>Tyrone</b> hungry. Always fill up as much as you can with any meat or fish. If there is an unlit TRN, use anything to fill up. If there’s a lit TRN, ignore the order completely and just throw on it whatever you please, as much as you can."}
        };

        var manual = "";
        foreach (var pizza in _pizzaRecipes)
        {
            manual += "<div><b>" + pizza.Value.Name + "</b><br/>"
                + String.Join(", ", pizza.Value.Ingredients.Select(ingredient => _ingredientNames[ingredient]).ToArray()) + "</div>";
        }
        Debug.Log(manual);
        manual = "";
        foreach (var customer in _customers)
        {
            manual += "<div>" + customer.Description + "</div>";
        }
        Debug.Log(manual);

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

    class PizzaRecipe
    {
        public string Name { get; set; }
        public List<Ingredient> Ingredients { get; set; }
    }

    class Customer
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}