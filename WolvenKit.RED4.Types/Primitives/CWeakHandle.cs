using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace WolvenKit.RED4.Types
{
    public static class CWeakHandle
    {
        public static IRedBaseHandle Parse(Type handleType, RedBaseClass value)
        {
            var method = typeof(CWeakHandle).GetMethod(nameof(Parse), BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(RedBaseClass) }, null);
            var generic = method.MakeGenericMethod(handleType);

            return (IRedBaseHandle)generic.Invoke(null, new object[] { value });
        }

        public static CWeakHandle<T> Parse<T>(RedBaseClass value) where T : RedBaseClass
        {
            return new CWeakHandle<T>((T)value);
        }
    }

    [RED("whandle")]
    public class CWeakHandle<T> : IRedWeakHandle<T>, /*IRedNotifyObjectChanged,*/ IEquatable<CWeakHandle<T>>, IRedCloneable where T : RedBaseClass
    {
        //public event ObjectChangedEventHandler ObjectChanged;

        private T _chunk;

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T Chunk
        {
            get => _chunk;
            set
            {
                _chunk = value;

                //if (!Equals(_chunk, value))
                //{
                //    if (_chunk != null)
                //    {
                //        _chunk.ObjectChanged -= OnObjectChanged;
                //    }
                //
                //    var oldChunk = _chunk;
                //    _chunk = value;
                //
                //    if (_chunk != null)
                //    {
                //        _chunk.ObjectChanged += OnObjectChanged;
                //    }
                //
                //    var args = new ObjectChangedEventArgs(ObjectChangedType.Modified, null, oldChunk, _chunk);
                //    args._callStack.Add(this);
                //
                //    ObjectChanged?.Invoke(null, args);
                //}
            }
        }

        //private void OnObjectChanged(object sender, ObjectChangedEventArgs e)
        //{
        //    if (e._callStack.Contains(this))
        //    {
        //        return;
        //    }
        //    e._callStack.Add(this);
        //
        //    ObjectChanged?.Invoke(sender, e);
        //}

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public Type InnerType => typeof(T);


        public RedBaseClass GetValue() => Chunk;
        public void SetValue(RedBaseClass cls) => Chunk = (T)cls;


        public CWeakHandle(){}

        public CWeakHandle(T chunk)
        {
            Chunk = chunk;
        }


        public static implicit operator CWeakHandle<T>(T value) => new(value);
        public static implicit operator T(CWeakHandle<T> value) => value.Chunk;


        public bool Equals(CWeakHandle<T> other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }



            if (!Equals(Chunk, other.Chunk))
            {
                return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((CWeakHandle<T>)obj);
        }

        public override int GetHashCode() => EqualityComparer<T>.Default.GetHashCode((T)Chunk);

        public object ShallowCopy()
        {
            return MemberwiseClone();
        }

        public object DeepCopy(Dictionary<object, object> visited)
        {
            if (Chunk == null)
            {
                return CWeakHandle.Parse(InnerType, null);
            }

            if (!visited.TryGetValue(Chunk, out var clone))
            {
                clone = RedTypeManager.Create(Chunk.GetType());
                visited.Add(Chunk, clone);
                ((IRedCloneable)Chunk).DeepCopy(clone, visited);
            }

            return CWeakHandle.Parse(InnerType, (RedBaseClass)clone);
        }
    }
}
