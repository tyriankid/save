using System;
using System.Collections.Generic;
using UnityEngine;


namespace Config
{

    public static class DataLoader
    {
        public static readonly Dictionary<ushort, Equipment> equipmentMaps = new Dictionary<ushort,  Equipment>();
        public static readonly Dictionary<ushort, Char> monsterMaps = new Dictionary<ushort, Char>();
        public static readonly Dictionary<ushort, Sets> setsMaps = new Dictionary<ushort, Sets>();

        public static readonly List<Scene> sceneMaps = new List<Scene>();
        public static readonly List<Task> taskMaps = new List<Task>();

        private static bool loaded = false;

        // 初始化
        public static void Init()
        {
            if (!loaded)
            {
                List<Char> chars = LoadFile<Char>("monster");
                for (int i = 0; i < chars.Count; i++)
                    monsterMaps.Add(chars[i].ID, chars[i]);

                List<Equipment> equipment = LoadFile<Equipment>("equipment");
                for (int i = 0; i < equipment.Count; i++)
                {
                    //try
                    //{
                        equipmentMaps.Add(equipment[i].ID, equipment[i]);
                    //}
                    //catch (Exception e)
                    //{
                    //    Debug.Log("add " + equipment[i].ID);
                    //}
                }

                List<Sets> sets = LoadFile<Sets>("sets");
                for (int i = 0; i < sets.Count; i++)
                {
                    setsMaps.Add(sets[i].ID, sets[i]);
                }

                // 加载场景信息
                sceneMaps.AddRange(LoadFile<Scene>("scenes"));
                // 加载任务信息
                taskMaps.AddRange(LoadFile<Task>("tasks")); 
            }
        }

        static List<T> LoadFile<T>(string file) where T : IData, new()
        {
            List<T> map = new List<T>();

            TextAsset text = Resources.Load(file) as TextAsset;
            if (text == null)
            {
                Debug.LogWarning("not find config " + file);
                return map;
            }

            string[] lines = text.text.Split('\r', '\n');
            for (int i = 0, space = 0; i < lines.Length; i++)
            {
                if (string.IsNullOrEmpty(lines[i]))
                {
                    space++;
                    continue;
                }

                if (i < 2 + space)
                    continue;

                T t = new T();
                System.Reflection.FieldInfo[] fields = t.GetType().GetFields();

                string[] attrs = lines[i].Split(',');
                for (int j = 0; j < attrs.Length; j++)
                {
                    try
                    {
                        object obj = fields[j].GetValue(t);
                        if (obj.GetType() == typeof(int))
                        {
                            int n = string.IsNullOrEmpty(attrs[j]) ? 0 : int.Parse(attrs[j]);
                            fields[j].SetValue(t, n);
                        }
                        else if (obj.GetType() == typeof(uint))
                        {
                            uint n = string.IsNullOrEmpty(attrs[j]) ? 0 : uint.Parse(attrs[j]);
                            fields[j].SetValue(t, n);
                        }
                        else if (obj.GetType() == typeof(ushort))
                        {
                            ushort n = string.IsNullOrEmpty(attrs[j]) ? (ushort)0 : ushort.Parse(attrs[j]);
                            fields[j].SetValue(t, n);
                        }
                        else if (obj.GetType() == typeof(bool))
                        {
                            fields[j].SetValue(t, bool.Parse(attrs[j]));
                        }
                        else if (obj.GetType() == typeof(byte))
                        {
                            byte n = string.IsNullOrEmpty(attrs[j]) ? (byte)0 : byte.Parse(attrs[j]);
                            fields[j].SetValue(t, n);
                        }
                        else if (obj.GetType() == typeof(string))
                        {
                            fields[j].SetValue(t, attrs[j]);
                        }
                        else if (obj.GetType() == typeof(long))
                        {
                            fields[j].SetValue(t, long.Parse(attrs[j]));
                        }
                        else if (obj.GetType() == typeof(short))
                        {
                            short n = string.IsNullOrEmpty(attrs[j]) ? (short)0 : short.Parse(attrs[j]);
                            fields[j].SetValue(t, n);
                        }
                        else if (obj.GetType() == typeof(Equipment.Type))
                        {
                            fields[j].SetValue(t, System.Enum.Parse(typeof(Equipment.Type), attrs[j]));
                        }
                        else if (obj.GetType() == typeof(Equipment.Quality))
                        {
                            fields[j].SetValue(t, System.Enum.Parse(typeof(Equipment.Quality), attrs[j]));
                        }
                        else if (obj.GetType() == typeof(WeaponType))
                        {
                            fields[j].SetValue(t, System.Enum.Parse(typeof(WeaponType), attrs[j]));
                        }
                        else if (obj.GetType() == typeof(Profession))
                        {
                            fields[j].SetValue(t, System.Enum.Parse(typeof(Profession), attrs[j]));
                        }
                        else if (obj.GetType() == typeof(Char.Quality))
                        {
                            fields[j].SetValue(t, System.Enum.Parse(typeof(Char.Quality), attrs[j]));
                        }
                        else if (obj.GetType() == typeof(Scene.Difficulty))
                        {
                            fields[j].SetValue(t, System.Enum.Parse(typeof(Scene.Difficulty), attrs[j]));
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.Log("str = " + attrs[j] + " - " + i + " - " + map.Count);
                    }
                }

                map.Add(t);
            }

            return map;
        }
    }
}