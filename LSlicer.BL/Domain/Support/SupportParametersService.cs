using LSlicer.BL.Interaction;
using LSlicer.BL.Interaction.Contracts;
using LSlicer.Helpers;
using LSlicing.Data.Interaction.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LSlicer.BL.Domain
{
    public class SupportParametersService : ISupportParametersService
    {
        private readonly ISetParametersProvider<ISupportParameters> _setParametersProvider;
        private readonly ISupportParametersRepository _supportParametersRepository;
        private readonly IAppSettings _appSettings;
        private readonly ILoggerService _logger;
        private readonly IDictionary<int, IList<(string ParametersIdentifier, FileInfo ParametersFile)>> _actualSupportParameters = new Dictionary<int, IList<(string ParametersIdentifier, FileInfo ParametersFile)>>();

        public SupportParametersService(
            ISetParametersProvider<ISupportParameters> setParametersProvider,
            ISupportParametersRepository supportParametersRepository,
            IAppSettings appSettings, 
            ILoggerService logger)
        {
            _setParametersProvider = setParametersProvider;
            _supportParametersRepository = supportParametersRepository;
            _appSettings = appSettings;
            _actualSupportParameters.Add(int.MinValue, new List<(string ParametersIdentifier, FileInfo ParametersFile)> 
                { ("default", new FileInfo(PathHelper.Resolve(_appSettings.DefaultSupportParameters)) )});
            _logger = logger;
        }

        public IEnumerable<ISupportParameters> GetAll() => _supportParametersRepository.GetAll();

        public IList<FileInfo> TakeOutParameters(int partId) 
        {
            IList<(string ParametersIdentifier, FileInfo ParametersFile)> infos;
            if (partId != int.MinValue && _actualSupportParameters.TryGetValue(partId, out infos))
            {
                _actualSupportParameters.Remove(partId);
                return infos.Select(x => x.ParametersFile).ToList();
            }
            return _actualSupportParameters[int.MinValue].Select(x => x.ParametersFile).ToList();
        }

        public void Set(ISupportParameters slicingParameters, int partId, FileInfo fileInfo)
        {

            _setParametersProvider.SetParameters(slicingParameters, fileInfo);

            if (_actualSupportParameters.ContainsKey(partId))
                _actualSupportParameters[partId].Add((slicingParameters.GetIdentifier(), fileInfo));
            else
                _actualSupportParameters.Add(partId, new List<(string ParametersIdentifier, FileInfo ParametersFile)> { (slicingParameters.GetIdentifier(), fileInfo) });

            _logger.Info($"[{nameof(SupportParametersService)}] Job parameters has been saved to \"{fileInfo.FullName}\"");
        }

        public void Save(IEnumerable<ISupportParameters> parameters)
        {
            var existingParameters = _supportParametersRepository.GetAll().ToList();
            foreach (var parametersItem in parameters)
            {
                var theSame = existingParameters.FirstOrDefault(x => x.Id == parametersItem.Id);
                if (theSame == null) 
                { 
                    _supportParametersRepository.Add(parametersItem);
                    _logger.Info($"[{nameof(SupportParametersService)}] Parameters id:{parametersItem.Id} has been added to repository.");
                }
                else 
                { 
                    _supportParametersRepository.Update(parametersItem);
                    _logger.Info($"[{nameof(SupportParametersService)}] Parameters id:{parametersItem.Id} has been updated in repository.");
                }
            }
            _supportParametersRepository.Save();
        }

        public void Add(ISupportParameters supportParameters)
        {
            _supportParametersRepository.Add(supportParameters);
            _logger.Info($"[{nameof(SlicingParametersService)}] Parameters id:{supportParameters.Id} has been added to repository.");
            //}
            //else
            //    throw new WrongSlicingParametersException("Check parameters for adding.");
        }

        public void Delete(ISupportParameters supportParameters)
        {
            _supportParametersRepository.Remove(supportParameters);
            _logger.Info($"[{nameof(SlicingParametersService)}] Parameters id:{supportParameters.Id} has been deleted to repository.");
        }

        public IList<string> GetParametersIdentifiers(int partId)
        {
            IList<(string ParametersIdentifier, FileInfo ParametersFile)> infos;
            if (_actualSupportParameters.TryGetValue(partId, out infos))
            {
                return infos.Select(x => x.ParametersIdentifier).ToList();
            }
            return _actualSupportParameters[int.MinValue].Select(x => x.ParametersIdentifier).ToList();
        }
    }
}
