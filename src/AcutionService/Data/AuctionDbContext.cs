using System;
using AcutionService.Entities;
using Microsoft.EntityFrameworkCore;

namespace AcutionService.Data;

public class AuctionDbContext : DbContext
{
    public AuctionDbContext(DbContextOptions options) : base(options)
    {
    }
    public DbSet<Auction> Auctions { get; set; }
     
}
