using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class XMLItem : Attribute
{
    public string name;
    public bool multiline;

    public XMLItem(string name)
    {
        this.name = name;
    }
}

[System.AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class MeleeAttribute : Attribute{}
[System.AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class RangedAttribute : Attribute{}

[System.AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class XMLItemList : Attribute
{
    public string name;

    public XMLItemList(string name)
    {
        this.name = name;
    }
}

[System.AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class XMLLinkList : Attribute
{
    public string name;

    public XMLLinkList(string name)
    {
        this.name = name;
    }
}

[System.AttributeUsage(AttributeTargets.Class)]
public class ImageList : Attribute
{
    public Type renderedImageType;

    public ImageList(Type imageList)
    {
        this.renderedImageType = imageList;
    }
}

[System.AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class XMLItemLink : Attribute
{
    public Type type;
    public string name;

    public XMLItemLink(string name, Type t)
    {
        this.name = name;
        this.type = t;
    }
}

public class XMLRequiredPickFrom : Attribute
{
    public string name;

    public XMLRequiredPickFrom(string name)
    {
        this.name = name;
    }
}