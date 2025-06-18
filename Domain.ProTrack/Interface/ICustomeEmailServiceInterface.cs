using Domain.ProTrack.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ProTrack.Interface
{
    public interface ICustomeEmailServiceInterface
    {
        Task<(IdentityResult, string)> SendEmailConfirmation(AppUser user);
        Task<bool> ConfirmEmailAsync(string userId, string token);
    }
}
