using LSlicer.BL.Interaction;
using LSlicer.BL.Interaction.Contracts;
using LSlicer.Data.Model;
using LSlicing.Data.Interaction.Contracts;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace JsonWrapper
{
    public class JsonSlicingParametersRepository : ISlicingParametersRepository
    {
        private readonly FileInfo _file;
        private readonly ILoggerService _loggerService;
        private ICollection<ISlicingParameters> _parameters;

        public JsonSlicingParametersRepository(FileInfo file, ILoggerService loggerService)
        {
            _file = file;
            _loggerService = loggerService;
            Init();
        }

        public int LastId()
        {
            var id = -1;
            foreach(var parameters in _parameters)
            {
                if (parameters.Id >= id) id = parameters.Id;
            }
            return id;
        }

        public void Add(ISlicingParameters entity) 
        { 

            _parameters.Add(entity);
            _loggerService.Info($"Parameters \"{entity.ToString()}\" has been added to repository");
        }

        public IEnumerable<ISlicingParameters> GetAll() =>  _parameters.AsEnumerable();

        public void Remove(ISlicingParameters entity)
        {
            ISlicingParameters slicingParameters = null;
            foreach (var parameters in _parameters)
            {
                if (parameters.Id == entity.Id)
                {
                    slicingParameters = parameters;
                }
            }
            if (!(slicingParameters is null))
            {
                _parameters.Remove(slicingParameters);
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

        public void Update(ISlicingParameters entity)
        {
            var parameters = _parameters.FirstOrDefault(p => p.Id == entity.Id);
            if (parameters != null) parameters = entity;
        }

        private void Init() 
        {
            using (StreamReader sr = new StreamReader(_file.FullName))
            {
                string json = sr.ReadToEnd();
                _parameters = JsonConvert.DeserializeObject<List<SlicingParameters>>(json).Cast<ISlicingParameters>().ToList();
            }
            _loggerService.Info($"Parameters has been loaded from \"{_file.FullName}\"");
        }
    }
}
