using LSlicer.BL.Exceptions;
using LSlicer.BL.Interaction;
using LSlicer.BL.Interaction.Contracts;
using LSlicer.Helpers;
using LSlicer.Data.Interaction.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSlicer.BL.Domain
{
    public class SlicingParametersService : ISlicingParametersService
    {
        private readonly ISlicingParametersRepository _slicingParametersRepository;
        private readonly ISetParametersProvider<ISlicingParameters> _setSlicingParametersProvider;
        private readonly IAppSettings _appSettings;
        private readonly ILoggerService _logger;
        private readonly IDictionary<int, IList<(string ParametersIdentifier, FileInfo ParametersFile)>> _actualSlicingParameters = new Dictionary<int, IList<(string, FileInfo)>>();

        public SlicingParametersService(
            ISlicingParametersRepository slicingParametersRepository,
            ISetParametersProvider<ISlicingParameters> setSlicingParametersProvider,
            IAppSettings appSettings, 
            ILoggerService logger)
        {
            _slicingParametersRepository = slicingParametersRepository;
            _setSlicingParametersProvider = setSlicingParametersProvider;
            _appSettings = appSettings;
            //_actualSlicingParameters.Add(int.MinValue, new List<(string,FileInfo)> { ("default", new FileInfo(PathHelper.Resolve(_appSettings.DefaultSlicingParameters))) });
            _logger = logger;
        }

        public IEnumerable<ISlicingParameters> GetAll() => _slicingParametersRepository.GetAll();

        public IList<FileInfo> TakeOutParameters(int partId) 
        {
            IList<(string ParametersIdentifier, FileInfo ParametersFile)> infos;
            if (partId != int.MinValue && _actualSlicingParameters.TryGetValue(partId, out infos))
            {
                _actualSlicingParameters.Remove(partId);
                return infos.Select(x => x.ParametersFile).ToList();
            }
            return _actualSlicingParameters[int.MinValue].Select(x => x.ParametersFile).ToList();
        }

        public void Save(IEnumerable<ISlicingParameters> parameters)
        {
            var existingParameters = _slicingParametersRepository.GetAll().ToList();
            foreach (var parametersItem in parameters)
            {
                var theSame = existingParameters.FirstOrDefault(x => x.Id == parametersItem.Id);
                if (theSame == null)
                {
                    _slicingParametersRepository.Add(parametersItem);
                    _logger.Info($"[{nameof(SlicingParametersService)}] Parameters id:{parametersItem.Id} has been added to repository.");
                }
                else
                {
                    _slicingParametersRepository.Update(parametersItem);
                    _logger.Info($"[{nameof(SlicingParametersService)}] Parameters id:{parametersItem.Id} has been updated in repository.");
                }
            }
            _slicingParametersRepository.Save();
        }

        public void Set(ISlicingParameters slicingParameters, int partId, FileInfo fileInfo)
        {
            if (!SlicingParametersValidator.Check(slicingParameters))
                throw new WrongSlicingParametersException("Check parameters for saving.");

                _setSlicingParametersProvider.SetParameters(slicingParameters, fileInfo);
                if (_actualSlicingParameters.ContainsKey(partId))
                    _actualSlicingParameters[partId].Add((slicingParameters.GetIdentifier() , fileInfo));
                else
                    _actualSlicingParameters.Add(partId, new List<(string ParametersIdentifier, FileInfo ParametersFile)> { (slicingParameters.GetIdentifier(), fileInfo) });
                _logger.Info($"[SlicingParametersService] Job parameters has been saved to \"{fileInfo.FullName}\"");
         }

        public void Add(ISlicingParameters slicingParameters)
        {
            if (SlicingParametersValidator.Check(slicingParameters))
            {
                _slicingParametersRepository.Add(slicingParameters);
                _logger.Info($"[{nameof(SlicingParametersService)}] Parameters id:{slicingParameters.Id} has been added to repository.");
            }
            else
                throw new WrongSlicingParametersException("Check parameters for adding.");
        }

        public void Delete(ISlicingParameters slicingParameters)
        {
            _slicingParametersRepository.Remove(slicingParameters);
            _logger.Info($"[{nameof(SlicingParametersService)}] Parameters id:{slicingParameters.Id} has been deleted to repository.");
        }

        public IList<string> GetParametersIdentifiers(int partId)
        {
            IList<(string ParametersIdentifier, FileInfo ParametersFile)> infos;
            if (_actualSlicingParameters.TryGetValue(partId, out infos))
            {
                return infos.Select(x => x.ParametersIdentifier).ToList();
            }
            return _actualSlicingParameters[int.MinValue].Select(x => x.ParametersIdentifier).ToList();
        }
    }
}
