using System.Collections.ObjectModel;

namespace WolvenKit.RED4.Types;

[RED("static")]
public class CStatic<T> : ObservableCollection<T>, IRedStatic<T>, IRedCloneable, IEquatable<CStatic<T>> where T : IRedType
{
    public CStatic(int size) : base(new List<T>()) => MaxSize = size;

    public CStatic(Flags flags) : base(new List<T>())
    {
        MaxSize = flags.Current;

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

    public int MaxSize { get; set; }

    public Type InnerType => typeof(T);

    protected override void InsertItem(int index, T item)
    {
        if (index >= MaxSize)
        {
            throw new NotSupportedException();
        }

        base.InsertItem(index, item);
    }

    #region IRedCloneable

    public object ShallowCopy() => MemberwiseClone();

    public object DeepCopy()
    {
        var other = new CStatic<T>(Count);

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

        return Equals((CStatic<T>?)obj);
    }

    public bool Equals(CStatic<T>? other)
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