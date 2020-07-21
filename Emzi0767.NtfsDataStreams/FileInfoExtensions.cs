using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Emzi0767.NtfsDataStreams
{
    /// <summary>
    /// Contains NTFS-ADS extension methods for <see cref="FileInfo"/> class.
    /// </summary>
    public static class FileInfoExtensions
    {
        /// <summary>
        /// Enumerates all data streams contained within specified file.
        /// </summary>
        /// <param name="file">File to query data streams for.</param>
        /// <returns>An enumerator over data streams contained within the file.</returns>
        public static IEnumerable<NtfsDataStream> EnumerateDataStreams(this FileInfo file)
            => InteropWrapper.EnumerateDataStreams(file);

        /// <summary>
        /// Creates a new alternate data stream within specified file. If a stream with this name exists, an exception is thrown.
        /// </summary>
        /// <param name="file">File to create the data stream for.</param>
        /// <param name="name">Name of the data stream to create.</param>
        /// <returns><see cref="FileStream"/> instance for IO operations.</returns>
        public static FileStream CreateDataStream(this FileInfo file, string name)
            => InteropWrapper.Open(new NtfsDataStream(file, name, 0, NtfsDataStreamType.Data), FileMode.CreateNew, FileAccess.Write, FileShare.None);

        /// <summary>
        /// Creates a new alternate data stream within specified file in text mode. If a stream with this name exists, an exception is thrown.
        /// </summary>
        /// <param name="file">File to create the data stream for.</param>
        /// <param name="name">Name of the data stream to create.</param>
        /// <param name="encoding">Encoding to use for text.</param>
        /// <returns><see cref="StreamWriter"/> instance for text writing operations.</returns>
        public static StreamWriter CreateTextDataStream(this FileInfo file, string name, Encoding encoding)
            => new StreamWriter(CreateDataStream(file, name), encoding);

        /// <summary>
        /// Gets an existing alternate data stream within specified file. Throws if specified stream does not exist.
        /// </summary>
        /// <param name="file">File to query data streams for.</param>
        /// <param name="name">Name of the data stream to retrieve.</param>
        /// <returns>Specified stream info.</returns>
        public static NtfsDataStream GetNtfsDataStream(this FileInfo file, string name)
            => EnumerateDataStreams(file).FirstOrDefault(x => x.Name == name);
    }
}
