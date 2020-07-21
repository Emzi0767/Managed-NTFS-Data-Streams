// This file is part of Managed NTFS Data Streams project
//
// Copyright 2020 Emzi0767
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//   http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace Emzi0767.NtfsDataStreams
{
    internal static class InteropWrapper
    {
        #region Stream Enumerators
        public static IEnumerable<FileDataStream> EnumerateDataStreams(this FileInfo file)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file));

            // init locals
            var fpath = file.FullName;
            var fsd = new Interop.FindStreamData();
            AsIntPtr(ref fsd, out var fsdptr);
            var hFindStream = Interop.INVALID_HANDLE_VALUE;

            try
            {
                // check if we can get any stream
                if (!FindFirstStream(fpath, fsdptr, out hFindStream))
                    yield break;

                // Extract stream info
                ExtractStreamInfo(fsd.PtrToString(), out var streamName, out var streamType);
                yield return new FileDataStream(file, streamName, fsd.StreamSize, streamType);

                // Extract more streams until we run out
                while (FindNextStream(fsdptr, hFindStream))
                {
                    ExtractStreamInfo(fsd.PtrToString(), out streamName, out streamType);
                    yield return new FileDataStream(file, streamName, fsd.StreamSize, streamType);
                }
            }
            finally
            {
                if (hFindStream != Interop.INVALID_HANDLE_VALUE)
                    FindClose(hFindStream);
            }
        }

        // enumerator workarounds
        private static unsafe bool FindFirstStream(string lpFileName, IntPtr lpFindStreamData, out IntPtr hFindStream)
        {
            hFindStream = Interop.FindFirstStreamW(lpFileName, Interop.StreamInfoLevels.FindStreamInfoStandard, lpFindStreamData, 0);
            if (hFindStream == Interop.INVALID_HANDLE_VALUE)
            {
                var err = Marshal.GetLastWin32Error();
                if (err == Interop.ERROR_HANDLE_EOF)
                    return false;

                ThrowErrorAsException(err, lpFileName);
            }

            return true;
        }

        private static unsafe bool FindNextStream(IntPtr lpFindStreamData, IntPtr hFindStream)
        {
            if (!Interop.FindNextStreamW(hFindStream, lpFindStreamData))
            {
                var err = Marshal.GetLastWin32Error();
                if (err == Interop.ERROR_HANDLE_EOF)
                    return false;

                ThrowErrorAsException(err, null);
            }

            return true;
        }

        private static unsafe void FindClose(IntPtr hFindStream)
        {
            if (!Interop.FindClose(hFindStream))
                ThrowErrorAsException(Marshal.GetLastWin32Error(), null);
        }
        #endregion

        #region IO Helpers
        public static FileStream Open(FileDataStream ads, FileMode mode, FileAccess access, FileShare share)
        {
            if (ads.Type != FileDataStreamType.Data)
                throw new InvalidOperationException("Only $DATA streams can be opened for reading or writing.");

            if (ads.Name.Length == 0) // default stream
                return ads.File.Open(mode, access, share);

            var (nmode, nflags) = ManagedToNative(mode);
            var naccess = ManagedToNative(access);
            var nshare = ManagedToNative(share);

            var lpFileName = $"{ads.File.FullName}:{ads.Name}";

            var hFile = Interop.CreateFileW(lpFileName, naccess, nshare, IntPtr.Zero, nmode, nflags, IntPtr.Zero);
            if (hFile == Interop.INVALID_HANDLE_VALUE)
            {
                ThrowErrorAsException(Marshal.GetLastWin32Error(), lpFileName);
                return null;
            }

            return new FileStream(new SafeFileHandle(hFile, true), access, 4096, true);
        }

        public static void Delete(FileDataStream ads)
        {
            if (ads.Type != FileDataStreamType.Data)
                throw new InvalidOperationException("Only $DATA streams can be deleted.");

            if (ads.Name.Length == 0) // default stream
            {
                ads.File.Delete();
                return;
            }

            var lpFileName = $"{ads.File.FullName}:{ads.Name}";
            if (!Interop.DeleteFileW(lpFileName))
                ThrowErrorAsException(Marshal.GetLastWin32Error(), lpFileName);
        }

        private static Interop.FileShareMode ManagedToNative(FileShare share)
        {
            var mode = Interop.FileShareMode.None;

            if ((share & FileShare.Delete) == FileShare.Delete) mode |= Interop.FileShareMode.FILE_SHARE_DELETE;
            if ((share & FileShare.Read) == FileShare.Read) mode |= Interop.FileShareMode.FILE_SHARE_READ;
            if ((share & FileShare.Write) == FileShare.Write) mode |= Interop.FileShareMode.FILE_SHARE_WRITE;

            return mode;
        }

        private static Interop.FileAccessMode ManagedToNative(FileAccess access)
        {
            var mode = Interop.FileAccessMode.None;

            if ((access & FileAccess.Read) == FileAccess.Read) mode |= Interop.FileAccessMode.GENERIC_READ;
            if ((access & FileAccess.Write) == FileAccess.Write) mode |= Interop.FileAccessMode.GENERIC_WRITE;

            return mode;
        }

        private static (Interop.FileCreationDisposition creation, Interop.FileFlagsAndAttributes attribs) ManagedToNative(FileMode mode)
        {
            var mode1 = mode switch
            {
                FileMode.CreateNew => Interop.FileCreationDisposition.CREATE_NEW,
                FileMode.Create => Interop.FileCreationDisposition.CREATE_ALWAYS,
                FileMode.Open => Interop.FileCreationDisposition.OPEN_EXISTING,
                FileMode.OpenOrCreate => Interop.FileCreationDisposition.OPEN_ALWAYS,
                FileMode.Truncate => Interop.FileCreationDisposition.TRUNCATE_EXISTING,
                FileMode.Append => Interop.FileCreationDisposition.OPEN_ALWAYS
            };

            return (mode1, Interop.FileFlagsAndAttributes.FILE_FLAG_OVERLAPPED);
        }
        #endregion

        #region Helpers
        private static unsafe void AsIntPtr<T>(ref T val, out IntPtr ptr)
            => ptr = new IntPtr(Unsafe.AsPointer(ref val));

        private static unsafe string PtrToString(this Interop.FindStreamData fsd)
            => new string(fsd.cStreamName);

        private static unsafe void ExtractStreamInfo(string s, out string name, out FileDataStreamType type)
        {
            var ss = s.AsSpan(1);
            var nl = ss.IndexOf(':');
            var ns = ss.Slice(0, nl);
            ss = ss.Slice(nl + 1);

#if NETCOREAPP2_1
            name = new string(ns);
            var ts = new string(ss);
#else
            fixed (char* sp = &ns.GetPinnableReference())
                name = new string(sp);

            string ts;
            fixed (char* sp = &ns.GetPinnableReference())
                ts = new string(sp);
#endif

            type = FileDataStreamTypeConverter.GetStreamType(ts);
        }

        private static void ThrowErrorAsException(int code, string fpath)
        {
            https://github.com/RichardD2/NTFS-Streams/blob/master/ntfsstreams/Trinet.Core.IO.Ntfs/SafeNativeMethods.cs

            fpath ??= "<null>";
            throw code switch
            {
                2   => new FileNotFoundException("Specified file was not found.", fpath),
                3   => new DirectoryNotFoundException($"Could not find a part of the path \"{fpath}\"."),
                5   => new UnauthorizedAccessException($"Access to the path \"{fpath}\" was denied."),
                15  => new DriveNotFoundException($"Access to the path \"{fpath}\" was denied."),
                32  => new IOException($"The process cannot access the file \"{fpath}\" because it is being used by another process.", HResultFromError(code)),
                80  => new IOException($"The file \"{fpath}\" already exists.", HResultFromError(code)),
                87  => new IOException(MessageFromError(code), HResultFromError(code)),
                183 => new IOException($"Cannot create \"{fpath}\" because a file or directory with the same name already exists."),
                206 => new PathTooLongException(),
                995 => new OperationCanceledException(),
                _   => Marshal.GetExceptionForHR(HResultFromError(code))
            };
        }

        private static int HResultFromError(int code)
            => unchecked((int)0x80070000 | code);

        private unsafe static string MessageFromError(int code)
        {
            var lpBufferSize = 512;
            var lpBuffer = stackalloc char[lpBufferSize];
            lpBufferSize = Interop.FormatMessage(Interop.FormatMessageFlags.Defaults, IntPtr.Zero, code, 0, new IntPtr(lpBuffer), lpBufferSize, IntPtr.Zero);
            if (lpBufferSize != 0)
                return new string(lpBuffer, 0, lpBufferSize);

            return $"Unknown IO error.";
        }

        internal static FileStream SeekToEnd(this FileStream fs)
        {
            fs.Seek(0, SeekOrigin.End);
            return fs;
        }
        #endregion
    }
}
