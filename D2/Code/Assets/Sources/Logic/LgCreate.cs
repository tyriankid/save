using UnityEngine;
using System.Collections;


public class LgCreate : MonoBehaviour
{
    void Start()
    {
        Transform root = transform.FindChild("Camera/Anchor/Panel/A" + (int)Global.LocalHero.charactor.profession);
        root.gameObject.SetActive(true);
    }

    void OnCreate()
    {
        Transform root = transform.FindChild("Camera/Anchor/Panel/Input");
        UIInput title = root.GetComponent<UIInput>();
        if (string.IsNullOrEmpty(title.value) || title.value == title.defaultText)
        {
            title.value = title.defaultText;
            title.activeTextColor = Color.red;
            return;
        }
        
        if (Global.SolePlayerMode)
        {
            Global.LocalHero.charactor.name = title.value;
            Global.LocalHero.charactor.backpack.Add(1563);

            Global.MyHeros.Add(Global.LocalHero);
            
            ParseAgent.StorageToLocal();

            Game.ChangeScene("Main");
        }
        else
        {
            ParseAgent.handle.CreateRole(title.value, Global.LocalHero.charactor.profession);
        }
    }

    void OnCancel()
    {
        Game.ChangeScene("SpawnChar");
    }
}