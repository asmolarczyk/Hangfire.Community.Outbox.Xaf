using System.Linq.Expressions;
using Hangfire.Outbox.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hangfire.Outbox.Extensions;

public static class IBackgroundJobClientExtensions
{
    public static void EnqueueOutbox<T>(this IBackgroundJobClient _, Expression<Func<T, Task>> methodCall, DbContext dbContext, string queue = null)
    {
        dbContext.Set<OutboxJob>().Add(OutboxJob.Create(methodCall, queue));
    }
    
    public static void EnqueueOutbox<T>(this IBackgroundJobClient _, Expression<Action<T>> methodCall, DbContext dbContext, string queue = null)
    {
        dbContext.Set<OutboxJob>().Add(OutboxJob.Create(methodCall, queue));
    }
    
    public static void EnqueueOutbox(this IBackgroundJobClient _, Expression<Func<Task>> methodCall, DbContext dbContext, string queue = null)
    {
        dbContext.Set<OutboxJob>().Add(OutboxJob.Create(methodCall, queue));
    }
    
    public static void EnqueueOutbox(this IBackgroundJobClient _, Expression<Action> methodCall, DbContext dbContext, string queue = null)
    {
        dbContext.Set<OutboxJob>().Add(OutboxJob.Create(methodCall, queue));
    }
    
    public static void ScheduleOutbox<T>(this IBackgroundJobClient _, Expression<Func<T, Task>> methodCall, DateTimeOffset enqueueAt, DbContext dbContext, string queue = null)
    {
        dbContext.Set<OutboxJob>().Add(OutboxJob.Create(methodCall, enqueueAt, queue));
    }
    
    public static void ScheduleOutbox<T>(this IBackgroundJobClient _, Expression<Func<T, Task>> methodCall, TimeSpan delay, DbContext dbContext, string queue = null)
    {
        dbContext.Set<OutboxJob>().Add(OutboxJob.Create(methodCall, delay, queue));
    }
    
    public static void ScheduleOutbox(this IBackgroundJobClient _, Expression<Func<Task>> methodCall, DateTimeOffset enqueueAt, DbContext dbContext, string queue = null)
    {
        dbContext.Set<OutboxJob>().Add(OutboxJob.Create(methodCall, enqueueAt, queue));
    }
    
    public static void ScheduleOutbox(this IBackgroundJobClient _, Expression<Func<Task>> methodCall, TimeSpan delay, DbContext dbContext, string queue = null)
    {
        dbContext.Set<OutboxJob>().Add(OutboxJob.Create(methodCall, delay, queue));
    }
    
    public static void ScheduleOutbox(this IBackgroundJobClient _, Expression<Action> methodCall, DateTimeOffset enqueueAt, DbContext dbContext, string queue = null)
    {
        dbContext.Set<OutboxJob>().Add(OutboxJob.Create(methodCall, enqueueAt, queue));
    }
    
    public static void ScheduleOutbox(this IBackgroundJobClient _, Expression<Action> methodCall, TimeSpan delay, DbContext dbContext, string queue = null)
    {
        dbContext.Set<OutboxJob>().Add(OutboxJob.Create(methodCall, delay, queue));
    }
    
    public static void ScheduleOutbox<T>(this IBackgroundJobClient _, Expression<Action<T>> methodCall, DateTimeOffset enqueueAt, DbContext dbContext, string queue = null)
    {
        dbContext.Set<OutboxJob>().Add(OutboxJob.Create(methodCall, enqueueAt, queue));
    }
    
    public static void ScheduleOutbox<T>(this IBackgroundJobClient _, Expression<Action<T>> methodCall, TimeSpan delay, DbContext dbContext, string queue = null)
    {
        dbContext.Set<OutboxJob>().Add(OutboxJob.Create(methodCall, delay, queue));
    }
}