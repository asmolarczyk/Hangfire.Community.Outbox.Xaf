using Hangfire.Community.Outbox.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hangfire.Community.Outbox.Extensions;

public static class ModelBuilderExtensions
{
    public static void MapOutboxJobs(this ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OutboxJob).Assembly);
    }
}