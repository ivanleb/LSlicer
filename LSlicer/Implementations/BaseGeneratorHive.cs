using LSlicer.BL.Interaction;
using System;
using System.Collections.Generic;

namespace LSlicer.Implementations
{
    public class BaseGeneratorHive<T> : IGeneratorHive<T>
    {
        protected readonly IDictionary<string, T> _generators;
        protected readonly IAppSettings _appSettings;
        protected readonly ILoggerService _logger;
        protected readonly string _engineList;

        public BaseGeneratorHive(T[] generators, IAppSettings appSettings, ILoggerService logger)
        {
            _appSettings = appSettings;
            _logger = logger;
            _generators = new Dictionary<string, T>();
            _engineList = "";
            foreach (var generator in generators)
            {
                _engineList += generator.ToString() + ";";
                _generators.Add(generator.ToString(), generator);
            }
        }

        public T Get(string generatorName)
        {
            if (_generators.TryGetValue(generatorName, out T result))
            {
                _logger.Info($"[{nameof(BaseGeneratorHive<T>)}] Use {generatorName}.");
                return result;
            }
            throw new ArgumentException($"Wrong engine name {generatorName}.");
        }
    }

}
