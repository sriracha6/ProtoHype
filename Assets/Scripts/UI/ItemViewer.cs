using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using Armors;

public class ItemViewer : MonoBehaviour
{
    public static ItemViewer I;

    [SerializeField] UIDocument uidoc;
    VisualElement root;
    VisualElement scrollViewBox;

    [SerializeField] VisualTreeAsset ITEMVIEWERASSET;
    [Space]
    [Header("Prefabs")]
    [SerializeField] VisualTreeAsset singleItem;
    [SerializeField] VisualTreeAsset linkItem;
    [SerializeField] VisualTreeAsset box;
    [SerializeField] VisualTreeAsset requiredPickFrom;

    public List<object> history = new List<object>();
    public int currentHistoryIndex = 0;

    protected void Start()
    {
        if (I == null)
        {
            I = this;
            UIManager.OnUiChange += OnUiChange;
        }
    }

    protected void OnUiChange() 
    {
        UIManager.TransferToNewUI(I.ITEMVIEWERASSET.CloneTree().Q<VisualElement>("ItemViewerParent"), "ItemViewerParent");
        I.uidoc = UIManager.ui;

        I.root = I.uidoc.rootVisualElement;
        UIManager.MakeDraggable(I.root.Q<VisualElement>("ItemViewerParent"), I.root.Q<VisualElement>("ItemViewerParent").Q<VisualElement>("TitleBar"));
        I.root.Q<VisualElement>("ItemViewerParent").style.visibility = Visibility.Hidden;
        I.root.Q<VisualElement>("ItemViewerParent").style.display = DisplayStyle.None;

        I.root.Q<Button>("IVCloseButton").clicked += delegate 
        {
            I.root.Q<VisualElement>("ItemViewerParent").style.visibility = Visibility.Hidden;
            I.root.Q<VisualElement>("ItemViewerParent").style.display = DisplayStyle.None;
        };
        I.scrollViewBox = I.root.Q<VisualElement>("Contents");

        I.root.Q<Button>("BackButton").clicked += delegate 
        {
            if (I.history.Count > 0 && I.currentHistoryIndex-1 >= 0)
            {
                I.currentHistoryIndex--;
                I.DisplayItem(I.history[I.currentHistoryIndex]);
            }
        };
        I.root.Q<Button>("BackButton").clicked += delegate
        {
            if (I.currentHistoryIndex + 1 < I.history.Count)
            {
                I.currentHistoryIndex++;
                I.DisplayItem(I.history[I.currentHistoryIndex]);
            }
        };
    }

    // TOneverDO: this function is really poorly made with a lot of code reptition.
    public void DisplayItem(object item)
    {
        var t = item.GetType();
        if (t.BaseType != typeof(Item) && t.BaseType != typeof(Build))
            return;

        I.history.Add(item);

        I.root.Q<VisualElement>("ItemViewerParent").style.visibility = Visibility.Visible;
        I.root.Q<VisualElement>("ItemViewerParent").style.display = DisplayStyle.Flex;
        I.root.Q<VisualElement>("ItemViewerParent").BringToFront();
        const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;// | BindingFlags.FlattenHierarchy;
        I.scrollViewBox.Clear();
        //MemberInfo[] baseMembers = t.BaseType.GetFields(bindingFlags).Cast<MemberInfo>().Concat(t.BaseType.GetProperties(bindingFlags)).ToArray();

        I.root.Q<VisualElement>("Image").style.backgroundImage = XMLLoader.Loaders.LoadTex(item.GetType().GetProperty("SourceFile").GetValue(item).ToString());

        List<MemberInfo> members = t.GetFields(bindingFlags).Cast<MemberInfo>()
            .Concat(t.GetProperties(bindingFlags))
            .ToList();

        members.Reverse();
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
                
                prefab.Q<Label>("Label").text = $"<color=#99776A>{attrName}</color>: {GetValue(field, item)}";
                I.scrollViewBox.Add(prefab);
            }
            if(myAttributes[0].GetType() == typeof(XMLItemLink))
            {
                if (string.IsNullOrEmpty(GetValue(field, item).ToString()))
                    continue;
                var prefab = I.linkItem.CloneTree();
                var attrName = myAttributes[0].GetType().GetField("name").GetValue(myAttributes[0]).ToString();
                prefab.Q<Label>("Link").text = $"<color=#99776A>{attrName}:</color> <u>{GetValue(field, item)}</ui>";

                prefab.Q<Label>("Link").RegisterCallback<MouseDownEvent>(x => OnLinkClick(x, GetValue(field, item)));

                I.scrollViewBox.Add(prefab);
            }
            if(myAttributes[0].GetType() == typeof(XMLItemList))
            {             
                if (GetValue(field, item) == null)
                    continue;
                IList oTheList = GetValue(field, item) as IList;
                if (oTheList.Count == 0)
                    continue;
                
                var prefabBox = I.box.CloneTree();
                int index = 0;
                foreach(object o in oTheList)
                {
                    var prefab = I.singleItem.CloneTree();
                    string text = index != oTheList.Count - 1 ? $"{o}," : $"{o}";
                    prefab.Q<Label>("Label").text = text;
                    prefabBox.Q<ScrollView>("Contents").Add(prefab);
                    index++;
                }
                prefabBox.Q<Label>("MainLabel").text = $"<color=#99776A>{myAttributes[0].GetType().GetField("name").GetValue(myAttributes[0])}</color>";
                I.scrollViewBox.Add(prefabBox);
            }
            if(myAttributes[0].GetType() == typeof(XMLLinkList))
            {
                if (GetValue(field, item) == null)
                    continue;
                IList oTheList = GetValue(field, item) as IList;
                if (oTheList.Count == 0)
                    continue;

                var prefabBox = I.box.CloneTree();
                int index = 0;
                foreach (object o in oTheList)
                {
                    var prefab = I.linkItem.CloneTree();
                    string text = index != oTheList.Count - 1 ? $"<u>{o}</u><color=#F1E8C8>,</color>" : $"<u>{o}</u>";
                    prefab.Q<Label>("Link").text = text;
                    prefab.Q<Label>("Link").RegisterCallback<MouseDownEvent>(x => OnLinkClick(x, o));
                    prefabBox.Q<ScrollView>("Contents").Add(prefab);
                    index++;
                }
                prefabBox.Q<Label>("MainLabel").text = $"<color=#99776A>{myAttributes[0].GetType().GetField("name").GetValue(myAttributes[0])}</color>";
                I.scrollViewBox.Add(prefabBox);
            }
            if (myAttributes[0].GetType() == typeof(XMLRequiredPickFrom)) // please collapse this if in your IDE this is SHITTY
            {
                if (GetValue(field, item) == null)
                    continue;
                List<List<Armor>> oTheList = GetValue(field, item) as List<List<Armor>>;
                if (oTheList.Count == 0)
                    continue;

                var prefabBox = I.requiredPickFrom.CloneTree();
                if (oTheList.Count == 1)
                {
                    var s = prefabBox.Q<Label>("PickFromLabel");
                    s.parent.Remove(s);
                }
                int index = 0;
                string currentBox = "PickFromScrollBox";
                foreach (List<Armor> o in oTheList)
                {
                    if (index == oTheList.Count - 1) currentBox = "RequiredScrollBox";
                    int index2 = 0;
                    var prefabMain = new VisualElement();
                    prefabMain.style.paddingBottom = 5;
                    foreach (Armor o2 in o)
                    {
                        var prefab = I.linkItem.CloneTree();
                        string text = index2 != o.Count - 1 ? $"<u>{o2}</u><color=#F1E8C8>,</color>" : $"<u>{o2}</u>";
                        prefab.Q<Label>("Link").text = text;
                        prefab.Q<Label>("Link").RegisterCallback<MouseDownEvent>(x => OnLinkClick(x, o2));
                        prefabMain.Add(prefab);
                        index2++;
                    }
                    prefabBox.Q<ScrollView>(currentBox).Add(prefabMain);
                    index++;
                }
                prefabBox.Q<Label>("MainLabel").text = $"<color=#99776A>{myAttributes[0].GetType().GetField("name").GetValue(myAttributes[0])}</color>";
                I.scrollViewBox.Add(prefabBox);
            }
        }

    }

    public static void OnLinkClick(MouseDownEvent x, object displayItem)
    {
        if (x.button == Keybinds.LeftMouse)
            I.DisplayItem(displayItem);

        if (I.currentHistoryIndex < I.history.Count - 1)
        {
            I.currentHistoryIndex++;
            I.history.RemoveRange(I.currentHistoryIndex, I.history.Count - I.currentHistoryIndex);
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
