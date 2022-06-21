using Armors;
using PawnFunctions;
using Body;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Attacks;

public static class HealthFunctions
{
	public static float randomVariation(float orig)
	{
		return (float)System.Math.Round(Random.Range(0.75f, 1.25f) * orig, System.MidpointRounding.AwayFromZero);
	}

	public static List<Armor> armorFromBodyparts(string name, Pawn p)
	{
		List<Armor> temp = new List<Armor>();
		for (int i = 0; i < p.armor.Count; i++)
		{
			if (p.armor[i].covers.Contains(Bodypart.Get(name)))
				temp.Add(p.armor[i]);
		}
		return temp;
	}

	public static float totalReduction(List<Armor> a, DamageType dt, float backup = 0)
	{
		float r = 0;
		for (int i = 0; i < a.Count; i++)
		{
			r += a[i].getProtection(dt);
		}
		if (r <= 0) return backup;
		return r;
	}
}
