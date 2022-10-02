using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Regiments;

public class MiniMap : MonoBehaviour
{
    UIDocument uidoc;
    VisualElement root;
    VisualElement map;
    VisualElement image;
    VisualTreeAsset REGIMENT;

    float multiplier = 5.3f/2f;

    protected void Start()
    {
        root = uidoc.rootVisualElement;
        map = root.Q<VisualElement>("MiniMap");
        image = map.Q<VisualElement>("Image");
    }

    /*protected void Update()
    {
        image.Clear();
        foreach(Regiment r in Regiment.List)
        {
            VisualElement v = REGIMENT.CloneTree();
            v.style.translate =
                new Translate(Player.transform.position.x * multiplier,
                Player.transform.position.z * -multiplier, 0);
            _playerRepresentation.style.rotate = new Rotate(
                new Angle(Player.transform.rotation.eulerAngles.y));
        }
    }*/
}
