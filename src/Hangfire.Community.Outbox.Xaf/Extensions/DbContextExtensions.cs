namespace Hangfire.Community.Outbox.Xaf.Extensions;

using System.Linq.Expressions;
using DevExpress.Xpo;
using Entities;

public static class DbContextExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="session"></param>
    /// <param name="methodCall"></param>
    /// <param name="queue"></param>
    /// <typeparam name="T"></typeparam>
    public static void EnqueueOutbox<T>(this UnitOfWork session, Expression<Func<T, Task>> methodCall, string queue = null)
    {
        session.Save(OutboxJob.Create(methodCall, queue));
    }
    
    public static void EnqueueOutbox<T>(this UnitOfWork session, Expression<Action<T>> methodCall, string queue = null)
    {
        session.Save(OutboxJob.Create(methodCall, queue));
    }
    
    public static void EnqueueOutbox(this UnitOfWork session, Expression<Func<Task>> methodCall, string queue = null)
    {
        session.Save(OutboxJob.Create(methodCall, queue));
    }
    
    public static void EnqueueOutbox(this UnitOfWork session, Expression<Action> methodCall, string queue = null)
    {
        session.Save(OutboxJob.Create(methodCall, queue));
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="session"></param>
    /// <param name="methodCall"></param>
    /// <param name="enqueueAt"></param>
    /// <param name="queue"></param>
    /// <typeparam name="T"></typeparam>
    public static void ScheduleOutbox<T>(this UnitOfWork session, Expression<Func<T, Task>> methodCall, DateTimeOffset enqueueAt, string queue = null)
    {
        session.Save(OutboxJob.Create(methodCall, enqueueAt, queue));
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="session"></param>
    /// <param name="methodCall"></param>
    /// <param name="delay"></param>
    /// <param name="queue"></param>
    /// <typeparam name="T"></typeparam>
    public static void ScheduleOutbox<T>(this UnitOfWork session, Expression<Func<T, Task>> methodCall, TimeSpan delay, string queue = null)
    {
        session.Save(OutboxJob.Create(methodCall, delay, queue));
    }
    
    public static void ScheduleOutbox(this UnitOfWork session, Expression<Func<Task>> methodCall, DateTimeOffset enqueueAt, string queue = null)
    {
        session.Save(OutboxJob.Create(methodCall, enqueueAt, queue));
    }
    
    public static void ScheduleOutbox(this UnitOfWork session, Expression<Func<Task>> methodCall, TimeSpan delay, string queue = null)
    {
        session.Save(OutboxJob.Create(methodCall, delay, queue));
    }
    
    public static void ScheduleOutbox(this UnitOfWork session, Expression<Action> methodCall, DateTimeOffset enqueueAt, string queue = null)
    {
        session.Save(OutboxJob.Create(methodCall, enqueueAt, queue));
    }
    
    public static void ScheduleOutbox(this UnitOfWork session, Expression<Action> methodCall, TimeSpan delay, string queue = null)
    {
        session.Save(OutboxJob.Create(methodCall, delay, queue));
    }
    
    public static void ScheduleOutbox<T>(this UnitOfWork session, Expression<Action<T>> methodCall, DateTimeOffset enqueueAt, string queue = null)
    {
        session.Save(OutboxJob.Create(methodCall, enqueueAt, queue));
    }
    
    public static void ScheduleOutbox<T>(this UnitOfWork session, Expression<Action<T>> methodCall, TimeSpan delay, string queue = null)
    {
        session.Save(OutboxJob.Create(methodCall, delay, queue));
    }
}