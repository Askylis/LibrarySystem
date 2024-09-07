namespace SkyHope.LibraryManager.WebApi.HttpModels
{
    public class UserUpdateDto
    {
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public decimal LateFeeDue { get; set; }
    }
}
