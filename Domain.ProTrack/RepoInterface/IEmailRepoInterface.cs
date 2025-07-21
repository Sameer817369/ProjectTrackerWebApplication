using Domain.ProTrack.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ProTrack.RepoInterface
{
    public interface IEmailRepoInterface
    {
        Task<string> GenerateEmailTokenAsync(AppUser user);
        Task<IdentityResult> ConfirmEmailAsync(string userId, string token);
        Task<AppUser?> GetUserEmailAsync(string email);
    }
}
