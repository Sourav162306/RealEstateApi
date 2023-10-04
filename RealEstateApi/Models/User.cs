using System.ComponentModel.DataAnnotations;

namespace RealEstateApi.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        public string Phone { get; set; }
        [Required]
        public string Password { get; set; }
        public ICollection<Property> Properties { get; set; }

    }
}
