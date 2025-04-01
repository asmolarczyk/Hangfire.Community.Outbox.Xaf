namespace Hangfire.Community.Outbox.Xaf.Services;

using DevExpress.ExpressApp;
using Entities;

public class HangfireOutboxModule : ModuleBase
{
    public HangfireOutboxModule()
    {
        AdditionalExportedTypes.Add(
            typeof(OutboxJob)
        );
    }
}