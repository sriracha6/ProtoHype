using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PawnFunctions;
using Weapons;
using Body;
using Attacks;
using Armors;

public struct Vital
{
	public VitalSystem system;
	public float effectiveness;

	public Vital(VitalSystem vitalsystem, float efectiveness)
	{
		system = vitalsystem;
		effectiveness = efectiveness;
	}
}

public class HealthSystem : MonoBehaviour
{
	//[Header("Settings")]
	private float _blood = 10f;
	public float totalBlood { get { return _blood; } set { _blood = Mathf.Clamp(value, 0, maxBlood); } }
	public const float bleedToBlood = 0.3f;
	public const float bleedRate = 1f;
	public const float bleedHPLoss = 1f;
	public const float maxBlood = 10f;
	[Space]
	public const int normalBodypartHitChance = 25;
	
	//[Header("Info")] // :)
	[HideInInspector] public bool isAlive { get; private set; }
	[SerializeField] float totalBleedRate;
	[SerializeField] public List<Vital> vitals = new List<Vital>();
	public List<Bodypart> bodyparts = new List<Bodypart>();
	List<Bodypart> bleedingBodyparts = new List<Bodypart>(); // so we dont have to search all bodyparts
	private float __pain;
	public float pain { get { return __pain; } set { __pain = Mathf.Clamp01(value); } }
	[Space]

	[Header("Components")]
	[SerializeField] Rigidbody2D rb;
	[SerializeField] Pawn p;
	[SerializeField] CombatSystem combat;
	[SerializeField] PawnPathfind pfind;
	[SerializeField] SpriteRenderer weaponSprite;
	[SerializeField] ParticleSystem blood;
	[SerializeField] GameObject bloodPrefab;
	[SerializeField] GameObject bloodParent;

	public List<Wound> wounds = new List<Wound>();

	public delegate void NotifyPawnInfo(List<Vital> wounds, float pain);
	public delegate void NotifyPawnShock(string reason);
	public delegate void NotifyPawnBPs(List<Bodypart> bps, float pain);
	
	// todo: destroy seeker too?

	private void Awake()
	{
		NotifyPawnInfo v = new NotifyPawnInfo(UpdateVitals);
        NotifyPawnShock s = new NotifyPawnShock(UpdateShock);
		NotifyPawnBPs b = new NotifyPawnBPs(UpdateBodyparts);

		bodyparts = new List<Bodypart>(BodypartManager.BodypartList); // WE DONT WANT EVERYONE TO HAVE THE SAME PARTS. CRISIS AVERTED!!!!!
		// todo: add random to make it feel ^^ more geniune?

		vitals.Add(new Vital(VitalSystem.Dexterity, 1f));          
		vitals.Add(new Vital(VitalSystem.Sight, 1f));              
		vitals.Add(new Vital(VitalSystem.Breathing, 1f));          
		vitals.Add(new Vital(VitalSystem.Conciousness, 1f));       //
		vitals.Add(new Vital(VitalSystem.BloodPumping, 1f));       
		vitals.Add(new Vital(VitalSystem.Moving, 1f));             
	}

    private void UpdateBodyparts(List<Bodypart> bps, float pain) { }
    private void UpdateShock(string reason) { }
    private void UpdateVitals(List<Vital> wounds, float pain) { }

    private void Start()
	{
		InvokeRepeating(nameof(Bleed), 0f, bleedRate * 10f);
		bloodParent = GameManager2D.Instance.bloodParent;
	}

	public void SubtractHealth(float amount, Weapon sourceWeapon, Attack attack = null, DamageType rangeDType = DamageType.None) // 0: melee, 1:range this is a bad solution
	{
		//if (sourceWeapon == null) // we need this for final build
		//	sourceWeapon = WeaponManager.Get("Empty");

		Bodypart bp = GetBodypart();
		float armorDamageAmount = amount - amount /
					totalReduction(armorFromBodyparts(bp),attack.damageType,backup: amount); // crisis averted: dividing by 0
		// -- Generate Wound --
		if (rangeDType != DamageType.None) // this is bad code
		{
			float brate = rangeDType == DamageType.Sharp ? randomVariation(armorDamageAmount) : 0;
			totalBleedRate += brate;
			DoDamage(bp, new Wound(rangeDType.ToString(), sourceWeapon, armorDamageAmount, attack, brate));
			//Debug.Log($"NEW WOUND\n---------------\nDAMAGE:{wounds[wounds.Count - 1].damage}\nBLEEDRATE:{wounds[wounds.Count - 1].bleedRate}\nTYPE:{wounds[wounds.Count - 1].type}", gameObject);

		}          // Range
		else
		{
			float brate = attack.damageType.ToString() == "Sharp" ? randomVariation(armorDamageAmount) : 0;
			totalBleedRate += brate;
			DoDamage(bp, new Wound(attack.damageType.ToString(), sourceWeapon, armorDamageAmount, attack, brate));
			//Debug.Log($"NEW PROJECTILE WOUND\n---------------\nDAMAGE:{wounds[wounds.Count - 1].damage}\nBLEEDRATE:{wounds[wounds.Count - 1].bleedRate}\nTYPE:{wounds[wounds.Count - 1].type}", gameObject);
		}                               // elee
		generateBloodSplatter(1);
	}

    public Bodypart GetBodypart()
	{
		int x = Random.Range(0, 100);
		if (x <= normalBodypartHitChance)
		{
			List<Bodypart> _ = bodyparts.FindAll(x => x.hitChance.Equals(HitChance.Normal)); // is this necessary 
			return _[Random.Range(0, _.Count)];
		}
		else
		{
			List<Bodypart> _ = bodyparts.FindAll(x => x.hitChance.Equals(HitChance.Elevated));
			return _[Random.Range(0, _.Count)];
		}
	}

	public void DoDamage(Bodypart p, Wound w)
	{
		if (p.wounds.Count == 0 && w.bleedRate > 0)
		{
			bleedingBodyparts.Add(p); // we wont have to remove bc no one lives long enough
			totalBleedRate += p.bleedingFactor*w.bleedRate;
		}
		p.HP -= Mathf.Clamp(p.damageMultiplier * w.damage,0,float.MaxValue);
		p.effectiveness -= w.damage;

		if (p.HP <= 0)
			p.effectiveness = 0;

		// --- REFRESH VITALS --- 
		if (p.effects != VitalSystem.None && p.effectAmount.Equals(EffectAmount.Normal)) // 75%
		{
			vitals[(int)GetVital(p.effects, 1)] = new Vital(p.effects, Mathf.Clamp01(0.75f * (w.damage / (p.HP / p.TotalHP)) / 100));
		} // 75%
		else if (p.effects != VitalSystem.None && p.effectAmount.Equals(EffectAmount.Minor))
		{
			vitals[(int)GetVital(p.effects, 1)] = new Vital(p.effects, Mathf.Clamp01(0.25f * (w.damage / (p.HP / p.TotalHP)) / 100));
		}                                            // 25%
		// --- //

		pain += p.painFactor*(p.TotalHP-p.HP)/100;

		vitals[(int)GetVital(VitalSystem.Conciousness,1)] = new Vital(VitalSystem.Conciousness,1f-pain); // sex

		if (pain >= 1f)
			Down("in neurogenic shock"); // neurogenic shock
		else if (GetVital(VitalSystem.Conciousness) <= 0.25f)
			Down("unconcious");
		else if (GetVital(VitalSystem.Conciousness) <= 0f)
			Die("no conciousness");

        if (p.type == PartType.VitalOrgan && p.HP <= 0)
            Die("die vital organ");
		// todo: check if it's open before you update it pointlessly. ctrl + f this file to see all 100000 instances
        p.wounds.Add(w);

		UpdateVitals(vitals, pain);
	}

    #region Death/Down
    public void Down(string reason)
	{
		UpdateVitals(vitals, pain);
		pfind.notDowned = false;
		
		rb.rotation = 45f;
		weaponSprite.forceRenderingOff = true; // destroy is expensive, so this is better. and if there is coming back from the dead for some reason, just flick it bcak on.
		UpdateShock(reason);
	}
	public void Bleed()
	{
		totalBlood -= Mathf.Clamp(GetVital(VitalSystem.BloodPumping) * (bleedToBlood * totalBleedRate), 0f, maxBlood);

		if (totalBlood <= 3f) // on god. there isn't a better way to do this
			Down("under 30% blood");
		else if (totalBlood <= 0f)
			Die("no blood");
		if (GetVital(VitalSystem.Conciousness) <= 0.25f)
			Down("little conciousness");
		else if (GetVital(VitalSystem.Conciousness) <= 0f)
			Die("no conciousness");
		else if (GetVital(VitalSystem.Moving) <= 0.25f)
			Down("under 25% moving");

		foreach (Bodypart bp in bleedingBodyparts)
		{
			bp.HP -= bleedHPLoss * bp.wounds.Count; // todo
		}
		GameManager2D.Instance.pawnInfo.UpdateHealth(bodyparts, pain);
		GameManager2D.Instance.pawnInfo.UpdateVitals(vitals, pain);
	}
	public void Die(string reason) // todo: benchmark destroy vs just setting the parts inactive
	{
		GameManager2D.Instance.pawnInfo.UpdateVitals(vitals, pain); // events
		GameManager2D.Instance.pawnInfo.UpdateHealth(bodyparts, 0);
		CancelInvoke();
		isAlive = false;
		p.dead = true;
		rb.rotation = 45f;
		weaponSprite.forceRenderingOff = true; // destroy is expensive, so this is better. and if there is coming back from the dead for some reason, just flick it bcak on.
		Destroy(rb); // ez optimization, and prevents spaghetti code. le magnum opus.
		//Destroy(p); // we should NOT DO THIS!!
		Destroy(combat);
		Destroy(pfind);
		Destroy(this); // i thinkkkkkkkk
	}
    #endregion

    #region Functions
	public void generateBloodSplatter(int amount)
    {
		// todo: object pooling
		for (int i = 0; i < amount; i++)
		{
			blood.Play();
			GameObject _ = Instantiate(bloodPrefab, bloodParent.transform);
			_.GetComponent<SpriteRenderer>().sprite = CachedItems.bloodSplatters[Random.Range(0, CachedItems.bloodSplatters.Count)];
			var temp = transform.position;
			_.transform.position = new Vector2(temp.x + Random.Range(0f, 1f), temp.y + Random.Range(0f, 1f));
		}
	}
	public float randomVariation(float orig)
	{
		return (float)System.Math.Round(Random.Range(0.75f, 1.25f) * orig, System.MidpointRounding.AwayFromZero);
	}
	public List<Armor> armorFromBodyparts(Bodypart b)
    {
		List<Armor> temp = new List<Armor>();
		for (int i = 0; i<p.armor.Count; i++)
        {
			if (p.armor[i].covers.Contains(b))
				temp.Add(p.armor[i]);
        }
		return temp;
    }
	public float totalReduction(List<Armor> a,DamageType dt,float backup=0)
    {
		float r = 0;
		for(int i = 0; i < a.Count; i++)
        {
			r += a[i].getProtection(dt);
        }
        if (r <= 0) { return backup; }
		return r;
    }
	public float GetVital(VitalSystem v , int mode=0)
	{
		if (mode == 1)
		{
			return vitals.FindIndex(x => x.system == v);
		}

		if (v == VitalSystem.Moving)
		{
			return vitals.Find(x => x.system == VitalSystem.Conciousness).effectiveness * vitals.Find(x=>x.system==VitalSystem.Breathing).effectiveness * vitals.Find(x=>x.system==v).effectiveness;
		}
		else
		{
			return vitals.Find(x => x.system == VitalSystem.Conciousness).effectiveness * vitals.Find(x => x.system == v).effectiveness;
		}
	} // for other files. WE DONT WANT THIS TO BE STATIC!!! OR ANY OF THESE PAWN FILES!!!
    #endregion
}
