using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public delegate void CompletedEventHandle(params object[] args);

public enum Result
{
    Ongoing,
    Completed,
    Faulted,
    Canceled,   
    Disconnect,
}

public static class ParseHelper
{
    private static Result result = Result.Ongoing;

    public static Result Result
    {
        get { return ParseHelper.result; }
        set { ParseHelper.result = value; }
    } 

    /// <summary>
    /// 添加
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="table"></param>
    /// <param name="callback"></param>
    /// <param name="pairs"></param>
    public static void SaveObject(string table, CompletedEventHandle callback, params KeyValuePair<string, object>[] pairs)
    {
        result = Result.Ongoing;
        try
        {
            Parse.ParseObject parseObj = new Parse.ParseObject(table);
            for (int i = 0; pairs != null && i < pairs.Length; i++)
            {
                parseObj[pairs[i].Key] = pairs[i].Value;
            }

            parseObj.SaveAsync().ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    Debug.LogWarning("SaveObject is faulted " + t.Exception);
                    result = Result.Faulted;
                    string error = t.Exception.ToString();
                    if (error.Contains("api.parse.com; Host not found"))
                    {
                        result = Result.Disconnect;
                    }

                    callback();
                }
                else if (t.IsCanceled)
                {
                    Debug.LogWarning("SaveObject is canceled");
                    result = Result.Canceled;
                    callback();
                }
                else
                {
                    result = Result.Completed;
                    callback(parseObj);
                }
            });
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning(ex.ToString());
            result = Result.Faulted;
            callback();
        }
    }

    public static void DeleteObject(string field, string key, string table, CompletedEventHandle callback)
    {
        result = Result.Ongoing;
        try
        {
            Parse.ParseQuery<Parse.ParseObject> query = Parse.ParseObject.GetQuery(table).WhereEqualTo(field, key);

            query.FirstAsync().ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    result = Result.Faulted;
                    string error = t.Exception.ToString();
                    if (error.Contains("api.parse.com; Host not found"))
                    {
                        result = Result.Disconnect;
                    }

                    callback(key);
                }
                else if (t.IsCanceled)
                {
                    result = Result.Canceled;
                    callback(key);
                }
                else
                {
                    Parse.ParseObject parseObj = t.Result;
                    parseObj.DeleteAsync().ContinueWith(d =>
                    {
                        if (t.IsFaulted || t.IsCanceled)
                        {
                            result = Result.Faulted;
                        }
                        else
                        {
                            result = Result.Completed;
                            callback(key);
                        }
                    });
                }
            });
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning(ex.ToString());
            result = Result.Faulted;
            callback(key);
        }
    }

    public static void ContainObject(string field, string key, string table, CompletedEventHandle callback)
    {
        result = Result.Ongoing;

        Parse.ParseQuery<Parse.ParseObject> query = Parse.ParseObject.GetQuery(table).WhereContains(field, key);
        query.FirstAsync().ContinueWith(t =>
        {
            if (t.IsFaulted)
            {
                result = Result.Faulted;
                string error = t.Exception.ToString();
                if (error.Contains("api.parse.com; Host not found"))
                {
                    result = Result.Disconnect;
                }

                callback();
            }
            else if (t.IsCanceled)
            {
                result = Result.Canceled;
                callback();
            }
            else
            {
                result = Result.Completed;
                callback();
            }
        });
    }

    public static void GetObjects(string field, object[] key, string table, CompletedEventHandle callback)
    {
        result = Result.Ongoing;

        Parse.ParseQuery<Parse.ParseObject> query = Parse.ParseObject.GetQuery(table).WhereContainedIn(field, key);
        query.FindAsync().ContinueWith(t =>
        {
            if (t.IsFaulted || t.IsCanceled)
            {
                result = Result.Faulted;
                string error = t.Exception.ToString();
                if (error.Contains("api.parse.com; Host not found"))
                {
                    result = Result.Disconnect;
                }

                callback();
            }
            else
            {
                result = Result.Completed;

                callback(t.Result.GetEnumerator());
            }
        });
    }

    /// <summary>
    /// 查询
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="table"></param>
    /// <param name="callback"></param>
    public static void GetObject<T>(string field, object key, string table,CompletedEventHandle callback, bool ignoreWarning = false) where T : Config.IData
    {
        result = Result.Ongoing;

        Parse.ParseQuery<Parse.ParseObject> query = Parse.ParseObject.GetQuery(table).WhereEqualTo(field, key);
        query.FirstAsync().ContinueWith(t =>
        {
            if (t.IsFaulted)
            {
                if (!ignoreWarning)
                    Debug.LogWarning("GetObject is faulted " + t.Exception);

                result = Result.Faulted;
                string error = t.Exception.ToString();
                if (error.Contains("api.parse.com; Host not found"))
                {
                    result = Result.Disconnect;
                }

                callback(key);
            }
            else if (t.IsCanceled)
            {
                if (!ignoreWarning)
                    Debug.LogWarning("GetObject is canceled");

                result = Result.Canceled;
                callback(key);
            }
            else
            {
                Parse.ParseObject parseObj = t.Result;
                T c = LitJson.JsonMapper.ToObject<T>(parseObj.Get<string>("object"));

                result = Result.Completed;
                callback(c, parseObj);
            }
        });
    }

    /// <summary>
    /// 更新
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="newObj"></param>
    /// <param name="table"></param>
    /// <param name="callback"></param>
    public static void UpdateObject<T>(object key, Config.IData newObj, string table,CompletedEventHandle callback) where T : Config.IData
    {
        result = Result.Ongoing;

        Parse.ParseQuery<Parse.ParseObject> query = Parse.ParseObject.GetQuery(table).WhereEqualTo("ID", key);
        query.FirstAsync().ContinueWith(t =>
        {
            if (t.IsFaulted || t.IsCanceled)
            {
                Debug.LogWarning("UpdateObject is faulted " + t.Exception);
                result = Result.Faulted;
                callback();
            }
            else
            {
                Parse.ParseObject parseObj = t.Result;

                string value = LitJson.JsonMapper.ToJson(newObj);
                parseObj["object"] = value;

                parseObj.SaveAsync().ContinueWith(s =>
                {
                    if (s.IsFaulted)
                    {
                        Debug.LogWarning("UpdateObject is faulted " + s.Exception);
                        result = Result.Faulted;
                        callback();
                    }
                    else if (s.IsCanceled)
                    {
                        Debug.LogWarning("UpdateObject is canceled");
                        result = Result.Canceled;
                        callback();
                    }
                    else
                    {
                        result = Result.Completed;
                        callback(newObj);
                    }
                });
            }
        });
    }

    public static void CreateNewUser(string userName, string password, string email, CompletedEventHandle callback)
	{
        result = Result.Ongoing;

        Parse.ParseUser user = new Parse.ParseUser()
		{
		    Username = userName,
		    Password = password,
            Email = email,
		};
		
		user.SignUpAsync().ContinueWith(s =>
        {
            if (s.IsFaulted)
            {                
                result = Result.Faulted;
                string error = s.Exception.ToString();
                if (error.Contains("api.parse.com; Host not found"))
                {
                    result = Result.Disconnect;
                }
            }
            else if (s.IsCanceled)
            {
                Debug.LogWarning("CreateNewUser is canceled");
                result = Result.Canceled;
            }
            else
            {
                result = Result.Completed;
            }

            callback();
        });		
	}

    public static void AuthenticateUser(string username, string password,CompletedEventHandle callback)
    {
        result = Result.Ongoing;

        Parse.ParseUser.LogInAsync(username, password).ContinueWith(t =>
        {
            if (t.IsFaulted)
            {
                result = Result.Faulted;
                string error = t.Exception.ToString();
                if (error.Contains("api.parse.com; Host not found"))                
                {
                    result = Result.Disconnect;
                }
                 
                callback();            
            }
            else if (t.IsCanceled)
            {
                Debug.LogWarning("AuthenticateUser is canceled");

                result = Result.Canceled;
                callback();                
            }
            else
            {
                result = Result.Completed;

                Parse.ParseObject parseObj = t.Result;
                IList<string> roles = parseObj.Get<IList<string>>("roles");                
                callback(roles);
            }
        });
    }

    public static void AddRoleToUser(string roleName, CompletedEventHandle callback)
    {
        result = Result.Ongoing;

        Parse.ParseUser.CurrentUser.AddToList("roles", roleName);
        Parse.ParseUser.CurrentUser.SaveAsync().ContinueWith(t =>
        {
            if (t.IsFaulted)
            {
                result = Result.Faulted;
                string error = t.Exception.ToString();
                if (error.Contains("api.parse.com; Host not found"))
                {
                    result = Result.Disconnect;
                }
            }
            else if (t.IsCanceled)
            {
                result = Result.Canceled;
                Debug.LogWarning("AddRoleToUser is canceled");
            }
            else
            {
                result = Result.Completed;
            }

            callback();
        });
    }


}
