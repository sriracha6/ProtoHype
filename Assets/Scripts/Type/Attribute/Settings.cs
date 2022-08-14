using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeybindAttribute : System.Attribute
{
    public string Text;

    public KeybindAttribute(string Text)
    {
        this.Text = Text;
    }
}

public class DefaultSetting : System.Attribute
{
    public object defaultSetting;

    public DefaultSetting(object @default)
    {
        this.defaultSetting = @default;
    }
}

public class SettingDivider : System.Attribute
{
    public SettingDivider()
    {

    }
}
