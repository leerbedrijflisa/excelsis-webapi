namespace Lisa.Excelsis.WebApi
{
    public class UserProfile
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Number { get; set; }
        public string[] Roles { get; set; }
    }
}
