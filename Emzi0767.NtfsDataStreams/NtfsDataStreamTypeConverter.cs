using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Emzi0767.NtfsDataStreams
{
    public static class NtfsDataStreamTypeConverter
    {
        private static IReadOnlyDictionary<string, NtfsDataStreamType> TypeCache { get; } = GenerateTypeCache();

        public static NtfsDataStreamType GetStreamType(string typeName)
            => TypeCache.TryGetValue(typeName, out var streamType) switch
            {
                true => streamType,
                _ => NtfsDataStreamType.Unknown
            };

        private static IReadOnlyDictionary<string, NtfsDataStreamType> GenerateTypeCache()
            => typeof(NtfsDataStreamType)
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Select(x => new { value = (NtfsDataStreamType)x.GetValue(null), name = x.GetCustomAttribute<NtfsDataStreamTypeValueAttribute>() })
                .Where(x => x.name != null)
                .ToDictionary(x => x.name.TypeNameString, x => x.value);
    }
}
