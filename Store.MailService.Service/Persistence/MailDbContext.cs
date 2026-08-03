using Microsoft.EntityFrameworkCore;

namespace Store.MailService.Service.Persistence;

public class MailDbContext(DbContextOptions<MailDbContext> options) : DbContext(options)
{
    
}