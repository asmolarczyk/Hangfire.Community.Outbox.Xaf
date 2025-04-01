namespace Hangfire.Community.Outbox.Xaf.Entities;

using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using DevExpress.Xpo;
using Extensions;
using Hangfire.Common;

[Persistent]
public class OutboxJob : INotifyPropertyChanged
{
    private OutboxJob()
    { }

    private static OutboxJob Build(Job job, string queue)
    {
        return new OutboxJob
        {
            JobType = job.Type.GetFriendlyName(),
            ArgumentTypesJson =
                JsonSerializer.Serialize(job.Args.Select(a => a.GetType().GetFriendlyName())),
            ArgumentValuesJson = JsonSerializer.Serialize(job.Args),
            MethodName = job.Method.Name,
            CreatedOn = DateTime.UtcNow,
            Queue = queue
        };
    }
    
    private static OutboxJob Build(Job job, DateTimeOffset enqueueAt, string queue)
    {
        return new OutboxJob
        {
            JobType = job.Type.GetFriendlyName(),
            ArgumentTypesJson =
                JsonSerializer.Serialize(job.Args.Select(a => a.GetType().GetFriendlyName())),
            ArgumentValuesJson = JsonSerializer.Serialize(job.Args),
            MethodName = job.Method.Name,
            CreatedOn = DateTime.UtcNow,
            EnqueueAt = enqueueAt,
            Queue = queue
        };
    }
    
    private static OutboxJob Build(Job job, TimeSpan delay, string queue)
    {
        return new OutboxJob
        {
            JobType = job.Type.GetFriendlyName(),
            ArgumentTypesJson =
                JsonSerializer.Serialize(job.Args.Select(a => a.GetType().GetFriendlyName())),
            ArgumentValuesJson = JsonSerializer.Serialize(job.Args),
            MethodName = job.Method.Name,
            CreatedOn = DateTime.UtcNow,
            Delay = delay,
            Queue = queue
        };
    }
    
    public static OutboxJob Create<T>(Expression<Func<T, Task>> expression, string queue = null)
    {
        return Build(Job.FromExpression(expression, queue), queue);
    }
    
    public static OutboxJob Create(Expression<Func<Task>> expression, string queue = null)
    {
        return Build(Job.FromExpression(expression, queue), queue);
    }
    
    public static OutboxJob Create<T>(Expression<Action<T>> expression, string queue = null)
    {
        return Build(Job.FromExpression(expression, queue), queue);
    }
    
    public static OutboxJob Create(Expression<Action> expression, string queue = null)
    {
        return Build(Job.FromExpression(expression, queue), queue);
    }
    
    public static OutboxJob Create<T>(Expression<Func<T, Task>> expression, DateTimeOffset enqueueAt, string queue = null)
    {
        return Build(Job.FromExpression(expression, queue), enqueueAt, queue);
    }
    
    public static OutboxJob Create(Expression<Func<Task>> expression, DateTimeOffset enqueueAt, string queue = null)
    {
        return Build(Job.FromExpression(expression, queue), enqueueAt, queue);
    }
    
    public static OutboxJob Create<T>(Expression<Action<T>> expression, DateTimeOffset enqueueAt, string queue = null)
    {
        return Build(Job.FromExpression(expression, queue), enqueueAt, queue);
    }
    
    public static OutboxJob Create(Expression<Action> expression, DateTimeOffset enqueueAt, string queue = null)
    {
        return Build(Job.FromExpression(expression, queue), enqueueAt, queue);
    }
    
    public static OutboxJob Create<T>(Expression<Func<T, Task>> expression, TimeSpan delay, string queue = null)
    {
        return Build(Job.FromExpression(expression, queue), delay, queue);
    }
    
    public static OutboxJob Create(Expression<Func<Task>> expression, TimeSpan delay, string queue = null)
    {
        return Build(Job.FromExpression(expression, queue), delay, queue);
    }
    
    public static OutboxJob Create<T>(Expression<Action<T>> expression, TimeSpan delay, string queue = null)
    {
        return Build(Job.FromExpression(expression, queue), delay, queue);
    }
    
    public static OutboxJob Create(Expression<Action> expression, TimeSpan delay, string queue = null)
    {
        return Build(Job.FromExpression(expression, queue), delay, queue);
    }

    [Key(AutoGenerate = false)]
    public Guid Oid { get; init; } = Guid.NewGuid();

    public string JobType { get; init; }

    public string MethodName { get; init; }

    [Size(1000)]
    [Persistent]
    private string ArgumentTypesJson { get; init; }

    [Size(SizeAttribute.Unlimited)]
    [Persistent]
    private string ArgumentValuesJson { get; init; }

    public string? Queue { get; init; }

    private string? hangfireJobId;

    public string? HangfireJobId
    {
        get => hangfireJobId;
        set => SetField(ref hangfireJobId, value);
    }

    public DateTimeOffset? EnqueueAt { get; init; }

    public TimeSpan? Delay { get; init; }

    
    public IEnumerable<object> GetArguments()
    {
        var argumentTypeStrings = JsonSerializer.Deserialize<string[]>(ArgumentTypesJson);
        var types = argumentTypeStrings.Select(t => Type.GetType(t)).ToArray();

        if (types.Any(t => t == null))
        {
            var notFound = types.Select((type, index) => new { type, index }).Where(x => x.type == null).Select(x => x.index);
            var unresolvedTypes = string.Join("\\r\\n", notFound.Select(i => argumentTypeStrings.ElementAt(i)));
            
            throw new InvalidOperationException($"Could not resolve serialized argument types:\\r\\n{unresolvedTypes}");
        }
        
        var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(ArgumentValuesJson));
        var element = JsonElement.ParseValue(ref reader);
        var elements = element.EnumerateArray();
        
        for (int i = 0; i < elements.Count(); i++)
        {
            yield return elements.ElementAt(i).Deserialize(types[i]);
        }
    }

    public MethodInfo GetMethod()
    {
        var argTypes = GetArguments().Select(x => x.GetType()).ToArray();
        return GetJobType().GetMethod(MethodName, argTypes);
    }

    public Type GetJobType()
    {
        var jobType = Type.GetType(JobType);

        if (jobType == null)
        {
            throw new InvalidOperationException($"Could not resolve serialized job type '{JobType}'");
        }

        return jobType;
    }
    
    public DateTimeOffset CreatedOn { get; init; }

    private bool processed;

    public bool Processed
    {
        get => processed;
        set => SetField(ref processed, value);
    }

    private string? exception;
    
    [Size(SizeAttribute.Unlimited)]
    public string? Exception
    {
        get => exception;
        set => SetField(ref exception, value);
    }


    //public class OutboxJobEntityTypeConfiguration : IEntityTypeConfiguration<OutboxJob>
    //{
    //    public void Configure(EntityTypeBuilder<OutboxJob> builder)
    //    {
    //        builder.ToTable(HangfireOutboxStaticOptions.Options.OutboxJobTableName, HangfireOutboxStaticOptions.Options.OutboxJobSchema);
    //
    //        builder.Property(x => x.JobType).IsRequired();
    //        builder.Property(x => x.Exception).IsRequired(false);
    //        builder.Property(x => x.ArgumentTypesJson).IsRequired();
    //        builder.Property(x => x.ArgumentValuesJson).IsRequired();
    //        builder.Property(x => x.Queue).IsRequired(false);
    //        builder.Property(x => x.MethodName).IsRequired();
    //        builder.Property(x => x.EnqueueAt).IsRequired(false);
    //        builder.Property(x => x.Delay).IsRequired(false);
    //
    //        builder.HasIndex(x => x.Processed).HasDatabaseName("IDX_Processed");
    //        builder.HasIndex(x => x.CreatedOn).HasDatabaseName("IDX_CreatedOn");
    //    }
    //}
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}