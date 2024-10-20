using System.Linq.Expressions;
using Hangfire.EfCore.Outbox.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hangfire.EfCore.Outbox.Extensions;

public static class DbContextExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="methodCall"></param>
    /// <param name="queue"></param>
    /// <typeparam name="T"></typeparam>
    public static void EnqueueOutbox<T>(this DbContext dbContext, Expression<Func<T, Task>> methodCall, string queue = null)
    {
        dbContext.Set<OutboxJob>().Add(OutboxJob.Create(methodCall, queue));
    }
    
    public static void EnqueueOutbox<T>(this DbContext dbContext, Expression<Action<T>> methodCall, string queue = null)
    {
        dbContext.Set<OutboxJob>().Add(OutboxJob.Create(methodCall, queue));
    }
    
    public static void EnqueueOutbox(this DbContext dbContext, Expression<Func<Task>> methodCall, string queue = null)
    {
        dbContext.Set<OutboxJob>().Add(OutboxJob.Create(methodCall, queue));
    }
    
    public static void EnqueueOutbox(this DbContext dbContext, Expression<Action> methodCall, string queue = null)
    {
        dbContext.Set<OutboxJob>().Add(OutboxJob.Create(methodCall, queue));
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="methodCall"></param>
    /// <param name="enqueueAt"></param>
    /// <param name="queue"></param>
    /// <typeparam name="T"></typeparam>
    public static void ScheduleOutbox<T>(this DbContext dbContext, Expression<Func<T, Task>> methodCall, DateTimeOffset enqueueAt, string queue = null)
    {
        dbContext.Set<OutboxJob>().Add(OutboxJob.Create(methodCall, enqueueAt, queue));
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="methodCall"></param>
    /// <param name="delay"></param>
    /// <param name="queue"></param>
    /// <typeparam name="T"></typeparam>
    public static void ScheduleOutbox<T>(this DbContext dbContext, Expression<Func<T, Task>> methodCall, TimeSpan delay, string queue = null)
    {
        dbContext.Set<OutboxJob>().Add(OutboxJob.Create(methodCall, delay, queue));
    }
    
    public static void ScheduleOutbox(this DbContext dbContext, Expression<Func<Task>> methodCall, DateTimeOffset enqueueAt, string queue = null)
    {
        dbContext.Set<OutboxJob>().Add(OutboxJob.Create(methodCall, enqueueAt, queue));
    }
    
    public static void ScheduleOutbox(this DbContext dbContext, Expression<Func<Task>> methodCall, TimeSpan delay, string queue = null)
    {
        dbContext.Set<OutboxJob>().Add(OutboxJob.Create(methodCall, delay, queue));
    }
    
    public static void ScheduleOutbox(this DbContext dbContext, Expression<Action> methodCall, DateTimeOffset enqueueAt, string queue = null)
    {
        dbContext.Set<OutboxJob>().Add(OutboxJob.Create(methodCall, enqueueAt, queue));
    }
    
    public static void ScheduleOutbox(this DbContext dbContext, Expression<Action> methodCall, TimeSpan delay, string queue = null)
    {
        dbContext.Set<OutboxJob>().Add(OutboxJob.Create(methodCall, delay, queue));
    }
    
    public static void ScheduleOutbox<T>(this DbContext dbContext, Expression<Action<T>> methodCall, DateTimeOffset enqueueAt, string queue = null)
    {
        dbContext.Set<OutboxJob>().Add(OutboxJob.Create(methodCall, enqueueAt, queue));
    }
    
    public static void ScheduleOutbox<T>(this DbContext dbContext, Expression<Action<T>> methodCall, TimeSpan delay, string queue = null)
    {
        dbContext.Set<OutboxJob>().Add(OutboxJob.Create(methodCall, delay, queue));
    }
}