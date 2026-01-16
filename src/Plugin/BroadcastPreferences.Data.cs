using BroadcastPreferencesPlugin.Data;
using Newtonsoft.Json;
using Oxide.Core;
using Oxide.Core.Configuration;

namespace BroadcastPreferencesPlugin.Plugin;

public partial class BroadcastPreferences
{
    private PluginData _data;
    private DynamicConfigFile _dataFile;

    private void LoadData()
    {
        _dataFile = Interface.Oxide.DataFileSystem.GetFile(Name);
        _dataFile.Settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        try
        {
            _data = _dataFile.ReadObject<PluginData>();
        }
        catch
        {
            Puts("Could not load data, creating new datafile.");
            _data = new PluginData();
        }
        _dataFile.Clear();
        SaveData();
    }

    private void SaveData()
    {
        if (_dataFile != null) _dataFile.WriteObject(_data);
    }
}