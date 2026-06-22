namespace NatrixServices.Shared.Application;

public interface IBackgroundTask
{
    public TimeSpan UpdateInterval { get; }
    public Task ExecuteAsync();
}