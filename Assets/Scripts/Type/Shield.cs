using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Shields
{
    public static class ShieldManager
    {
        public static List<Shield> ShieldList = new List<Shield>();

        public static Shield Create(string sourcefile, string name, string desc, float sharpProt, float bluntProt, float moveSpeedEffect, float baseBlockChnc, float siz) // creates if it DOESNT exist
        {
            Shield c = new Shield(sourcefile, name, desc, sharpProt, bluntProt, moveSpeedEffect, baseBlockChnc, siz);
            ShieldList.Add(c);
            return c;
        }
        public static Shield Get(int id)
        {
            try
            {
                return ShieldList.Find(x => x.ID == id);
            }
            catch (NullReferenceException)
            {
                DB.Attention($"Couldn't find Shield of id {id}");
                return null;
            }
        }
        public static Shield Get(string name)
        {
            if(ShieldList.Exists(x=>x.Name == name))
                return ShieldList.Find(x => x.Name == name);
            else
                DB.Attention($"Couldn't find Shield of name {name}");
                return null;
        }
    }
    public class Shield : Item
    {
        public float sharpProtection;
        public float bluntProtection;
        public float movementSpeedEffect;
        public float baseBlockChance;
        public float size;

        public Shield(string sourcefile, string name, string desc, float sharpProt, float bluntProt, float moveSpeedEffect, float baseBlockChnc, float siz) // same for this
            :base(name, desc, sourcefile)
        {
            sharpProtection = sharpProt;
            bluntProtection = bluntProt;
            movementSpeedEffect = moveSpeedEffect;
            baseBlockChance = baseBlockChnc;
            size = siz;
        }
    }
}