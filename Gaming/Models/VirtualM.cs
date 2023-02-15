using System.ComponentModel.DataAnnotations;

namespace Gaming.Models
{
    public class VirtualM
    {
        [Key]
        public string Name { get; set; }

        [Required]
        public string Login { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string IP { get; set; }

        public bool IsStarted { get; set; }

    }
}
