using System.Collections;
using System.Collections.Specialized;

namespace WolvenKit.RED4.Types;

public interface IRedBaseArray : IRedPrimitive, IList, INotifyCollectionChanged
{
    public Type InnerType { get; }
}

public interface IRedArray : IRedBaseArray
{
    public void AddRange(ICollection list);
}

public interface IRedArray<T> : IRedArray, IRedGenericType<T>, IList<T>
{
}

public interface IRedArrayFixedSize : IRedBaseArray
{
}

public interface IRedArrayFixedSize<T> : IRedArrayFixedSize, IRedGenericType<T>, IList<T>
{
}

public interface IRedStatic : IRedBaseArray
{
    public int MaxSize { get; set; }
}

public interface IRedStatic<T> : IRedStatic, IRedGenericType<T>, IList<T>
{
}