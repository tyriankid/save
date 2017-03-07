using UnityEngine;
using System.Collections;


public class LgSpawner : MonoBehaviour
{
    private Transform root;
    void Start()
    {
        root = transform.FindChild("Camera/Panel");
        UIDraggablePanel panel = root.GetComponent<UIDraggablePanel>();
        panel.Scroll(0);
    }

    void OnSelected()
    {
        int index = (int)((Mathf.Abs(root.localPosition.x - 50)) / 216 + 1);

        Hero h = new Hero();
        h.charactor = new RemoteChar();
        h.charactor.profession = (Config.Profession)index;        

        Global.LocalHero = h;

        Game.ChangeScene("CreateChar");
    }

    void OnClose()
    {
        Game.ChangeScene("SelectChar");
    }
}