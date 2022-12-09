using LSlicer.BL.Interaction;
using LSlicer.Data.Interaction;
using LSlicer.Data.Model;
using LSlicer.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSlicer.BL.Domain
{
    public class PartRepository : IPartRepository
    {
        private readonly ILoggerService _logger;
        private readonly TreeNodeCollection<IPart> _partCollection;

        public PartRepository(ILoggerService logger)
        {
            _partCollection = new TreeNodeCollection<IPart>(new EmptyPart());
            _logger = logger;
        }

        public void Add(IPart entity)
        {
            if (_partCollection.TryAttach(entity, _partCollection.Root.Value.Id))
            {
                _logger.Info($"[{nameof(PartRepository)}] Add {entity.PartSpec.MeshFilePath} as first level part.");
            }
            else
                _logger.Info($"[{nameof(PartRepository)}] Cant add {entity.PartSpec.MeshFilePath} as first level part.");
        }

        public bool Attach(IPart part, int attachId)
        {
            if (_partCollection.TryAttach(part, attachId))
            {
                _logger.Info($"[{nameof(PartRepository)}] Attach {part.PartSpec.MeshFilePath} to part {attachId}.");
                return true;
            }
            else
            {
                _logger.Info($"[{nameof(PartRepository)}] Cannot attach {part.PartSpec.MeshFilePath} to part {attachId}.");
                return false;
            }
        }

        public List<int> PartsId = new List<int>();

        public int Copy(int id) => _partCollection.CopyNode(id, GetNextId());

        public IEnumerable<IPart> GetAll() => _partCollection;

        //public int GetNextId() => _partCollection.Count() > 0 ? _partCollection.Max(x => x.Id) + 1 : 0;
        public int GetNextId() => _partCollection.Count() > 0 ? _partCollection.Max(x => x.Id) + 1 : 0;
        /*
        public int GetNextId()
        {
            if(PartsId.Count() < 1)
            {
                PartsId.Add(0);
                return 0;
            }
            else
            {
                PartsId.Add( PartsId.Max() + 1);
                return PartsId.Max();
            }
        }
        */
        /*
        public int GetNextId()
        {
            if(_partCollection > 0)
            {
                var max = 0;
                var parts =_partCollection.ToList();
                for(var i = 0; i < _partCollection.Count(); i++)
                {
                    if(max < parts[i].Id)
                    {
                        max = parts[i].Id;
                        parts[i].
                    }
                }
            }
        }
        */
        public void Remove(IPart entity)
        {
            
            if (_partCollection.TryRemove(entity))
                _logger.Info($"[{nameof(PartRepository)}] Remove {entity.PartSpec.MeshFilePath}.");
            else
                _logger.Info($"[{nameof(PartRepository)}] Cant remove {entity.PartSpec.MeshFilePath}.");
            
        }

        public void RemoveAt(int id)
        {
            IPart part = GetAll().FirstOrDefault(x => x.Id == id);
            if(part != null)
                Remove(part);
        }

        public void Save()
        {
            //do nothing
        }

        public void Update(IPart entity)
        {
            if (_partCollection.TryUpdate(entity))
                _logger.Info($"[{nameof(PartRepository)}] Update {entity.PartSpec.MeshFilePath}.");
            else
                _logger.Info($"[{nameof(PartRepository)}] Cant update {entity.PartSpec.MeshFilePath}.");
        }

        public IPart GetById(int id)
        {
            return _partCollection.First(p => p.Id == id);
        }
    }
}
