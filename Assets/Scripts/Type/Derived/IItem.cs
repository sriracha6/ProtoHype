using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IItem<T>
{
    public T Get(int id);
    public T Get(string name);
}