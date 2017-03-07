using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Game : MonoBehaviour
{
    private Dictionary<string, Scene> scenes = new Dictionary<string, Scene>();
    private string activeScene = "";
    private AudioSource backgroundAudio;

    public AudioClip startAudio;
    public AudioClip lobbyAudio;
    public AudioClip battleAudio;

    public bool mute = false;

    public static Game handle;

    // Use this for initialization
    void Start()
    {
        handle = this;

        if (PlayerPrefs.HasKey("SoundVolume"))
            NGUITools.soundVolume = PlayerPrefs.GetFloat("SoundVolume");
        else
            NGUITools.soundVolume = mute ? 0 : 1;

        Config.DataLoader.Init();

        //////////////////////////////////////////////////////////////////////////
        // regist scene
        RegistWindow<Login>();
        RegistWindow<Main>();
        RegistWindow<SelectChar>();
        RegistWindow<SpawnChar>();
        RegistWindow<CreateChar>();
        RegistWindow<Skill>();
        RegistWindow<CharProp>();
        RegistWindow<GameCopy>();
        RegistWindow<Rank>();
        RegistWindow<Task>();
        RegistWindow<Battle>();
        RegistWindow<Backpack>();
        RegistWindow<Depot>();
        RegistWindow<DarePvP>();
        RegistWindow<OtherProp>();
        RegistWindow<Producer>();
        RegistWindow<Chest>();
        //////////////////////////////////////////////////////////////////////////               
        //Debug.Log(Application.internetReachability);
        
        ChangeScene("Login",0, false);

        StartCoroutine(PlaySound<Login>());
    }

    public static IEnumerator PlaySound<T>() where T : Scene
    {
        if (handle.backgroundAudio != null)
            GameObject.DestroyImmediate(handle.backgroundAudio.gameObject);
        else
        {
            yield return new WaitForSeconds(0.1f);
            handle.backgroundAudio = NGUITools.PlaySound(handle.startAudio, false);
            yield return new WaitForSeconds(4);
        }

        if (typeof(T) == typeof(Battle))
            handle.backgroundAudio = NGUITools.PlaySound(handle.battleAudio, true);
        else
            handle.backgroundAudio = NGUITools.PlaySound(handle.lobbyAudio, true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Menu))
        {
            if (LgSystemSetting.Visible)
                LgSystemSetting.Hide();
            else
                LgSystemSetting.Show();
        }

        if (!Global.SolePlayerMode && ParseHelper.Result == Result.Ongoing)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (LgMessageBox.Visible)
                LgMessageBox.Hide();
            else if (LgSystemSetting.Visible)
                LgSystemSetting.Hide();
            else if (LgAbout.Visible)
                LgAbout.Hide();
            else if (activeScene == "Main")
            {
                ChangeScene("SelectChar");
            }
            else if (activeScene == "Depot" || activeScene == "Backpack" ||
                activeScene == "Task" || activeScene == "Rank" ||
                activeScene == "CharProp" || activeScene == "GameCopy")
            {
                ChangeScene("Main");
            }
            else if (activeScene == "Skill")
            {
                if (LgSkill.handle != null)
                    LgSkill.handle.OnClose();
            }
            else if (activeScene == "Battle")
            {
                ChangeScene("GameCopy");
            }
            else if (activeScene == "SelectChar" || activeScene == "Producer")
            {
                ChangeScene("Login");
            }
            else if (activeScene == "Login")
            {
                Application.Quit();
            }
            else if (activeScene == "CreateChar")
            {
                ChangeScene("SpawnChar");
            }
            else if (activeScene == "SpawnChar")
            {
                ChangeScene("SelectChar");
            }
            else if (activeScene == "DarePvP" || activeScene == "OtherProp")
            {
                ChangeScene("Rank");
            }
        }
    }

    public static void ChangeScene(string newScene, float time = 0, bool loading = true)
    {
        handle.StartCoroutine(SyncChangeScene(newScene, time, loading));
    }

    private static IEnumerator SyncChangeScene(string newScene, float time = 0, bool loading = true)
    {
        if (time > 0)
            yield return new WaitForSeconds(time);

        if (loading)
            LgLoading.Show();

        if (handle.scenes.ContainsKey(handle.activeScene))
        {
            handle.scenes[handle.activeScene].Destroy();
            // 保证所有静态引用设置为null
            Resources.UnloadUnusedAssets();
            // 或者
            //Resources.UnloadAsset(handle.scenes[handle.activeScene].gameObject);
            //
            System.GC.Collect();
        }

        handle.activeScene = newScene;
        handle.scenes[newScene].Start();

        if (loading)
        {
            yield return new WaitForSeconds(0.3f);
            // close loading
            LgLoading.Hide();
        }

        yield return 0;
    }

    void OnDestroy()
    {
        if (scenes.ContainsKey(activeScene))
            scenes[activeScene].Destroy();
    }

    void RegistWindow<T>() where T : Scene, new()
    {
        T t = new T();
        scenes.Add(t.GetType().Name, t);
    }

    // 获得当前场景
    public static T CurrentScene<T>() where T : Scene
    {
        return handle.scenes[handle.activeScene] as T;
    }

    public static string ActiveScene
    {
        get
        {
            return handle.activeScene;
        }
    }
}
