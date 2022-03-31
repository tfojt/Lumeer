using System.Collections.Generic;

namespace Lumeer.Models.Rest
{
    public class User
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public List<string> Organizations { get; set; }
        public bool Agreement { get; set; }
        public long AgreementDate { get; set; }
        public bool Newsletter { get; set; }
        public object Notifications { get; set; }
        public object Hints { get; set; }
        public DefaultWorkspace DefaultWorkspace { get; set; }
        public string TimeZone { get; set; }
        public bool AffiliatePartner { get; set; }
        public bool EmailVerified { get; set; }
        public object Onboarding { get; set; }
    }
}
