using System;

namespace Emzi0767.NtfsDataStreams
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class NtfsDataStreamTypeValueAttribute : Attribute
    {
        public string TypeNameString { get; }

        public NtfsDataStreamTypeValueAttribute(string typeNameString)
        {
            this.TypeNameString = typeNameString;
        }
    }
}
