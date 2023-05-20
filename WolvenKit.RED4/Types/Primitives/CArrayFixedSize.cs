using System.Collections;
using System.Collections.ObjectModel;

namespace WolvenKit.RED4.Types;

public class CArrayFixedSize<T> : ObservableCollection<T>, IRedArrayFixedSize<T>, IRedCloneable, IEquatable<CArrayFixedSize<T>> where T : IRedType
{
    public CArrayFixedSize(int size) : base(new T[size])
    {
        for (var i = 0; i < size; i++)
        {
            this[i] = System.Activator.CreateInstance<T>();
        }
    }

    public CArrayFixedSize(Flags flags) : base(new T[flags.Current])
    {
        var hasNext = flags.MoveNext();
        for (var i = 0; i < Count; i++)
        {
            if (hasNext)
            {
                this[i] = (T)RedTypeManager.CreateRedType(typeof(T), flags.Clone());
            }
            else
            {
                this[i] = (T)RedTypeManager.CreateRedType(typeof(T));
            }
        }
    }

    // TODO [CArrayFixedSize]: Find a better way to do this (just for `worldStaticCollisionShapeCategories_CollisionNode`)
    public CArrayFixedSize(int size1, int size2) : base(new List<T>(size1))
    {
        for (var i = 0; i < size1; i++)
        {
            this[i] = (T)RedTypeManager.CreateRedType(typeof(T), size2);
        }
    }

    public Type InnerType => typeof(T);

    protected override void ClearItems() => throw new NotSupportedException();

    protected override void InsertItem(int index, T item) => throw new NotSupportedException();

    protected override void RemoveItem(int index) => throw new NotSupportedException();

    #region IRedCloneable

    public object ShallowCopy() => MemberwiseClone();

    public object DeepCopy()
    {
        var other = new CArrayFixedSize<T>(Count);

        for (var i = 0; i < Count; i++)
        {
            if (this[i] is IRedCloneable cl)
            {
                other[i] = (T)cl.DeepCopy();
            }
            else
            {
                other[i] = this[i];
            }
        }

        return other;
    }

    #endregion IRedCloneable

    #region IEquatable

    public override bool Equals(object? obj)
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

        return Equals((CArrayFixedSize<T>?)obj);
    }

    public bool Equals(CArrayFixedSize<T>? other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        if (!Equals(Count, other.Count))
        {
            return false;
        }

        for (var i = 0; i < Count; i++)
        {
            if (!Equals(this[i], other[i]))
            {
                return false;
            }
        }

        return true;
    }

    public override int GetHashCode() => base.GetHashCode();

    #endregion IEquatable
}