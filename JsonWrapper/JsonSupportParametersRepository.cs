using LSlicer.BL.Interaction;
using LSlicer.Data.Model;
using LSlicer.Data.Interaction.Contracts;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace JsonWrapper
{
    public class JsonSupportParametersRepository : ISupportParametersRepository
    {
        private readonly FileInfo _file;
        private readonly ILoggerService _loggerService;
        private ICollection<ISupportParameters> _parameters;

        public JsonSupportParametersRepository(FileInfo file, ILoggerService loggerService)
        {
            _file = file;
            _loggerService = loggerService;
            Init();
        }

        public void Add(ISupportParameters entity)
        {
            _parameters.Add(entity);
            _loggerService.Info($"Parameters \"{entity.ToString()}\" has been added to repository");
        }

        public int LastId()
        {
            var id = -1;
            foreach (var parameters in _parameters)
            {
                if (parameters.Id >= id) id = parameters.Id;
            }
            return id;
        }

        public IEnumerable<ISupportParameters> GetAll() => _parameters.AsEnumerable();

        public void Remove(ISupportParameters entity)
        {
            ISupportParameters supportParameters = null;
            foreach (var parameters in _parameters)
            {
                if (parameters.Id == entity.Id)
                {
                    supportParameters = parameters;
                }
            }
            if (!(supportParameters is null))
            {
                _parameters.Remove(supportParameters);
                _loggerService.Info($"Parameters \"{entity.ToString()}\" has been remove from repository");
            }
        }

        public void Save()
        {
            string json = JsonConvert.SerializeObject(_parameters, Formatting.Indented);
            if (File.Exists(_file.FullName)) File.Delete(_file.FullName);
            using (StreamWriter sw = new StreamWriter(_file.FullName, append: false))
            {
                sw.Write(json);
            }
            _loggerService.Info($"Parameters has been sved to \"{_file.FullName}\"");
        }

        public void Update(ISupportParameters entity)
        {
            var parameters = _parameters.FirstOrDefault(p => p.Id == entity.Id);
            if (parameters != null) parameters = entity;
        }

        private void Init()
        {
            using (StreamReader sr = new StreamReader(_file.FullName))
            {
                string json = sr.ReadToEnd();
                _parameters = JsonConvert.DeserializeObject<List<SupportParameters>>(json).Cast<ISupportParameters>().ToList();
            }
            _loggerService.Info($"Parameters has been loaded from \"{_file.FullName}\"");
        }
    }
}
