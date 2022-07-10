using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using System;

public class ItemViewer : MonoBehaviour
{
    public static ItemViewer I;

    [SerializeField] UIDocument uidoc;
    VisualElement root;
    VisualElement scrollViewBox;

    [Space]
    [Header("Prefabs")]
    [SerializeField] VisualTreeAsset singleItem;
    [SerializeField] VisualTreeAsset linkItem;
    [SerializeField] VisualTreeAsset box;

    protected void Start()
    {
        if(I == null)
            I = this;
        I.root = I.uidoc.rootVisualElement;
        UIManager.MakeDraggable(I.root.Q<VisualElement>("ItemViewerParent"), I.root.Q<VisualElement>("ItemViewerParent").Q<VisualElement>("TitleBar"));
        I.root.Q<VisualElement>("ItemViewerParent").style.visibility = Visibility.Hidden;
        I.root.Q<Button>("CloseButton").clicked += delegate { I.root.Q<VisualElement>("ItemViewerParent").style.visibility = Visibility.Hidden; };
        I.scrollViewBox = I.root.Q<VisualElement>("Contents");
        I.DisplayItem(Animals.AnimalArmor.Get("Barding"));
    }

    public void DisplayItem(object item)
    {
        var t = item.GetType();
        if (t.BaseType != typeof(Item) && t.BaseType != typeof(Build))
            return;
        I.root.Q<VisualElement>("ItemViewerParent").style.visibility = Visibility.Visible;
        const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;// | BindingFlags.FlattenHierarchy;

        //MemberInfo[] baseMembers = t.BaseType.GetFields(bindingFlags).Cast<MemberInfo>().Concat(t.BaseType.GetProperties(bindingFlags)).ToArray();

        List<MemberInfo> members = t.GetFields(bindingFlags).Cast<MemberInfo>()
            .Concat(t.GetProperties(bindingFlags))
            .ToList();
        //members.InsertRange(0, baseMembers);

        if (t == typeof(Weapons.Weapon))
        {
            var s = (Weapons.WeaponType)t.GetProperty("Type").GetValue(item) == Weapons.WeaponType.Melee ? typeof(RangedAttribute) : typeof(MeleeAttribute);
            var oppositeS = s == typeof(RangedAttribute) ? typeof(MeleeAttribute) : typeof(RangedAttribute);

            foreach (MemberInfo field in new List<MemberInfo>(members))
            {
                var x = field.GetCustomAttributes(true);
                var tts = new List<Type>();
                foreach (object o in field.GetCustomAttributes(true))
                    tts.Add(o.GetType());

                if (tts.Contains(s) && !tts.Contains(oppositeS) )
                    members.Remove(field);
            }
        }

        int nameCount = 0;
        foreach(MemberInfo field in members)
        {
            object[] myAttributes = field.GetCustomAttributes(true);
            if (myAttributes.Length == 0) continue;
            if (GetValue(field, item) == null) continue;

            if(myAttributes[0].GetType() == typeof(XMLItem))
            {
                var prefab = I.singleItem.CloneTree();
                if (string.IsNullOrEmpty(GetValue(field, item).ToString()))
                    continue;
                var attrName = myAttributes[0].GetType().GetField("name").GetValue(myAttributes[0]).ToString();
                
                if (attrName == "Name" && nameCount == 1) continue;
                if (attrName == "Name") nameCount++;

                prefab.Q<Label>("Label").text = $"<color=#99776A>{attrName}</color>: {GetValue(field, item)}";
                I.scrollViewBox.Add(prefab);
            }
            if(myAttributes[0].GetType() == typeof(XMLItemLink))
            {
                var prefab = I.linkItem.CloneTree();
                var attrName = myAttributes[0].GetType().GetField("name").GetValue(myAttributes[0]).ToString();
                prefab.Q<Label>("Link").text = $"<color=#99776A>{attrName}:</color> {GetValue(field, item)}";

                prefab.Q<Label>("Link").RegisterCallback<MouseDownEvent>(
                    x => {
                        if(x.button == Keybinds.LeftMouse)
                            I.DisplayItem(GetValue(field, item));
                        });
            }
        }

    }

    public static object GetValue(MemberInfo memberInfo, object forObject)
    {
        switch (memberInfo.MemberType)
        {
            case MemberTypes.Field:
                return ((FieldInfo)memberInfo).GetValue(forObject);
            case MemberTypes.Property:
                return ((PropertyInfo)memberInfo).GetValue(forObject);
            default:
                throw new NotImplementedException();
        }
    }
}
