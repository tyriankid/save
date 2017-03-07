using System;
using System.Collections.Generic;
using UnityEngine;


public class UnitTest : MonoBehaviour
{
    void Start()
    {
        Config.DataLoader.Init();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            OnEquipmentTest();
        }
    }

    void OnEquipmentTest()
    {
        Transform t = transform.FindChild("Camera/Anchor/Panel/Sprite");

        foreach (Config.Equipment eq in Config.DataLoader.equipmentMaps.Values)
        {
            UISprite sp = t.GetComponent<UISprite>();
            sp.spriteName = eq.icon;

            if (sp.GetAtlasSprite() == null)
            {
                Debug.LogWarning("id " + eq.ID + " " + eq.icon);
            }
        }

        Debug.Log("equipment finish");
    }
}

