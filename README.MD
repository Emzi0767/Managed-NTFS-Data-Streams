# Managed NTFS Data Streams
A Win32 API wrapper, enabling support for NTFS Alternate Data Streams within .NET Standard and .NET Core. 

As the library is built around Win32 P/Invoke, platforms other than Windows are not supported. Furthermore, as the APIs 
used are specific to Windows versions no less than Vista, this library will not run under Windows XP. Lastly, the 
feature set implemented targets NTFS specifically, however any file system with named data stream support should work, 
provided the loaded driver exposes said functionality (as of Windows 10 1909, the only built-in filesystems which 
support this feature are NTFS and ReFS, albeit the latter has some limitations).

## Usage
After installing the library via NuGet, several extension methods for `FileInfo` will become available via the 
`Emzi0767.NtfsDataStreams` namespace.

### `FileDataStream` class
This class provides information about a single data stream, including stream's byte length, name, and the file it's 
attached to.

This class provides an interface similar to that of `FileInfo`. Using an instance of this class, you an perform I/O 
operations on an existing stream, such as reading, writing, or deleting.

### Querying available data streams
```cs
fileInfo.EnumerateDataStreams()
```

Returns an enumerator over the available data streams for given file. Each stream will be represented as an instance 
of `FileDataStream` class.

**NOTE**: Improperly disposing of this enumerator might lead to handle leakage. When using this enumerator outside of 
LINQ or `foreach` loops, make sure to enumerate it fully, or dispose of it explicitly. This way any internal handles 
will be released.

### Retrieving a single data stream
```cs
fileInfo.GetDataStream(string name)
```

Returns information about a single data stream as an instance of `FileDataStream`, or `null` if a stream with a given 
name does not exist.

### Creating a new data stream
```cs
fileInfo.CreateDataStream(string name)
```

Creates a new data stream, and returns a `FileStream` for performing I/O operations on it. If the stream already 
exists, this method will fail with an exception.

```cs
fileInfo.CreateTextDataStream(string name, Encoding encoding)
```

Like above, but wraps the created stream into a `StreamWriter` instance.

## Notes about data streams
Every file has a default stream, with an empty name. This stream is missing for directories. In this implementation, 
any operations on the default stream will be applied to the file instead.
