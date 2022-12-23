namespace Rental4You.ViewModels
{
    public class UserRolesViewModel
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string UserName { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}
