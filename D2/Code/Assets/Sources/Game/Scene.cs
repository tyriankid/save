using UnityEngine;
using System.Collections;

public class Scene 
{
    protected GameObject uiRoot;

	// Use this for initialization
    public virtual void Start()
    {        
        //try
        //{
        //    Config.Char charactor = new Config.Char();
        //    charactor.ID = 2;
        //    charactor.name = "啦啦啦啦";
        //    charactor.level = 4;

        //    AsyncTask.SaveObject(charactor, "char", OnInsert);
        //}
        //catch (System.Exception ex)
        //{
        //    Debug.LogException(ex);
        //}
    }
	
	// Update is called once per frame
    public virtual void Update()
    {

    }

    public virtual void Destroy()
    {
        NGUITools.DestroyImmediate(uiRoot);
    }

    //public virtual void OnGUI()
    //{

    //}

    //void OnInsert(Config.IData data, uint id)
    //{
    //    Debug.Log("id = " + id);
    //}
}
