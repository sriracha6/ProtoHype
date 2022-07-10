using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Shields
{
    [ImageList(typeof(CachedItems.RenderedShield))]
    public class Shield : Item
    {
        [XMLItem("Sharp Protection")] public float sharpProtection;
        [XMLItem("Blunt Protection")] public float bluntProtection;
        [XMLItem("Movement Speed Affect")] public float movementSpeedEffect;
        [XMLItem("Base Block Chance")] public float baseBlockChance;
        [XMLItem("Size")] public float size;

        public Shield(string sourcefile, string name, string desc, float sharpProt, float bluntProt, float moveSpeedEffect, float baseBlockChnc, float siz) // same for this
            :base(name, desc, sourcefile)
        {
            sharpProtection = sharpProt;
            bluntProtection = bluntProt;
            movementSpeedEffect = moveSpeedEffect;
            baseBlockChance = baseBlockChnc;
            size = siz;
        }

        public static List<Shield> List = new List<Shield>();

        public static Shield Create(string sourcefile, string name, string desc, float sharpProt, float bluntProt, float moveSpeedEffect, float baseBlockChnc, float siz) // creates if it DOESNT exist
        {
            Shield c = new Shield(sourcefile, name, desc, sharpProt, bluntProt, moveSpeedEffect, baseBlockChnc, siz);
            List.Add(c);
            return c;
        }
        public static Shield Get(int id)
        {
            try
            {
                return List.Find(x => x.ID == id);
            }
            catch (NullReferenceException)
            {
                DB.Attention($"Couldn't find Shield of id {id}");
                return null;
            }
        }
        public static Shield Get(string name)
        {
            if (List.Exists(x => x.Name == name))
                return List.Find(x => x.Name == name);
            else
                DB.Attention($"Couldn't find Shield of name {name}");
            return null;
        }
    }
}