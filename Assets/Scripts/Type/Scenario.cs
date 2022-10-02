using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using XMLLoader;

public class Scenario
{
    public string Filepath { get; private set; }
    public string Directory { get { return Filepath.Substring(0, Filepath.Length - System.IO.Path.GetFileName(Filepath).Length); } }
    public string Name { get { return Loaders.LoadXML(Filepath).Q<XmlNode>("Scenario").Q<string>("Name"); } }
    public string Description { get { return Loaders.LoadXML(Filepath).Q<XmlNode>("Scenario").Q<string>("Description"); } }
    public string LocalImagePath { get { return Loaders.LoadXML(Filepath).Q<XmlNode>("Scenario").Q<string>("IconPath"); } }

    public static List<Scenario> List = new List<Scenario>();

    public Scenario(string filepath)
    {
        Filepath = filepath;
        List.Add(this);
    }
}
