using System;
using Datamodel.Model;
using Datamodel.Service.Interface;
using NHibernate;
using NHibernate.Criterion;

namespace Datamodel.Service.Impl
{
    public class ShortcutDefDAOServiceImpl : AbstractEntity<ShortcutDefDAO>, ShortcutDefDAOService
    {
        public ShortcutDefDAO GetEntityByName(String name)
        {
            ShortcutDefDAO result = null;
            ISession session = Util.DatabaseUtil.Instance.Session;

            using (ITransaction transaction = session.BeginTransaction())
            {
                session.Clear();
                result = session
                    .CreateCriteria(typeof(ShortcutDefDAO))
                    .Add(Restrictions.Eq("Name", name))
                    .UniqueResult<ShortcutDefDAO>();
            }

            return result;
        }
    }
}