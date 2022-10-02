using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Weapons;

// for once, using OOP for what it's good at!
// todo: source modname for folder in that folder. big blcak penis men
public class Item
{
    static List<Item> items = new List<Item>();
    
    [XMLItem("Description")] public string Description { get; }
    [XMLItem("Name")] public string Name { get; }
    public int ID { get; }
    public string SourceFile { get; }
    public string SourceFileName { get; }

    public override string ToString()
    {
        return Name;
    }

    public Item(string name, string description, string sourcefile)
    {
        this.Name = name;
        this.Description = description;
        items.Add (this);
        this.ID = items.Count;
        this.SourceFile = sourcefile;
        this.SourceFileName = Path.GetFileName(sourcefile);
    }

    public static Item GetGenericItem(string name)
    {
        if (items.Exists(x => x.Name == name))
            return items.Find(x => x.Name == name);
        else
            DB.Attention($"Couldn't find [item] of name \"{name}\"");
            return null;
    }

    public static Item GetGenericItem(int id)
    {
        if (items.Exists(x => x.ID == id))
            return items.Find(x => x.ID == id);
        else
            DB.Attention($"Couldn't find [item] of id \"{id}\"");
        return null;
    }
}
