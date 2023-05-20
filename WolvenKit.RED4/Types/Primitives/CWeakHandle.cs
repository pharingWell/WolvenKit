using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace WolvenKit.RED4.Types;

public static class CWeakHandle
{
    public static IRedBaseHandle Parse(Type handleType, RedBaseClass? value)
    {
        var method = typeof(CWeakHandle).GetMethod(nameof(Parse), BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(RedBaseClass) }, null);
        if (method == null)
        {
            throw new MissingMethodException(nameof(CWeakHandle), nameof(Parse));
        }

        var generic = method.MakeGenericMethod(handleType);
        if (generic.Invoke(null, new object[] { value }) is not IRedBaseHandle result)
        {
            throw new Exception();
        }

        return result;
    }

    public static CWeakHandle<T> Parse<T>(RedBaseClass? value) where T : RedBaseClass
    {
        return new CWeakHandle<T>((T?)value);
    }
}

[RED("whandle")]
public class CWeakHandle<T> : IRedWeakHandle<T>, IEquatable<CWeakHandle<T>> where T : RedBaseClass
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private T? _chunk;

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public T? Chunk
    {
        get => _chunk;
        set => SetField(ref _chunk, value);
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public Type InnerType => typeof(T);

    public CWeakHandle() {}
    public CWeakHandle(T? chunk) => Chunk = chunk;

    public RedBaseClass? GetValue() => Chunk;
    public void SetValue(RedBaseClass? cls) => Chunk = (T?)cls;


    public static implicit operator CWeakHandle<T>(T value) => new(value);
    public static implicit operator T?(CWeakHandle<T> value) => value.Chunk;


    public bool Equals(CWeakHandle<T>? other)
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

        return Equals((CWeakHandle<T>)obj);
    }

    public override int GetHashCode() => EqualityComparer<T>.Default.GetHashCode((T)Chunk);

    #region INotifyPropertyChanged

    public event PropertyChangingEventHandler? PropertyChanging;
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanging([CallerMemberName] string? propertyName = null) => PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        OnPropertyChanging(propertyName);
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    #endregion INotifyPropertyChanged
}