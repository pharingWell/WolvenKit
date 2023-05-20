using System.Collections;
using System.Collections.ObjectModel;

namespace WolvenKit.RED4.Types;

[RED("array")]
public class CArray<T> : ObservableCollection<T>, IRedArray<T>, IRedCloneable, IEquatable<CArray<T>> where T : IRedType
{
    public CArray() : base(new List<T>())
    {

    }

    public CArray(int size) : base(new List<T>(size))
    {

    }

    public Type InnerType => typeof(T);

    public void AddRange(ICollection collection)
    {
        foreach (var item in collection)
        {
            Add((T)item);
        }
    }

    public void AddRange(IList<T> collection)
    {
        foreach (var item in collection)
        {
            Add(item);
        }
    }

    #region IRedCloneable

    public object ShallowCopy() => MemberwiseClone();

    public object DeepCopy()
    {
        var other = new CArray<T>();

        foreach (var element in this)
        {
            if (element is IRedCloneable cl)
            {
                other.Add((T)cl.DeepCopy());
            }
            else
            {
                other.Add(element);
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

        return Equals((CArray<T>?)obj);
    }

    public bool Equals(CArray<T>? other)
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