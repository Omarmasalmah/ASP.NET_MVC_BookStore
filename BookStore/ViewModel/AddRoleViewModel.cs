using System.ComponentModel.DataAnnotations;

namespace BookStore.ViewModel
{
    public class AddRoleViewModel
    {
        [Required]
        [Display(Name = "Role")]
        public string RoleName { get; set; }
    }
}
