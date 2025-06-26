using System;
using System.Collections.Generic;

[Serializable]
public class XPStateData
{
    public List<bool> xpGrantedStates;

    public XPStateData(List<bool> xpStates)
    {
        xpGrantedStates = xpStates;
    }
}
