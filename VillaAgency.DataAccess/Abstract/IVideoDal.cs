using VillaAgency.DataAccess.Abstract.Common;
using VillaAgency.Entity.Entities;

namespace VillaAgency.DataAccess.Abstract
{
    public interface IVideoDal : IGenericDal<VideoView>
    {
        Task MakeItActive(string id);
        Task MakeItPassive(string id);
    }
}
