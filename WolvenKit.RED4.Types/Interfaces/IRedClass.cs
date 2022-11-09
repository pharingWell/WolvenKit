using System;
using System.Collections.Generic;

namespace WolvenKit.RED4.Types
{
    public interface IRedCloneable
    {
        public object ShallowCopy();

        public object DeepCopy() => DeepCopy(new Dictionary<object, object>(ReferenceEqualityComparer.Instance));
        public object DeepCopy(Dictionary<object, object> visited);
        internal virtual void DeepCopy(object target, Dictionary<object, object> visited) { }
    }

    public interface IRedClass : IRedType
    {

    }
}
