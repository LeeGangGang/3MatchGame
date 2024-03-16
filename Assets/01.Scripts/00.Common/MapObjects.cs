using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapObjects<T>
{
    T[,] objs;
    int maxSizeX;
    int maxSizeY;

    public MapObjects(int maxSizeX, int maxSizeY)
    {
        objs = new T[maxSizeX, maxSizeY];
        this.maxSizeX = maxSizeX;
        this.maxSizeY = maxSizeY;
    }

    public T this[int x, int y]
    {
        get
        {
            if (x < 0 || x >= maxSizeX)
                return default(T);
            if (y < 0 || y >= maxSizeY)
                return default(T);

            if (objs[x, y] != null)
                return objs[x, y];
            else
                return default(T);
        }
        set
        {
            if (x < 0 || x >= maxSizeX)
                return;
            if (y < 0 || y >= maxSizeY)
                return;

            objs[x, y] = value;
        }
    }

    public T[,] All()
    {
        return objs;
    }

    public bool IsFull()
    {
        bool isFull = true;

        foreach (var obj in objs)
        {
            if (obj == null)
            {
                isFull = false;
                break;
            }
        }

        return isFull;
    }
}
