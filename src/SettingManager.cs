using System.IO;
using System.Linq;

namespace YYGQ;

internal static class SettingManager
{
    internal static readonly string configPath = Path.Combine(MelonEnvironment.UserDataDirectory, $"{Name}.cfg");
    internal static readonly string effectPath = Path.Combine("UserData", "BattleEffects");

    private static MelonPreferences_Entry<Data> _setting;
    private static Data Setting => _setting.Value;

    internal static string EffectPackPath => Path.Combine(effectPath, Setting.CurrentEffect);

    internal static void Register()
    {
        var category = MelonPreferences.CreateCategory(Name, Name);
        category.SetFilePath(configPath);

        if (!Directory.Exists(effectPath))
        {
            Directory.CreateDirectory(effectPath);
        }
        
        _setting = category.CreateEntry("Setting", new Data(string.Empty));
        
        RefreshPacks();
    }

    internal static void RefreshPacks()
    {
        Setting.StoredEffects = Directory.GetDirectories(effectPath)
            .Select(Path.GetFileName)
            .ToArray();
    }
}