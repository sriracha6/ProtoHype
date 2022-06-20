using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class KeyboardShortcuts : MonoBehaviour
{
    void Update()
    {
        if(Input.GetKeyDown(Keybinds.clearTileSelection))
        {
            Player.selectedTileBounds.Clear();
            Player.selectedTilePoses.Clear();
        }
        if(Input.GetKeyDown(Keybinds.clearPawnSelection))
        {
            Player.ourSelectedPawns.Clear();
            Player.selectedPawns.Clear();
        }

        if(Input.GetKeyDown(Keybinds.hideUI))
        {
            if(!Keybinds.UIhidden)
            {
                Keybinds.UIhidden = true;
                UIManager.ui.rootVisualElement.style.display = DisplayStyle.None;
            }
            else
            {
                UIManager.ui.rootVisualElement.style.display = DisplayStyle.Flex;
                Keybinds.UIhidden = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.Keypad0))
            PopulateRegiments.selectRegiment(0);
        if (Input.GetKeyDown(KeyCode.Keypad1))
            PopulateRegiments.selectRegiment(1);
        if (Input.GetKeyDown(KeyCode.Keypad2))
            PopulateRegiments.selectRegiment(2);
        if (Input.GetKeyDown(KeyCode.Keypad3))
            PopulateRegiments.selectRegiment(3);
        if (Input.GetKeyDown(KeyCode.Keypad4))
            PopulateRegiments.selectRegiment(4);
        if (Input.GetKeyDown(KeyCode.Keypad5))
            PopulateRegiments.selectRegiment(5);
        if (Input.GetKeyDown(KeyCode.Keypad6))
            PopulateRegiments.selectRegiment(6);
        if (Input.GetKeyDown(KeyCode.Keypad7))
            PopulateRegiments.selectRegiment(7);
        if (Input.GetKeyDown(KeyCode.Keypad8))
            PopulateRegiments.selectRegiment(8);
        if (Input.GetKeyDown(KeyCode.Keypad9))
            PopulateRegiments.selectRegiment(9);

        if (Input.GetKeyDown(Keybinds.rp_inc))
            UIManager.ui.rootVisualElement.Q<VisualElement>("RegimentControlStuff").Q<SliderInt>("Slider").value += Keybinds.sliderStep;
        if (Input.GetKeyDown(Keybinds.rp_dec))
            UIManager.ui.rootVisualElement.Q<VisualElement>("RegimentControlStuff").Q<SliderInt>("Slider").value -= Keybinds.sliderStep;
    }
}
