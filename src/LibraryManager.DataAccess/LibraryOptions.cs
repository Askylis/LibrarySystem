namespace LibraryManager.DataAccess
{
    public sealed class LibraryOptions
    {
        public required int MaxBookCount { get; set; }
        public required decimal LateFeePerDay { get; set; }
        public required int DueInDays { get; set; }
    }
}
