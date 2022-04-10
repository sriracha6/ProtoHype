using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// for once, using OOP for what it's good at!
static class ItemsList
{
    public static List<Item> items = new List<Item> ();
}
public class Item
{
    public string Name;
    public string Description;
    public int ID { get; private set; }

    public Item(string name, string description)
    {
        this.Name = name;
        this.Description = description;
        ItemsList.items.Add (this);
        this.ID = ItemsList.items.Count;
    }
}
