namespace Hangfire.Community.Outbox.Xaf.Extensions;

using DevExpress.ExpressApp;
using DevExpress.Xpo;
using Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

public static class WebApplicationExtensions
{
    public static void UseHangfireOutboxDashboard(this WebApplication app, string path = "/hangfireoutbox")
    {
        app.MapGet(path, async (HttpContext context, [FromServices] INonSecuredObjectSpaceFactory osFactory) =>
        {
            using var os = osFactory.CreateNonSecuredObjectSpace(typeof(OutboxJob));
            var latest = await os.GetObjectsQuery<OutboxJob>()
                .Take(1000)
                .OrderByDescending(x => x.CreatedOn)
                .Select(x => new { Id = x.Oid, x.HangfireJobId, x.JobType, x.MethodName, x.Queue, x.Exception, Status = x.HangfireJobId == null && x.Exception == null ? OutboxJobStatus.Pending : x.HangfireJobId != null ? OutboxJobStatus.Processed : OutboxJobStatus.Error })
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