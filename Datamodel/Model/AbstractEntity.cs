using System;

namespace Datamodel.Model
{
    abstract public class AbstractEntity
    {
        public virtual Guid Id { get; set; }
    }
}