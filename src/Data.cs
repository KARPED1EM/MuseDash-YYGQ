using Tomlet.Attributes;

namespace YYGQ;

public class Data
{
    [TomlPrecedingComment("The current using effect pack")]
    internal string CurrentEffect;
    
    [TomlPrecedingComment("Stored effect pack paths")]
    internal string[] StoredEffects;

    public Data()
    {
    }
    
    internal Data(string currentEffect)
    {
        CurrentEffect = currentEffect;
    }
    
}