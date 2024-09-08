namespace LibraryManager.DataAccess
{
    public sealed class LibraryOptions
    {
        public required int MaxBookCount { get; set; }
        public required decimal LateFeePerDay { get; set; }
        public required int DueInDays { get; set; }
        public required string AnonName { get; set; }
        public required string AnonAddress { get; set; }
        public required string AnonPhoneNumber { get; set; }
    }
}
