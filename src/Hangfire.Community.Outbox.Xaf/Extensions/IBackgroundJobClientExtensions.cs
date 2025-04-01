namespace Hangfire.Community.Outbox.Xaf.Extensions;

using System.Linq.Expressions;
using DevExpress.Xpo;
using Entities;

public static class BackgroundJobClientExtensions
{
    public static void EnqueueOutbox<T>(this IBackgroundJobClient _, Expression<Func<T, Task>> methodCall, UnitOfWork session, string queue = null)
    {
        session.Save(OutboxJob.Create(methodCall, queue));
    }
    
    public static void EnqueueOutbox<T>(this IBackgroundJobClient _, Expression<Action<T>> methodCall, UnitOfWork session, string queue = null)
    {
        session.Save(OutboxJob.Create(methodCall, queue));
    }
    
    public static void EnqueueOutbox(this IBackgroundJobClient _, Expression<Func<Task>> methodCall, UnitOfWork session, string queue = null)
    {
        session.Save(OutboxJob.Create(methodCall, queue));
    }
    
    public static void EnqueueOutbox(this IBackgroundJobClient _, Expression<Action> methodCall, UnitOfWork session, string queue = null)
    {
        session.Save(OutboxJob.Create(methodCall, queue));
    }
    
    public static void ScheduleOutbox<T>(this IBackgroundJobClient _, Expression<Func<T, Task>> methodCall, DateTimeOffset enqueueAt, UnitOfWork session, string queue = null)
    {
        session.Save(OutboxJob.Create(methodCall, enqueueAt, queue));
    }
    
    public static void ScheduleOutbox<T>(this IBackgroundJobClient _, Expression<Func<T, Task>> methodCall, TimeSpan delay, UnitOfWork session, string queue = null)
    {
        session.Save(OutboxJob.Create(methodCall, delay, queue));
    }
    
    public static void ScheduleOutbox(this IBackgroundJobClient _, Expression<Func<Task>> methodCall, DateTimeOffset enqueueAt, UnitOfWork session, string queue = null)
    {
        session.Save(OutboxJob.Create(methodCall, enqueueAt, queue));
    }
    
    public static void ScheduleOutbox(this IBackgroundJobClient _, Expression<Func<Task>> methodCall, TimeSpan delay, UnitOfWork session, string queue = null)
    {
        session.Save(OutboxJob.Create(methodCall, delay, queue));
    }
    
    public static void ScheduleOutbox(this IBackgroundJobClient _, Expression<Action> methodCall, DateTimeOffset enqueueAt, UnitOfWork session, string queue = null)
    {
        session.Save(OutboxJob.Create(methodCall, enqueueAt, queue));
    }
    
    public static void ScheduleOutbox(this IBackgroundJobClient _, Expression<Action> methodCall, TimeSpan delay, UnitOfWork session, string queue = null)
    {
        session.Save(OutboxJob.Create(methodCall, delay, queue));
    }
    
    public static void ScheduleOutbox<T>(this IBackgroundJobClient _, Expression<Action<T>> methodCall, DateTimeOffset enqueueAt, UnitOfWork session, string queue = null)
    {
        session.Save(OutboxJob.Create(methodCall, enqueueAt, queue));
    }
    
    public static void ScheduleOutbox<T>(this IBackgroundJobClient _, Expression<Action<T>> methodCall, TimeSpan delay, UnitOfWork session, string queue = null)
    {
        session.Save(OutboxJob.Create(methodCall, delay, queue));
    }
}