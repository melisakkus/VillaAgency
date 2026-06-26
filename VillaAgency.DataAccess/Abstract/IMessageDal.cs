using VillaAgency.DataAccess.Abstract.Common;
using VillaAgency.Entity.Entities;

namespace VillaAgency.DataAccess.Abstract
{
    public interface IMessageDal : IGenericDal<Message>
    {
        Task MarkAsReadAsync(string id);

    }
}
