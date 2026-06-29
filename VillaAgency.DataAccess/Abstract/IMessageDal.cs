using System.Linq.Expressions;
using VillaAgency.DataAccess.Abstract.Common;
using VillaAgency.Entity.Entities;

namespace VillaAgency.DataAccess.Abstract
{
    public interface IMessageDal : IGenericDal<Message>
    {
        Task MarkAsReadAsync(string id);
        Task MarkAsNotReadAsync(string id);
        Task MarkAsDeletedAsync(string id);
        Task MarkAsNotDeletedAsync(string id);

        Task<int> GetCountAsync(Expression<Func<Message, bool>> filter);
    }
}
