using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LessonMigration.Services
{
    public interface IEmailService
    {
        Task SendEmail(string emailaddress, string url);
    }
}
