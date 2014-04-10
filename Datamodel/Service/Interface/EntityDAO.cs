using System;
using System.Collections.Generic;

namespace Datamodel.Service.Interface
{
    public interface EntityDAO<T> where T : Model.AbstractEntity
    {
        T GetEntityById(Guid id);

        void DeleteEntityById(Guid id);

        T SaveEntity(T entity);

        void SaveOrUpdateEntity(T entity);

        IList<T> GetAll();
    }
}