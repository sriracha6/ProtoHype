using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Weapons;

// for once, using OOP for what it's good at!
// todo: source modname for folder in that folder. big blcak penis men
static class ItemsList
{
    public static List<Item> items = new List<Item> ();
}
public class Item
{
    public string Name { get; }
    public string Description { get; }
    public int ID { get; }
    public string SourceFile { get; }

    public Item(string name, string description, string sourcefile)
    {
        this.Name = name;
        this.Description = description;
        ItemsList.items.Add (this);
        this.ID = ItemsList.items.Count;
        this.SourceFile = Path.GetFileName(sourcefile);
    }
}
