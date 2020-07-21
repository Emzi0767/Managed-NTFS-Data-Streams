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

namespace Emzi0767.NtfsDataStreams
{
    public enum FileDataStreamType : int
    {
        Unknown,

        [FileDataStreamTypeValue("$ATTRIBUTE_LIST")]
        AttributeList,

        [FileDataStreamTypeValue("$BITMAP")]
        Bitmap,

        [FileDataStreamTypeValue("$DATA")]
        Data,

        [FileDataStreamTypeValue("$EA")]
        ExtendedAttributes,

        [FileDataStreamTypeValue("$EA_INFORMATION")]
        ExtendedAttributeInformation,

        [FileDataStreamTypeValue("$FILE_NAME")]
        FileName,

        [FileDataStreamTypeValue("$INDEX_ALLOCATION")]
        IndexAllocation,

        [FileDataStreamTypeValue("$INDEX_ROOT")]
        IndexRoot,

        [FileDataStreamTypeValue("$LOGGED_UTILITY_STREAM")]
        LoggedUtilityStream,

        [FileDataStreamTypeValue("$OBJECT_ID")]
        ObjectId,

        [FileDataStreamTypeValue("$REPARSE_POINT")]
        ReparsePoint
    }
}
