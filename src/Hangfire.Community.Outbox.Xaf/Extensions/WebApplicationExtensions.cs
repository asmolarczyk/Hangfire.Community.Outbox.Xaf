﻿using Hangfire.Community.Outbox.Entities;
using Hangfire.Community.Outbox.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hangfire.Community.Outbox.Extensions;

public static class WebApplicationExtensions
{
    public static void UseHangfireOutboxDashboard(this WebApplication app, string path = "/hangfireoutbox")
    {

        app.MapGet(path, async (HttpContext context, [FromServices] IDbContextAccessor dbContextAccessor) =>
        {
            var latest = await dbContextAccessor.GetDbContext()
                .Set<OutboxJob>()
                .AsNoTracking()
                .Take(1000)
                .OrderByDescending(x => x.CreatedOn)
                .Select(x => new { x.Id, x.HangfireJobId, x.JobType, x.MethodName, x.Queue, x.Exception, Status = x.HangfireJobId == null && x.Exception == null ? OutboxJobStatus.Pending : x.HangfireJobId != null ? OutboxJobStatus.Processed : OutboxJobStatus.Error })
                .ToArrayAsync();

            return Results.Ok(latest);
        });
    }
}

public enum OutboxJobStatus
{
    Pending,
    Processed,
    Error
}