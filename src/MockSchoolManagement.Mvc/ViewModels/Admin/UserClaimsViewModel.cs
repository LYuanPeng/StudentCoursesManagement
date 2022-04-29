using System.Collections.Generic;

namespace MockSchoolManagement.ViewModels
{
    public class UserClaimsViewModel
    {
        public string UserId { get; set; }
        public List<UserClaim> Claims { get; set; }
        public UserClaimsViewModel()
        {
            Claims = new List<UserClaim>();
        }
    }
}
