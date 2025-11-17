namespace ATM_BackEnd;
using Microsoft.EntityFrameworkCore;

public class ATMContext(DbContextOptions<ATMContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<ATMTransaction> Transactions { get; set; }
}