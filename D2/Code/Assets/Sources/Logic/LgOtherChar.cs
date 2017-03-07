using UnityEngine;
using System.Collections;


public class LgOtherChar : MonoBehaviour
{
    private GameObject selectedTile;


    void Start()
    {
        LgBackpack.InitUIComponent(Global.OtherHero.charactor, transform);
    }

    void OnClose()
    {
        Game.ChangeScene("Rank");
    }


    void OnClickTip(GameObject obj)
    {
        UserData ud = obj.GetComponent<UserData>();
        if (ud == null)
            return;

        ushort eid = ushort.Parse(ud.data);
        if (!Config.DataLoader.equipmentMaps.ContainsKey(eid))
            return;

        if (selectedTile != obj)
        {
            UITooltip.ShowText(Global.CombEquipmentTips(Global.OtherHero.charactor, eid, true, false));

            Transform root = transform.FindChild("Camera/Anchor/Panel/Tooltip_UI");
            UISprite bg = root.GetComponent<UISprite>();

            UILabel lab = root.FindChild("Label").GetComponent<UILabel>();
            lab.transform.localPosition = new Vector2(0, -6);
            bg.width = Mathf.Max(bg.width + 8, 100);
            bg.height += 16;

            float dw = root.localPosition.x + bg.width - Screen.width / 2;
            float dh = Mathf.Abs(root.localPosition.y - bg.height) - Screen.height / 2;
            if (dh > 0)
                dh += root.localPosition.y;
            else
                dh = root.localPosition.y;

            if (dw > 0)
                dw = root.localPosition.x + bg.width / 2 - dw;
            else
                dw = root.localPosition.x + bg.width / 2;

            root.localPosition = new Vector3(dw, dh);

            selectedTile = obj;
        }
    }

    void OnCancelTip()
    {
        UITooltip.ShowText(null);
        selectedTile = null;
    }
}