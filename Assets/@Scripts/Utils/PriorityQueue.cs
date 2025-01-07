using System;
using System.Collections.Generic;
using UnityEngine;

public class Node : IComparable<Node>
{
    public int y => position.y;
    public int x => position.x;

    public int h;
    public int g;
    public int f;
    public Vector3Int position;

    public Node(int h, int g, int f, Vector3Int position)
    {
        this.h = h;
        this.g = g;
        this.f = f;
        this.position = position;
    }

    public int CompareTo(Node other)
    {
        return f.CompareTo(other.f); // f 값 기준으로 비교
    }
}

public class PriorityQueue<T> where T : IComparable<T>
{
    private List<T> _list = new List<T>();

    public void Push(T item)
    {
        _list.Add(item);
        if (_list.Count == 1)
        {
            return;
        }

        int targetIndex = _list.Count - 1;
        while (true)
        {
            int parentIndex = (targetIndex - 1) / 2;
            T parentItem = _list[parentIndex];
            if (parentItem.CompareTo(item) <= 0)
            {
                break;
            }

            _list[parentIndex] = item;
            _list[targetIndex] = parentItem;
            targetIndex = parentIndex;
        }
    }

    public bool IsEmpty()
    {
        return _list.Count == 0;
    }

    public T Top()
    {
        if (_list.Count == 0)
        {
            return default;
        }

        return _list[0];
    }

    public void Pop()
    {
        if (_list.Count == 0)
        {
            Debug.LogWarning("Pop called on an empty PriorityQueue!");
            return;
        }

        _list[0] = _list[_list.Count - 1];
        _list.RemoveAt(_list.Count - 1);

        int targetIndex = 0;
        while (true)
        {
            int leftIndex = targetIndex * 2 + 1;
            int rightIndex = targetIndex * 2 + 2;
            int now = targetIndex;

            if (leftIndex < _list.Count && _list[leftIndex].CompareTo(_list[now]) < 0)
            {
                now = leftIndex;
            }

            if (rightIndex < _list.Count && _list[rightIndex].CompareTo(_list[now]) < 0)
            {
                now = rightIndex;
            }

            if (now == targetIndex)
            {
                break;
            }

            (_list[targetIndex], _list[now]) = (_list[now], _list[targetIndex]);
            targetIndex = now;
        }
    }
}