using UnityEngine;
using System.Collections;


public class Login : Scene
{
    public override void Start()
    {
        uiRoot = GameObject.Instantiate(Resources.Load("Login_UI")) as GameObject;
        //AsyncTask.GetObject<Config.Char>("c1002", "char", OnQueryChar);  
    }

    //void OnQueryChar(Config.IData data)
    //{
    //    Config.Char c = data as Config.Char;

    //    Debug.Log("char = " + c.name);
    //}
}

public class SelectChar : Scene
{
    public override void Start()
    {
        uiRoot = GameObject.Instantiate(Resources.Load("SSelect_UI")) as GameObject;
    }
}

public class SpawnChar : Scene
{
    public override void Start()
    {
        uiRoot = GameObject.Instantiate(Resources.Load("CSelect_UI")) as GameObject;
    }
}

public class CreateChar : Scene
{
    public override void Start()
    {
        uiRoot = GameObject.Instantiate(Resources.Load("Create_UI")) as GameObject;
    }
}

public class Main : Scene
{
    public override void Start()
    {
        uiRoot = GameObject.Instantiate(Resources.Load("Main_UI")) as GameObject;
    }
}

public class Skill : Scene
{
    public override void Start()
    {
        uiRoot = GameObject.Instantiate(Resources.Load("Skill_UI")) as GameObject;
    }
}

public class CharProp : Scene
{
    public override void Start()
    {
        uiRoot = GameObject.Instantiate(Resources.Load("Char_UI")) as GameObject;
    }
}

public class OtherProp : Scene
{
    public override void Start()
    {
        uiRoot = GameObject.Instantiate(Resources.Load("Other_UI")) as GameObject;
    }
}

public class GameCopy : Scene
{
    public override void Start()
    {
        uiRoot = GameObject.Instantiate(Resources.Load("Copy_UI")) as GameObject;
    }
}

public class Rank : Scene
{
    public override void Start()
    {
        uiRoot = GameObject.Instantiate(Resources.Load("Rank_UI")) as GameObject;
    }
}

public class Task : Scene
{
    public override void Start()
    {
        uiRoot = GameObject.Instantiate(Resources.Load("Task_UI")) as GameObject;
    }
}

public class Battle : Scene
{
    public override void Start()
    {
        uiRoot = GameObject.Instantiate(Resources.Load("Fighting_UI")) as GameObject;
    }
}

public class Backpack : Scene
{
    public override void Start()
    {
        uiRoot = GameObject.Instantiate(Resources.Load("Backpack_UI")) as GameObject;
    }
}

public class Depot : Scene
{
    public override void Start()
    {
        uiRoot = GameObject.Instantiate(Resources.Load("Depot_UI")) as GameObject;
    }
}

public class DarePvP : Scene
{
    public override void Start()
    {
        uiRoot = GameObject.Instantiate(Resources.Load("PvP_UI")) as GameObject;
    }
}

public class Producer : Scene
{
    public override void Start()
    {
        uiRoot = GameObject.Instantiate(Resources.Load("Producer")) as GameObject;
    }
}

public class Chest : Scene
{
    public override void Start()
    {
        uiRoot = GameObject.Instantiate(Resources.Load("Chest_UI")) as GameObject;
    }
}