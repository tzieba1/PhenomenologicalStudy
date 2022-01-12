using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using PhenomenologicalStudy.API.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PhenomenologicalStudy.API.Authorization.Claims
{
  public class CustomUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<User>
  {
    public CustomUserClaimsPrincipalFactory(
        UserManager<User> userManager,
        IOptions<IdentityOptions> optionsAccessor)
        : base(userManager, optionsAccessor)
    {
    }
    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(User user)
    {
      var identity = await base.GenerateClaimsAsync(user);

      identity.AddClaim(new Claim(ClaimTypes.GivenName, user.FirstName ?? ""));
      identity.AddClaim(new Claim(ClaimTypes.Surname, user.LastName ?? ""));

      return identity;
    }
  }
}
