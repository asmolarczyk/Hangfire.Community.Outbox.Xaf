namespace Hangfire.Community.Outbox.Xaf.Extensions;

using Entities;
using Microsoft.EntityFrameworkCore;

public static class ModelBuilderExtensions
{
    public static void MapOutboxJobs(this ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OutboxJob).Assembly);
    }
}