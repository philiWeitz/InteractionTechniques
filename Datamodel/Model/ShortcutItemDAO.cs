using System;

namespace Datamodel.Model
{
    public class ShortcutItemDAO : AbstractEntity
    {
        public virtual String Name { get; set; }

        public virtual int Strength { get; set; }

        public ShortcutItemDAO()
        {
            Name = String.Empty;
            Strength = 1;
        }
    }
}