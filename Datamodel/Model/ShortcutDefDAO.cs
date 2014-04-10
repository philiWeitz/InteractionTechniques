using System;
using System.Collections.Generic;

namespace Datamodel.Model
{
    public class ShortcutDefDAO : AbstractEntity
    {
        public virtual String Name { get; set; }

        public virtual int Idx { get; set; }

        public virtual bool Active { get; set; }

        public virtual String ProcessName { get; set; }

        public virtual IDictionary<String, String> GestureMap { get; set; }

        public ShortcutDefDAO()
        {
            Idx = 0;
            Active = true;
            ProcessName = String.Empty;
            GestureMap = new Dictionary<String, String>();
        }
    }
}