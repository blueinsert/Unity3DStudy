using System;
using System.Collections;
using System.Collections.Generic;

public class AutoBindAttribute : Attribute
{
    public string path { get; private set; }

    public AutoBindAttribute(string path)
    {
        this.path = path;
    }
}

public class UIIntent {

    public string name;
    public string prefabName;
    public string uiControllerName;

}
