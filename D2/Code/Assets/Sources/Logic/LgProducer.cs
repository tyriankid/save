using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class LgProducer : MonoBehaviour
{
    public TextAsset textAsset;
    public UILabel label;
    public float speed = 1;
    public float initDelay = 1;

    private string[] lines;
    private int index = 0;
    private List<UILabel> content = new List<UILabel>();
    private float delay = 1;

    void Start()
    {
        lines = textAsset.text.Split('\n');
        label.gameObject.SetActive(false);
    }

    void Update()
    {
        for (int i = 0; i < content.Count; i++)
        {
            content[i].transform.localPosition += new Vector3(0, speed * Time.deltaTime);

            if (content[i].transform.localPosition.y >= 225)
            {
                content[i].gameObject.SetActive(false);
            }
        }

        if (delay >= initDelay)
        {
            Transform t = label.transform.parent.FindChild("P_O_" + index);
            if (t == null)
            {
                GameObject o = GameObject.Instantiate(label.gameObject) as GameObject;
                o.name = "P_O_" + index;
                o.transform.parent = label.transform.parent;
                o.transform.localPosition = label.transform.localPosition;
                o.transform.localScale = Vector3.one;
                o.SetActive(true);
                UILabel txt = o.transform.GetComponent<UILabel>();
                txt.text = lines[index];
                content.Add(txt);
            }
            else
            {
                t.localPosition = label.transform.localPosition;
                t.gameObject.SetActive(true);
            }

            index++;
            if (index == lines.Length)
                index = 0;

            delay = 0;
        }

        delay += Time.deltaTime;
    }

    void OnCancel()
    {
        Game.ChangeScene("Login");
    }


}