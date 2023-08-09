using Newtonsoft.Json;
using fafikspace.entities;
using System.IO;

namespace fafikspace.config
{
    public class ConfigService
    {
        public Config GetConfig()
        {
            var file = "Config.json";
            var data = File.ReadAllText(file);
            return JsonConvert.DeserializeObject<Config>(data);
        }
    }
}