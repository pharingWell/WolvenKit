using System;
using System.Collections;
using System.Collections.Generic;

namespace WolvenKit.RED4.Types
{
    public interface IRedRemoveable
    {
        public void Remove();
    }

    public interface IRedBaseHandle : IRedRemoveable
    {
        public Red4File File { get; }
        public int Pointer { get; set; }
    }

    public interface IRedBaseHandle<T> : IRedBaseHandle where T : IRedClass
    {
    }

    public interface IRedHandle : IRedBaseHandle, IRedType
    {
    }

    public interface IRedHandle<T> : IRedHandle, IRedBaseHandle<T>, IRedGenericType<T> where T : IRedClass
    {
    }
}
