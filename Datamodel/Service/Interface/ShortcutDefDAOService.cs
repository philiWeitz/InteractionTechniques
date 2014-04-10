using System;
using Datamodel.Model;

namespace Datamodel.Service.Interface
{
    public interface ShortcutDefDAOService : EntityDAO<ShortcutDefDAO>
    {
        ShortcutDefDAO GetEntityByName(String name);
    }
}