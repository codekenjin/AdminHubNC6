#nullable disable

using Microsoft.EntityFrameworkCore;

namespace AdminHubNC6.Data
{
    public class TodoEventDbContext : DbContext
    {
        public TodoEventDbContext(DbContextOptions<TodoEventDbContext> options)
            : base(options)
        {
        }

        public DbSet<AdminHubNC6.Models.TodoEventModel> TodoEvent { get; set; }
    }
}