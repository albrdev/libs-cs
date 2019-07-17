using System;

namespace Libs
{
    public interface IIdentifiable<T>
    {
        T Identifier { get; }
    }

    public interface IDescriptive
    {
        string Title { get; }
        string Description { get; }
    }
}
