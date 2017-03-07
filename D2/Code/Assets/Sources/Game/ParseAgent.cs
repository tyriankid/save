using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ParseAgent : MonoBehaviour
{
    public string uniqueId;
    public string password;
    public string phoneNum = "game4d";

    private List<System.Action> events = new List<System.Action>();

    public static ParseAgent handle;

	// Use this for initialization
	void Start () 
    {
        handle = this;
	}
	
    public void Login()
    {
        LgLoading.Show("正在连接服务器");

        if (Application.platform == RuntimePlatform.Android)
        {
            uniqueId = "ARD_" + SystemInfo.deviceUniqueIdentifier;
            phoneNum = AndroidNativePlugin.getPhoneNumber();
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
            uniqueId = "IPP_" + SystemInfo.deviceUniqueIdentifier;
        else if (Application.platform == RuntimePlatform.WP8Player)
            uniqueId = "WP8_" + SystemInfo.deviceUniqueIdentifier;
        else
            uniqueId = "TST_" + SystemInfo.deviceUniqueIdentifier;

        password = "1";

        ParseHelper.AuthenticateUser(uniqueId, password, OnCompletedLogin);
    }

	// Update is called once per frame
    void Update()
    {
        if (events.Count > 0)
        {
            lock (events)
            {
                for (int i = 0; i < events.Count; i++)
                    events[i].Invoke();

                events.Clear();
            }
        }

        //if (ParseHelper.Result == Result.Ongoing)
        //    return;
    }

    void OnCompletedLogin(params object[] args)
    {
        if (ParseHelper.Result == Result.Completed)
        {
            lock (events)
            {
                events.Add(() => LgLoading.Show("正在获取角色列表"));
            }

            IList<string> t = args[0] as IList<string>;

            string[] roles = new string[t.Count];
            t.CopyTo(roles, 0);

            ParseHelper.GetObjects("name", roles, "Roles", OnCompletedFindRoles);            
        }
        else if (ParseHelper.Result == Result.Disconnect)
        {
            lock (events)
            {
                events.Add(() =>
                {
                    LgLoading.Hide();
                    LgMessageBox.Show("服务器连接失败", UIWidget.Pivot.Center, null);
                });
            }
        }
        else
        {
            lock (events)
            {
                events.Add(() =>
                {
                    LgLoading.Show("首次登陆，注册当前设备");
                    ParseHelper.CreateNewUser(uniqueId, password, phoneNum+"@163.com", OnCompletedRegist);
                });
            }
        }
    }

    void OnCompletedFindRoles(params object[] args)
    {
        if (ParseHelper.Result == Result.Completed)
        {
            Global.MyHeros.Clear();

            try
            {
                IEnumerator<Parse.ParseObject> it = args[0] as IEnumerator<Parse.ParseObject>;
                while (it.MoveNext())
                {
                    Parse.ParseObject o = (Parse.ParseObject)it.Current;

                    Global.MyHeros.Add(new Hero(o));
                }
            }
            catch (System.Exception ex)
            {
                Debug.Log("role struct error! " + ex.ToString());	
            }

            lock (events)
            {
                events.Add(() =>
                {
                    LgLoading.Hide();
                    Game.ChangeScene("SelectChar");
                });                
            }
        }
        else
        {
            lock (events)
            {
                events.Add(() =>
                {
                    LgLoading.Hide();
                    LgMessageBox.Show("服务器连接超时", UIWidget.Pivot.Center, null);
                });
            }
        }
    }

    void OnCompletedRegist(params object[] args)
    {
        if (ParseHelper.Result != Result.Completed)
        {
            lock (events)
            {
                events.Add(() =>
                {
                    LgLoading.Hide();
                    LgMessageBox.Show("服务器连接超时", UIWidget.Pivot.Center, null);
                });
            }
        }
        else
        {
            lock (events)
            {
                events.Add(() => Game.ChangeScene("SelectChar"));
            }
        }
    }

    private static RemoteChar pendingChar;
    public void CreateRole(string name, Config.Profession profession)
    {
        LgLoading.Show("正在验证角色名【"+name+"】是否被占用");
        pendingChar = new RemoteChar();
        pendingChar.name = name;
        pendingChar.profession = profession;

        ParseHelper.ContainObject("name", name, "Roles", OnCompletedQueryRoleForCreate);
    }

    void OnCompletedQueryRoleForCreate(params object[] args)
    {
        if (ParseHelper.Result != Result.Completed && pendingChar != null)
        {
            // 获得排名
            ParseHelper.Result = Result.Ongoing;
            Parse.ParseQuery<Parse.ParseObject> query = Parse.ParseObject.GetQuery("Roles");
            query.CountAsync().ContinueWith(t =>
            {
                if (t.IsFaulted || t.IsCanceled)
                {
                    ParseHelper.Result = Result.Faulted;
                    lock (events)
                    {
                        events.Add(() =>
                        {
                            LgLoading.Hide();
                            LgMessageBox.Show("创建角色请求超时", UIWidget.Pivot.Center, null);
                        });
                    }
                }
                else
                {
                    ParseHelper.Result = Result.Completed;

                    lock (events)
                    {
                        events.Add(() =>
                        {
                            LgLoading.Show("创建角色中...");
                            int rank = t.Result;

                            Global.LocalHero.charactor = pendingChar;
                            Global.LocalHero.charactor.rank = rank;
                            Global.LocalHero.charactor.user = Parse.ParseUser.CurrentUser.Username;
                            Global.LocalHero.charactor.backpack.Add(1563);
                            Global.LocalHero.SaveAsyc(OnCompletedCreateRole);
                        });
                    }
                }
            });
        }
        else
        {
            lock (events)
            {
                events.Add(() =>
                {
                    LgLoading.Hide();
                    LgMessageBox.Show("该角色名[" + pendingChar.name + "]已经存在", UIWidget.Pivot.Center, null);

                    pendingChar = null;
                });
            }
        }        
    }

    void OnCompletedCreateRole(params object[] args)
    {        
        if (ParseHelper.Result == Result.Completed)
        {
            Global.LocalHero = new Hero(args[0] as Parse.ParseObject);
            // 加入英雄列表
            Global.MyHeros.Add(Global.LocalHero);

            lock (events)
            {
                events.Add(() =>
                {
                    LgLoading.Show("填充角色信息");                    

                    ParseHelper.AddRoleToUser(Global.LocalHero.charactor.name, OnCompletedAddRole);
                });
            }
        }
        else
        {
            lock (events)
            {
                events.Add(() =>
                {
                    LgLoading.Hide();
                    LgMessageBox.Show("由于网络原因，角色创建失败", UIWidget.Pivot.Center, null);
                });
            }
        }
        pendingChar = null;
    }

    void OnCompletedAddRole(params object[] args)
    {
        lock (events)
        {
            events.Add(() => LgLoading.Hide());

            if (ParseHelper.Result == Result.Completed)
            {                
                events.Add(() => Game.ChangeScene("Main"));
            }
            else
            {
                events.Add(() => LgMessageBox.Show("由于网络原因，角色创建失败", UIWidget.Pivot.Center, null));
            }
        }
    }

    public void DeleteRole(string name)
    {
        LgLoading.Show("正在删除角色 【"+ name + "】");
        // 非真删除
        //ParseHelper.DeleteObject("name", name, "Roles", OnCompletedDelRole);
        OnCompletedDelRole();
    }

    void OnCompletedDelRole(params object[] args)
    {       
        //if (ParseHelper.Result == Result.Completed)
        {
            ParseHelper.Result = Result.Ongoing;
            Parse.ParseUser.CurrentUser.RemoveAllFromList("roles", args[0].ToString());
            Parse.ParseUser.CurrentUser.SaveAsync().ContinueWith(t =>
            {
                if (t.IsFaulted || t.IsCanceled)
                {
                    ParseHelper.Result = Result.Faulted;
                    string error = t.Exception.ToString();
                    if (error.Contains("api.parse.com; Host not found"))
                    {
                        ParseHelper.Result = Result.Disconnect;                        
                    }

                    lock (events)
                    {
                        LgLoading.Hide();
                        events.Add(() => LgMessageBox.Show("删除角色请求超时", UIWidget.Pivot.Center, null));
                    }
                }
                else
                {
                    ParseHelper.Result = Result.Completed;
                    lock (events)
                    {
                        events.Add(() =>LgLoading.Hide());
                    }
                }
            });
        }
    }

//     public void QueryRole(string name)
//     {
//         ParseHelper.GetObject<RemoteChar>("name", name, "Roles", OnCompletedQueryRole, true);
//     }
// 
//     void OnCompletedQueryRole(params object[] args)
//     {
//         if (ParseHelper.Result != Result.Completed)
//         {
//             Debug.Log("该角色不存在 " + args[0]);
//         }
//         else
//         {            
//             Global.LocalHero.charactor = args[0] as RemoteChar;
//             Global.LocalHero.obj = args[1] as Parse.ParseObject;
//             Global.LocalHero.charactor.rank = Global.LocalHero.obj.Get<int>("rank");
// 
//             Debug.Log("query role successfull");
//         }
//     }

    public void UpdateRoleProperty(Parse.ParseObject obj, string scene, bool background)
    {
        if (!background)
            LgLoading.Show("更新角色属性");

        ParseHelper.Result = Result.Ongoing;

        obj.SaveAsync().ContinueWith(s =>
        {
            if (s.IsFaulted || s.IsCanceled)
            {
                ParseHelper.Result = Result.Faulted;

                if (!background)
                {
                    lock (events)
                    {
                        events.Add(() =>
                        {
                            LgLoading.Hide();
                            LgMessageBox.Show("由于网络原因，角色属性更新失败", UIWidget.Pivot.Center, null);
                        });
                    }
                }
            }
            else
            {
                ParseHelper.Result = Result.Completed;
                //
                lock (events)
                {
                    events.Add(() =>
                    {
                        if (!string.IsNullOrEmpty(scene))
                            Game.ChangeScene(scene);
                        else if (!background)
                            LgLoading.Hide();
                    });
                }
            }
        });
    }

    public void ReflushRank()
    {
        ParseHelper.Result = Result.Ongoing;
        LgLoading.Show("获取排行榜信息");
        Parse.ParseQuery<Parse.ParseObject> query = Parse.ParseObject.GetQuery("Roles").WhereLessThan("rank", Global.LocalHero.charactor.rank).WhereGreaterThan("rank", Global.LocalHero.charactor.rank - 5);

        query.FindAsync().ContinueWith(t =>
        {
            if (t.IsFaulted || t.IsCanceled)
            {
                ParseHelper.Result = Result.Faulted;
                lock (events)
                {
                    events.Add(() =>
                    {
                        LgLoading.Hide();
                        LgMessageBox.Show("由于网络原因，排行榜信息获得失败", UIWidget.Pivot.Center, null);
                    });
                }
            }            
            else
            {
                ParseHelper.Result = Result.Completed;

                Global.RankHeros.Clear();

                IEnumerator it = t.Result.GetEnumerator();
                while(it.MoveNext())
                {
                    Parse.ParseObject o = it.Current as Parse.ParseObject;

                    Global.RankHeros.Add(new Hero(o));
                }

                lock (events)
                {
                    events.Add(() => Game.ChangeScene("Rank"));
                }
            }
        });
    }

    void OnApplicationQuit() 
    {
        Parse.ParseUser.LogOut();
    }

    public static void StorageToLocal()
    {
        List<RemoteChar> chars = new List<RemoteChar>();
        for (int i = 0; i < Global.MyHeros.Count; i++)
            chars.Add(Global.MyHeros[i].charactor);
        PlayerPrefs.SetString("MyHeros", LitJson.JsonMapper.ToJson(chars));
    }
}
