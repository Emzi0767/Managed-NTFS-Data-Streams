namespace Emzi0767.NtfsDataStreams
{
    public enum NtfsDataStreamType : int
    {
        Unknown,

        [NtfsDataStreamTypeValue("$ATTRIBUTE_LIST")]
        AttributeList,

        [NtfsDataStreamTypeValue("$BITMAP")]
        Bitmap,

        [NtfsDataStreamTypeValue("$DATA")]
        Data,

        [NtfsDataStreamTypeValue("$EA")]
        ExtendedAttributes,

        [NtfsDataStreamTypeValue("$EA_INFORMATION")]
        ExtendedAttributeInformation,

        [NtfsDataStreamTypeValue("$FILE_NAME")]
        FileName,

        [NtfsDataStreamTypeValue("$INDEX_ALLOCATION")]
        IndexAllocation,

        [NtfsDataStreamTypeValue("$INDEX_ROOT")]
        IndexRoot,

        [NtfsDataStreamTypeValue("$LOGGED_UTILITY_STREAM")]
        LoggedUtilityStream,

        [NtfsDataStreamTypeValue("$OBJECT_ID")]
        ObjectId,

        [NtfsDataStreamTypeValue("$REPARSE_POINT")]
        ReparsePoint
    }
}
