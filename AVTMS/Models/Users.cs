
using Microsoft.AspNetCore.Identity;

namespace AVTMS.Models
{
    public class Users : IdentityUser
    {
        public  String FullName {  get; set; }

        
    }
}
