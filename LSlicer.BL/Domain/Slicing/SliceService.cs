using LSlicer.BL.Interaction;
using LSlicer.BL.Interaction.Contracts;
using LSlicer.Data.Interaction;
using LSlicer.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LSlicer.BL.Domain
{
    public class SliceService : ISliceService
    {
        private readonly IAppSettings _appSettings;
        private readonly IGeneratorHive<ISliceGenerator> _generatorHive;
        private readonly ISlicingParametersService _slicingParametersService;

        public SliceService(
            IAppSettings appSettings,
            ISlicingParametersService slicingParametersService, 
            IGeneratorHive<ISliceGenerator> generatorHive)
        {
            _appSettings = appSettings;
            _generatorHive = generatorHive;
            _slicingParametersService = slicingParametersService;
            _generatorHive = generatorHive;
        }


        public void MakeSlicing(IList<IPart> parts)
        {
            Dictionary<FileInfo, IList<IPart>> parametersForParts = new Dictionary<FileInfo, IList<IPart>>();

            foreach (var part in parts)
            {
                IList<FileInfo> parameterInfos = _slicingParametersService.TakeOutParameters(part.Id);
                foreach (var parametersInfo in parameterInfos)
                {
                    if (parametersForParts.ContainsKey(parametersInfo))
                        parametersForParts[parametersInfo].Add(part);
                    else
                        parametersForParts.Add(parametersInfo, new List<IPart> { part });
                }
            }
            foreach (var item in parametersForParts)
            {
                var partsToHandle = item.Value.ToArray();
                var parameters = item.Key;


                _generatorHive.Get(_appSettings.SelectedSliceEngine)
                    .SliceParts(partsToHandle, parameters, GetSlicingResultPath(parameters.Name));
            }
        }

        private FileInfo GetSlicingResultPath(string name)
        {
            var path = Path.Combine(
                PathHelper.Resolve(_appSettings.SlicingResultDirectory), 
                String.Concat("spec_", name));

            return new FileInfo(path);
        }
    }
}