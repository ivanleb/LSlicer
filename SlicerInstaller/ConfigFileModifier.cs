using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlicerInstaller
{
    public static class ConfigFileModifier
    {
        public static void ApplyMapping(string mappingFile, string configFile) 
        {
            Dictionary<string, string> _debugToReleaseMapping = new Dictionary<string, string>(20);

            using (var reader = new StreamReader(mappingFile))
            {
                string line;
                while ((line = reader.ReadLine()) != null) 
                { 
                    var rule = line.Split('=').Where(x => x != String.Empty).Take(2).ToList();
                    _debugToReleaseMapping.Add(rule[0], rule[1]);
                }
            }

            List<string> config = new List<string>();
            using (var reader = new StreamReader(configFile))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                    config.Add(line);
            }

            foreach (var rule in _debugToReleaseMapping)
                for (int i = 0; i < config.Count; i++)
                    config[i] = config[i].Replace(rule.Key, rule.Value);


            using (var writer = new StreamWriter(configFile, append: false))
                config.ForEach(line => writer.WriteLine(line));    
        }
    }
}
