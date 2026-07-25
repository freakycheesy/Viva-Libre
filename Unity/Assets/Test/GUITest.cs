using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUITest : MonoBehaviour
{
    public GUISkin skin;
    public Texture2D logo;
    public int i = 0;
    public string[] elements;
    void Start()
    {
        elements = new string[] { "Option 1: Fly Hacks", "Option 2: Game Crasher", "Option 3: Game Dumper", "Option 4: crossplay", "Option 5: crossplatform", "Option 6: 7", "Option 7: minutes", "Option 8: 9", "Option 9: years old = superdojo9"};
    }
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.S))
        {
            i++;
            if (i >= elements.Length) i = 0;
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            i--;
            if (i < 0) i = elements.Length - 1;
        }
    }
    void OnGUI()
    {
        GUI.skin = skin;
        GUILayout.Box(logo, "logo");
        GUILayout.Box("Mod Menu for Testing");
        GUILayout.BeginVertical("hover");
        for (int j = 0; j < elements.Length; j++)
        {
            if (j == i) GUILayout.Button($">>Element {elements[j]}<<", "hover");
            
            else GUILayout.Button($"Element {elements[j]}");
        }
        GUILayout.EndVertical();
    }
}
