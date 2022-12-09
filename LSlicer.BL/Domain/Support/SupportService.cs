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
    public class SupportService<T> : ISupportService
    {
        private readonly IAppSettings _appSettings;
        private readonly IGeneratorHive<ISupportGenerator> _generatorHive;
        private readonly ISupportParametersService _supportParametersService;
        private int _numberFrom;

        public SupportService(
            IAppSettings appSettings, 
            IGeneratorHive<ISupportGenerator> generatorHive, 
            ISupportParametersService supportParametersService)
        {
            _appSettings = appSettings;
            _generatorHive = generatorHive;
            _supportParametersService = supportParametersService;
        }

        public void MakeSupports(IList<IPart> parts)
        {
            _numberFrom = parts.Count;

            Dictionary<FileInfo, IList<IPart>> parametersForParts = new Dictionary<FileInfo, IList<IPart>>();

            foreach (var part in parts)
            {
                IList<FileInfo> parameterInfos = _supportParametersService.TakeOutParameters(part.Id);
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

                _generatorHive.Get(_appSettings.SelectedSupportEngine)
                    .GenerateSupports(partsToHandle, _numberFrom, parameters, GetSupportResultPath(parameters.Name));
            }
        }

        private FileInfo GetSupportResultPath(string name)
        {
            var path = Path.Combine(
                PathHelper.Resolve(_appSettings.SlicingResultDirectory), 
                String.Concat("support_", name, DateTime.Now.ToString("yyyyMMdd_HHmmss"), ".json"));

            return new FileInfo(path);
        }
    }
}