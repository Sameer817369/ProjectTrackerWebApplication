using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ProTrack.Interface
{
    public interface IEmailServiceInterface
    {
        public Task<IdentityResult> CreateEmailAsync(string email, string subject, string message);
    }
}
