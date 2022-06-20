using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PawnFunctions;

public class FlagBehaviour : MonoBehaviour
{
    [SerializeField] SpriteRenderer flag;
    public Sprite flagTexture;

    void Start() =>
        flag.sprite = flagTexture;
}