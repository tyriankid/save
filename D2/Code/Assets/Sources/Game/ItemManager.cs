using UnityEngine;
using System.Collections.Generic;


[System.Serializable]
public class ItemManager
{
    public class Tile
    {
        public Config.Equipment item;
        public int r;
        public int c;
        public GameObject root;
        public bool space = false;
    }

    public int row = 10;
    public int col = 12;
    public Vector2 unit = new Vector2(30, 28);

    private Tile[,] tiles;
    private List<Tile> items = new List<Tile>();

    public void Start()
    {
        tiles = new Tile[row, col];
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                tiles[i, j] = new Tile();
            }
        }
    }

    public void Clear()
    {
        items.Clear();

        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                tiles[i, j].space = false;
            }
        }
    }

    public void Update(GameObject tile, int i)
    {
        //for (int i = 0; i < items.Count; i++)
        {
            GameObject o = GameObject.Instantiate(tile) as GameObject;
            o.SetActive(true);
            o.transform.parent = tile.transform.parent;
            o.name = "Tile_" + i;
            UserData ud = o.AddComponent<UserData>();
            ud.data = items[i].item.ID.ToString();

            items[i].root = o;

            float x = items[i].c * unit.x + items[i].c * 2;
            float y = -items[i].r * unit.y - items[i].r * 2;
            o.transform.localPosition = new Vector2(x, y);
            o.transform.localScale = Vector3.one;

            UISprite sp = o.GetComponent<UISprite>();
            sp.spriteName = items[i].item.icon;
        }
    }

    public void DelItem(int id)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].item.ID == id)
            {
                Config.Equipment equipment = items[i].item;

                for (int c = 0; c < col; c++)
                {
                    for (int r = 0; r < row; r++)
                    {
                        if (tiles[r, c].space && tiles[r, c].item == equipment)
                        {
                            tiles[r, c].space = false;
                        }
                    }
                }

                items.RemoveAt(i);
                Global.LocalHero.charactor.backpack.Remove(equipment.ID);
                break;
            }
        }
    }

    public Config.Equipment DelItem(GameObject o)
    {
        if (o == null) return null;

        Config.Equipment equipment = null;
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].root == o)
            {
                equipment = items[i].item;

                for (int c = 0; c < col; c++)
                {
                    for (int r = 0; r < row; r++)
                    {
                        if (tiles[r, c].space && tiles[r, c].item == equipment)
                        {
                            tiles[r, c].space = false;
                        }
                    }
                }

                GameObject.DestroyImmediate(o);
                items.RemoveAt(i);
                Global.LocalHero.charactor.backpack.Remove(equipment.ID);
                break;
            }
        }

        return equipment;
    }

    public bool IsFull(Config.Equipment item)
    {
        if (item == null) return false;

        int s = item.costSpace;

        for (int j = 0; j < col; j++)
        {
            for (int i = 0; i < row; i++)
            {
                if (s == 11)
                {
                    if (tiles[i, j].space == false)
                    {
                        return true;
                    }
                }
                else if (s == 12)
                {
                    if (tiles[i, j].space == false && i + 1 < row && tiles[i + 1, j].space == false)
                    {
                        return true;
                    }
                }
                else if (s == 13)
                {
                    if (tiles[i, j].space == false &&
                        i + 1 < row && tiles[i + 1, j].space == false &&
                        i + 2 < row && tiles[i + 2, j].space == false)
                    {
                        return true;
                    }
                }
                else if (s == 14)
                {
                    if (tiles[i, j].space == false &&
                        i + 1 < row && tiles[i + 1, j].space == false &&
                        i + 2 < row && tiles[i + 2, j].space == false &&
                        i + 3 < row && tiles[i + 3, j].space == false)
                    {
                        return true;
                    }
                }
                else if (s == 21)
                {
                    if (tiles[i, j].space == false &&
                        j + 1 < col && tiles[i, j + 1].space == false)
                    {
                        return true;
                    }
                }
                else if (s == 22)
                {
                    if (tiles[i, j].space == false &&
                        i + 1 < row && tiles[i + 1, j].space == false &&
                        j + 1 < col && tiles[i, j + 1].space == false &&
                        i + 1 < row && j + 1 < col && tiles[i + 1, j + 1].space == false)
                    {
                        return true;
                    }
                }
                else if (s == 23)
                {
                    if (tiles[i, j].space == false &&
                        i + 1 < row && tiles[i + 1, j].space == false &&
                        j + 1 < col && tiles[i, j + 1].space == false &&
                        i + 1 < row && j + 1 < col && tiles[i + 1, j + 1].space == false &&
                        i + 2 < row && tiles[i + 2, j].space == false &&
                        i + 2 < row && j + 1 < col && tiles[i + 2, j + 1].space == false)
                    {
                        return true;
                    }
                }
                else if (s == 24)
                {
                    if (tiles[i, j].space == false &&
                        i + 1 < row && tiles[i + 1, j].space == false &&
                        j + 1 < col && tiles[i, j + 1].space == false &&
                        i + 1 < row && j + 1 < col && tiles[i + 1, j + 1].space == false &&
                        i + 2 < row && tiles[i + 2, j].space == false &&
                        i + 2 < row && j + 1 < col && tiles[i + 2, j + 1].space == false &&
                        i + 3 < row && tiles[i + 3, j].space == false &&
                        i + 3 < row && j + 1 < col && tiles[i + 3, j + 1].space == false)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public bool AddItem(Config.Equipment item)
    {
        if (item == null) return false;

        int s = item.costSpace;

        for (int j = 0; j < col; j++)
        {
            for (int i = 0; i < row; i++)
            {
                if (s == 11)
                {
                    if (tiles[i, j].space == false)
                    {
                        tiles[i, j].item = item;
                        tiles[i, j].r = i;
                        tiles[i, j].c = j;
                        tiles[i, j].space = true;

                        items.Add(tiles[i, j]);
                        return true;
                    }
                }
                else if (s == 12)
                {
                    if (tiles[i, j].space == false && i + 1 < row && tiles[i + 1, j].space == false)
                    {
                        tiles[i, j].item = item;
                        tiles[i, j].r = i;
                        tiles[i, j].c = j;
                        tiles[i, j].space = true;
                        tiles[i + 1, j].item = item;
                        tiles[i + 1, j].r = i;
                        tiles[i + 1, j].c = j;
                        tiles[i + 1, j].space = true;

                        items.Add(tiles[i, j]);
                        return true;
                    }
                }
                else if (s == 13)
                {
                    if (tiles[i, j].space == false &&
                        i + 1 < row && tiles[i + 1, j].space == false &&
                        i + 2 < row && tiles[i + 2, j].space == false)
                    {
                        tiles[i, j].item = item;
                        tiles[i, j].r = i;
                        tiles[i, j].c = j;
                        tiles[i, j].space = true;
                        tiles[i + 1, j].item = item;
                        tiles[i + 1, j].r = i;
                        tiles[i + 1, j].c = j;
                        tiles[i + 1, j].space = true;
                        tiles[i + 2, j].item = item;
                        tiles[i + 2, j].r = i;
                        tiles[i + 2, j].c = j;
                        tiles[i + 2, j].space = true;

                        items.Add(tiles[i, j]);
                        return true;
                    }
                }
                else if (s == 14)
                {
                    if (tiles[i, j].space == false &&
                        i + 1 < row && tiles[i + 1, j].space == false &&
                        i + 2 < row && tiles[i + 2, j].space == false &&
                        i + 3 < row && tiles[i + 3, j].space == false)
                    {
                        tiles[i, j].item = item;
                        tiles[i, j].r = i;
                        tiles[i, j].c = j;
                        tiles[i, j].space = true;
                        tiles[i + 1, j].item = item;
                        tiles[i + 1, j].r = i;
                        tiles[i + 1, j].c = j;
                        tiles[i + 1, j].space = true;
                        tiles[i + 2, j].item = item;
                        tiles[i + 2, j].r = i;
                        tiles[i + 2, j].c = j;
                        tiles[i + 2, j].space = true;
                        tiles[i + 3, j].item = item;
                        tiles[i + 3, j].r = i;
                        tiles[i + 3, j].c = j;
                        tiles[i + 3, j].space = true;

                        items.Add(tiles[i, j]);
                        return true;
                    }
                }
                else if (s == 21)
                {
                    if (tiles[i, j].space == false &&
                        j + 1 < col && tiles[i, j + 1].space == false)
                    {
                        tiles[i, j].item = item;
                        tiles[i, j].r = i;
                        tiles[i, j].c = j;
                        tiles[i, j].space = true;
                        tiles[i, j + 1].item = item;
                        tiles[i, j + 1].r = i;
                        tiles[i, j + 1].c = j;
                        tiles[i, j + 1].space = true;

                        items.Add(tiles[i, j]);
                        return true;
                    }
                }
                else if (s == 22)
                {
                    if (tiles[i, j].space == false &&
                        i + 1 < row && tiles[i + 1, j].space == false &&
                        j + 1 < col && tiles[i, j + 1].space == false &&
                        i + 1 < row && j + 1 < col && tiles[i + 1, j + 1].space == false)
                    {
                        tiles[i, j].item = item;
                        tiles[i, j].r = i;
                        tiles[i, j].c = j;
                        tiles[i, j].space = true;
                        tiles[i + 1, j].item = item;
                        tiles[i + 1, j].r = i;
                        tiles[i + 1, j].c = j;
                        tiles[i + 1, j].space = true;
                        tiles[i, j + 1].item = item;
                        tiles[i, j + 1].r = i;
                        tiles[i, j + 1].c = j;
                        tiles[i, j + 1].space = true;
                        tiles[i + 1, j + 1].item = item;
                        tiles[i + 1, j + 1].r = i;
                        tiles[i + 1, j + 1].c = j;
                        tiles[i + 1, j + 1].space = true;

                        items.Add(tiles[i, j]);
                        return true;
                    }
                }
                else if (s == 23)
                {
                    if (tiles[i, j].space == false &&
                        i + 1 < row && tiles[i + 1, j].space == false &&
                        j + 1 < col && tiles[i, j + 1].space == false &&
                        i + 1 < row && j + 1 < col && tiles[i + 1, j + 1].space == false &&
                        i + 2 < row && tiles[i + 2, j].space == false &&
                        i + 2 < row && j + 1 < col && tiles[i + 2, j + 1].space == false)
                    {
                        tiles[i, j].item = item;
                        tiles[i, j].r = i;
                        tiles[i, j].c = j;
                        tiles[i, j].space = true;
                        tiles[i + 1, j].item = item;
                        tiles[i + 1, j].r = i;
                        tiles[i + 1, j].c = j;
                        tiles[i + 1, j].space = true;
                        tiles[i, j + 1].item = item;
                        tiles[i, j + 1].r = i;
                        tiles[i, j + 1].c = j;
                        tiles[i, j + 1].space = true;
                        tiles[i + 1, j + 1].item = item;
                        tiles[i + 1, j + 1].r = i;
                        tiles[i + 1, j + 1].c = j;
                        tiles[i + 1, j + 1].space = true;

                        tiles[i + 2, j].item = item;
                        tiles[i + 2, j].r = i;
                        tiles[i + 2, j].c = j;
                        tiles[i + 2, j].space = true;
                        tiles[i + 2, j + 1].item = item;
                        tiles[i + 2, j + 1].r = i;
                        tiles[i + 2, j + 1].c = j;
                        tiles[i + 2, j + 1].space = true;

                        items.Add(tiles[i, j]);
                        return true;
                    }
                }
                else if (s == 24)
                {
                    if (tiles[i, j].space == false &&
                        i + 1 < row && tiles[i + 1, j].space == false &&
                        j + 1 < col && tiles[i, j + 1].space == false &&
                        i + 1 < row && j + 1 < col && tiles[i + 1, j + 1].space == false &&
                        i + 2 < row && tiles[i + 2, j].space == false &&
                        i + 2 < row && j + 1 < col && tiles[i + 2, j + 1].space == false &&
                        i + 3 < row && tiles[i + 3, j].space == false &&
                        i + 3 < row && j + 1 < col && tiles[i + 3, j + 1].space == false)
                    {
                        tiles[i, j].item = item;
                        tiles[i, j].r = i;
                        tiles[i, j].c = j;
                        tiles[i, j].space = true;
                        tiles[i + 1, j].item = item;
                        tiles[i + 1, j].r = i;
                        tiles[i + 1, j].c = j;
                        tiles[i + 1, j].space = true;
                        tiles[i, j + 1].item = item;
                        tiles[i, j + 1].r = i;
                        tiles[i, j + 1].c = j;
                        tiles[i, j + 1].space = true;
                        tiles[i + 1, j + 1].item = item;
                        tiles[i + 1, j + 1].r = i;
                        tiles[i + 1, j + 1].c = j;
                        tiles[i + 1, j + 1].space = true;

                        tiles[i + 2, j].item = item;
                        tiles[i + 2, j].r = i;
                        tiles[i + 2, j].c = j;
                        tiles[i + 2, j].space = true;
                        tiles[i + 2, j + 1].item = item;
                        tiles[i + 2, j + 1].r = i;
                        tiles[i + 2, j + 1].c = j;
                        tiles[i + 2, j + 1].space = true;

                        tiles[i + 3, j].item = item;
                        tiles[i + 3, j].r = i;
                        tiles[i + 3, j].c = j;
                        tiles[i + 3, j].space = true;
                        tiles[i + 3, j + 1].item = item;
                        tiles[i + 3, j + 1].r = i;
                        tiles[i + 3, j + 1].c = j;
                        tiles[i + 3, j + 1].space = true;

                        items.Add(tiles[i, j]);
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public int LastItem()
    {
        return items.Count - 1;
    }
}