using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PawnFunctions;
using Weapons;
using Body;
using Attacks;
using Armors;
using System.Linq;

using static HealthFunctions;
//using HS = HealthFunctions;

public struct Vital
{
	public VitalSystem system;
	public float effectiveness;

	public Vital(VitalSystem vitalsystem, float efectiveness)
	{
		system = vitalsystem;
		effectiveness = efectiveness;
	}

	public Vital(Vital v)
    {
		this.system = v.system;
		this.effectiveness = v.effectiveness;
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
	[SerializeField] float totalBleedRate;
	public List<Vital> vitals = new List<Vital>();
	public List<Bodypart> bodyparts = new List<Bodypart>();
	List<Bodypart> bleedingBodyparts = new List<Bodypart>(); // so we dont have to search all bodyparts
	private float __pain;
	public float pain { get { return __pain; } set { __pain = Mathf.Clamp01(value); } }
	public float lastDamageTime { get; private set; }
	public Pawn lastAttacker { get; private set; }

	public string userFriendlyStatus { get; private set; }
	public PawnShockRating statusType { get; private set; } = PawnShockRating.Good;
	[Space]

	[Header("Components")]
	[SerializeField] Rigidbody2D rb;
	[SerializeField] Pawn ___p;
	[SerializeField] public Pawn p { get; private set; }
	[SerializeField] Animator anim;
	[SerializeField] CombatSystem combat;
	[SerializeField] PawnPathfind pfind;
	[SerializeField] SpriteRenderer weaponSprite;
	[SerializeField] SpriteRenderer shieldSprite;
	[SerializeField] ParticleSystem blood;
	[SerializeField] GameObject bloodPrefab;
	[SerializeField] GameObject bloodParent;

	public List<Wound> wounds = new List<Wound>();

	// todo: destroy seeker too?

	private void Awake()
	{
		p = ___p;
		foreach(Bodypart sex in BodypartManager.BodypartList)
        {
			// fuck this :(
			bodyparts.Add(new Bodypart(sex));
        }
		foreach(Vital vs in Loader.loader.defaultVitals)
        {
			vitals.Add(new Vital(vs));
        }
	}

    private void UpdateBodyparts(List<Bodypart> bps, float pain) { }
    private void UpdateShock(string reason) { }
    private void UpdateVitals(List<Vital> wounds, float pain) { }

    private void Start()
	{
		StartCoroutine(Bleed());
		bloodParent = GameManager2D.Instance.bloodParent;
	}

    protected void FixedUpdate()
    {
		// this is an unbelievably stupid way to do this
		// but if i have to use it, it should definitely be fixed time
		lastDamageTime += Time.fixedDeltaTime;
    }
	/// <summary>
	/// Amount instead of attack because attack values don't take into account skills, etc.
	/// </summary>
	public void TakeMeleeDamage(float amount, Weapon sourceWeapon, Pawn attacker, Attack attack)
    {
		lastDamageTime = 0;
		lastAttacker = attacker;

		Bodypart bp = GetBodypart();
		float armorDamageAmount = amount - amount /
					totalReduction(armorFromBodyparts(bp.Name, p), attack.damageType, backup: amount); 

		float brate = attack.damageType.ToString() == "Sharp" ? randomVariation(armorDamageAmount) : 0;
		bp.bleedingRate = brate;
		totalBleedRate += brate;
		DoDamage(bp, new Wound(attack.damageType.ToString(), sourceWeapon, armorDamageAmount, attack, brate));
		generateBloodSplatter(1);
	}

	public void TakeRangeDamage(float amount, Weapon sourceWeapon, Pawn attacker, DamageType rangeDamageType, Attack attack) // todo: this kinda sucks with the reptition
    {
		//if (sourceWeapon == null) // we need this for final build
		//	sourceWeapon = WeaponManager.Get("Empty");
		lastDamageTime = 0; // this is a stupid af solution.
		lastAttacker = attacker;

		Bodypart bp = GetBodypart();
		float armorDamageAmount = amount - amount /
					totalReduction(armorFromBodyparts(bp.Name, p), attack.damageType, backup: amount); // crisis averted: dividing by 0
																							   // -- Generate Wound --
		float brate = rangeDamageType == DamageType.Sharp ? randomVariation(armorDamageAmount) : 0;
		bp.bleedingRate = brate;
		totalBleedRate += brate;
		DoDamage(bp, new Wound(rangeDamageType.ToString(), sourceWeapon, armorDamageAmount, attack, brate));

		generateBloodSplatter(1);
	}		

    public Bodypart GetBodypart()
	{
		int x = Random.Range(0, 100);
		List<Bodypart> _a;

		if (x <= normalBodypartHitChance)
			 _a = bodyparts.FindAll(x => x.hitChance.Equals(HitChance.Normal)); // is this necessary 
		else
			_a = bodyparts.FindAll(x => x.hitChance.Equals(HitChance.Elevated));
		
		return _a[Random.Range(0, _a.Count)];
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
			vitals[(int)GetVitalI(p.effects)] = 
				new Vital(p.effects, Mathf.Clamp01(0.75f * (w.damage / (p.HP / p.TotalHP)) / 100));
		 // 75%
		else if (p.effects != VitalSystem.None && p.effectAmount.Equals(EffectAmount.Minor))
			vitals[(int)GetVitalI(p.effects)] =
				new Vital(p.effects, Mathf.Clamp01(0.25f * (w.damage / (p.HP / p.TotalHP)) / 100));
		                                            // 25%
		// --- //

		pain += p.painFactor*(p.TotalHP-p.HP)/100;

		vitals[(int)GetVitalI(VitalSystem.Conciousness)] = new Vital(VitalSystem.Conciousness,1f-pain); // sex

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

		TryUpdatePawnInfo();
	}

	public void TryUpdatePawnInfo()
    {
		if (PawnInfo.currentSelectedPawn == this.p)
		{
			GameManager2D.Instance.pawnInfo.UpdateHealth(bodyparts, pain);
			GameManager2D.Instance.pawnInfo.UpdateVitals(vitals, pain);
			GameManager2D.Instance.pawnInfo.UpdateShock(userFriendlyStatus, statusType);
		}
	}

    #region Death/Down
	IEnumerator Bleed()
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
		TryUpdatePawnInfo();

		yield return new WaitForSeconds(bleedRate * 10);
		StartCoroutine(Bleed());
	}

	public void Down(string reason)
	{
		if (PawnInfo.currentSelectedPawn == this.p)
			UpdateVitals(vitals, pain);

		userFriendlyStatus = reason;
		statusType = PawnShockRating.Warning;
		p.pawnDowned = true;

		Destroy(pfind); // well if you're downed you shouldnt get up this is a bad approach though especially for mods, TODO
		Destroy(combat);

		anim.speed = 0.5f;
		anim.Play("Down");
		weaponSprite.forceRenderingOff = true; // destroy's expensive so this should be like a 0.5% improvement. also it means we can EASILY get it back at any time
		shieldSprite.forceRenderingOff = true;

		if (PawnInfo.currentSelectedPawn == this.p)
			UpdateShock(reason);
	}

	public void Die(string reason) // todo: benchmark destroy vs just setting the parts inactive
	{
		TryUpdatePawnInfo();
		userFriendlyStatus = reason;

		CancelInvoke();
		anim.StopPlayback();//s
		p.dead = true;
		p.pawnDowned = true;
		statusType = PawnShockRating.Bad;
		rb.rotation = 45f;

		p.country.members.Remove(p);
		p.country.memberTransforms.Remove(transform);

		if(!PopulateRegiments.hideState)
			PopulateRegiments.updateAllRegimentsSelectNumber(Player.regimentSelectNumber);
		
		weaponSprite.forceRenderingOff = true; // destroy is expensive, so this is better. and if there is coming back from the dead for some reason, just flick it bcak on.
		Destroy(rb); // ez optimization, and prevents spaghetti code. le magnum opus.
		//Destroy(p); // we should NOT DO THIS!!
		Destroy(combat);
		Destroy(pfind);
		Destroy(this); // i thinkkkkkkkk
	}
    #endregion

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

	public int GetVitalI(VitalSystem v)
    {
		return vitals.FindIndex(x => x.system == v);
	}

	public float GetVital(VitalSystem v)
	{
		if (v == VitalSystem.Moving)
			return vitals.Find(x => x.system == VitalSystem.Conciousness).effectiveness * vitals.Find(x=>x.system==VitalSystem.Breathing).effectiveness * vitals.Find(x=>x.system==v).effectiveness;
		else
			return vitals.Find(x => x.system == VitalSystem.Conciousness).effectiveness * vitals.Find(x => x.system == v).effectiveness;
	} // for other files. WE DONT WANT THIS TO BE STATIC!!! OR ANY OF THESE PAWN FILES!!!
}
