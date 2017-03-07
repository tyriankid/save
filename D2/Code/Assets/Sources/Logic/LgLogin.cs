using UnityEngine;
using System.Collections;


public class LgLogin : MonoBehaviour
{
    void Start()
    {

    }

    void OnNetEnter()
    {
        Global.SolePlayerMode = false;

        ParseAgent.handle.Login();        
    }

    void OnSoleEnter()
    {
        Global.SolePlayerMode = true;
        Game.ChangeScene("SelectChar");
    }

    void OnExit()
    {
        Application.Quit();
    }

    void OnAbout()
    {
        LgAbout.Show();//"制作人:Ben\n\n感谢伟大的暴雪，以此纪念我们逝去的青春。", UIWidget.Pivot.TopLeft, false);
    }

    void OnProduce()
    {
        Game.ChangeScene("Producer");
    }
}