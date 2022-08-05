using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Animals;
using PawnFunctions;
using XMLLoader;
using static CachedItems;
using Pathfinding;

// im gonna need a pawninfo for this arent i?
public class AnimalBehavior : MonoBehaviour
{
    public Pawn rider;
    public Animal sourceAnimal;
    public float trueSpeed;
    public bool isWarAnimal;
    public bool isDead;
    public Animator animator;

    [SerializeField] ParticleSystem blood;
    [SerializeField] GameObject bloodPrefab;
    public SpriteRenderer spr; // needs to be public to save a getcomponent call
    
    int __hitpoints;
    public int hitpoints
    {
        get
        {
            return __hitpoints;
        }
        set
        {
            __hitpoints = value;
            if (__hitpoints <= 0)
            {
                if (rider != null)
                {
                    rider.animal = null;
                    rider.pawnPathfind.speed /= trueSpeed;
                }
                transform.Rotate(new Vector3(0,0,45));
            }
        } 
    }
    public List<AnimalArmor> armors;

    protected void Start()
    {
        if(sourceAnimal == null)
        {
            DB.Attention("Null source animal, somehow");
            return;
        }
        isWarAnimal = sourceAnimal.ridable; // if not ridable, just wander around
        trueSpeed = sourceAnimal.speedEffect;

        if(armors != null)
            foreach (AnimalArmor armor in armors)
            {
                if (armor.forAnimal.Name == sourceAnimal.Name)
                    trueSpeed += armor.moveSpeedEffect;
                else
                {
                    Debug.Log($"Removing ILLEGAL armor");
                    armors.Remove(armor);
                }
            }

        if (isWarAnimal)
        {
            rider.pawnPathfind.speed *= trueSpeed; // cheeky
            transform.parent = rider.transform; // the z level is slightly above the pawn. i'll add size attribute later todo
        }
        #region Render                                      
        if (CachedItems.renderedAnimals.Exists(x => x.id == sourceAnimal && x.animalArmor == armors)) // we don't need to check the image contents hash here because 1. that would be slow. 2. it's contained in the animal type already so it wouldn't be equal if it wasnt't there
            spr.sprite = CachedItems.renderedAnimals.Find(x => x.id == sourceAnimal && x.animalArmor == armors).finalSprite;
        else
        {
            List<Sprite> animalArmorSprites = new List<Sprite>();
            Sprite srcAnimalSprite;
            Texture2D finalTex;

            if (renderedAnimalPicks.Exists(x => x.animal == sourceAnimal))
            {
                var x = renderedAnimalPicks.Find(x => x.animal == sourceAnimal);
                srcAnimalSprite = x.picks[Random.Range(0, x.picks.Count)];
            }
            else
            {
                List<Sprite> picks = new List<Sprite>();
                byte[] pickTex = Loaders.LoadImage(sourceAnimal.SourceFile);

                Texture2D fullImage = new Texture2D(1,1);
                fullImage.LoadImage(pickTex);
                fullImage.Apply();

                picks = SpriteSheetCreator.createSpritesFromSheet(pickTex, 512, 512);

                srcAnimalSprite = picks[Random.Range(0, picks.Count)];
                
                var hash = new Hash128();
                hash.Append(fullImage.GetPixels());

                sourceAnimal.spriteHash = hash;
                renderedAnimalPicks.Add(new RenderedAnimalPick(picks, sourceAnimal));
            }
            if(armors != null)
                foreach (AnimalArmor a in armors)
                {
                    if(renderedAnimalArmors.Exists(x=>x.animalArmor==a))
                        animalArmorSprites.Add(renderedAnimalArmors.Find(x=>x.animalArmor==a).spr);
                    else // make the animal armor
                    {
                        Sprite spr = Sprite.Create(Loaders.LoadTex(a.SourceFile), new Rect(0,0,512,512), Vector2.zero, 512);
                        renderedAnimalArmors.Add(new RenderedAnimalArmor(a, spr));
                        animalArmorSprites.Add(spr); // almost forgot this
                    }
                }
            finalTex = srcAnimalSprite.texture;
            foreach(Sprite p in animalArmorSprites)
                finalTex = PawnRenderer.CombineTextures(finalTex, p.texture);

            Sprite finalSprite = Sprite.Create(finalTex, new Rect(0,0,512,512), Vector2.zero);

            renderedAnimals.Add(new RenderedAnimal(armors, srcAnimalSprite, sourceAnimal, finalSprite));
        }
        #endregion
    }

    protected void Update()
    {
        if (isWarAnimal || isDead)
            return;

        // pathfind without pawn
    }

    public void TakeDamage(int amount)
    {
        if (armors.Count > 0)
            foreach (AnimalArmor a in armors)
                hitpoints -= (a.protection / 100) * amount;
        else
            hitpoints -= amount;

        generateBloodSplatter(1);
    }

    public void TakeBurn(int amount)
    {
        hitpoints -= amount;
    }

    public void generateBloodSplatter(int amount)
    {
        // todo: object pooling
        for (int i = 0; i < amount; i++)
        {
            blood.Play();
            GameObject _ = Instantiate(bloodPrefab);
            var temp = transform.position;
            _.transform.position = new Vector2(temp.x + Random.Range(0f, 1f), temp.y + Random.Range(0f, 1f));
        }
    }
}
