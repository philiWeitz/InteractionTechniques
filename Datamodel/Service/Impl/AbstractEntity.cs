using System;
using System.Collections.Generic;
using NHibernate;

namespace Datamodel.Service.Impl
{
    public abstract class AbstractEntity<T> where T : Model.AbstractEntity
    {
        public T GetEntityById(Guid id)
        {
            T entity = null;
            ISession session = Util.DatabaseUtil.Instance.Session;

            using (ITransaction transaction = session.BeginTransaction())
            {
                session.Clear();
                entity = (T)session.Get(typeof(T), id);
            }

            return entity;
        }

        public void DeleteEntityById(Guid id)
        {
            ISession session = Util.DatabaseUtil.Instance.Session;
            T item = GetEntityById(id);

            if (null != item)
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    session.Clear();
                    session.Delete(item);
                    transaction.Commit();
                }
            }
        }

        public T SaveEntity(T entity)
        {
            ISession session = Util.DatabaseUtil.Instance.Session;

            using (ITransaction transaction = session.BeginTransaction())
            {
                Guid id = (Guid)session.Save(entity);
                entity.Id = id;

                transaction.Commit();
            }
            return entity;
        }

        public void SaveOrUpdateEntity(T entity)
        {
            ISession session = Util.DatabaseUtil.Instance.Session;

            using (ITransaction transaction = session.BeginTransaction())
            {
                session.Clear();
                session.SaveOrUpdate(entity);
                transaction.Commit();
            }
        }

        public IList<T> GetAll()
        {
            IList<T> result = new List<T>();
            ISession session = Util.DatabaseUtil.Instance.Session;

            using (ITransaction transaction = session.BeginTransaction())
            {
                session.Clear();
                result = session.QueryOver<T>().List();
                transaction.Commit();
            }

            return result;
        }
    }
}