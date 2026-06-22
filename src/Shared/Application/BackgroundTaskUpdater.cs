namespace NatrixServices.Shared.Application;

public class BackgroundTaskUpdater(IServiceProvider serviceProvider, ILogger<BackgroundTaskUpdater> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();

        var backgroundTasks = scope.ServiceProvider.GetServices<IBackgroundTask>();

        var tasks = backgroundTasks.Select(t => RunBackgroundTask(t.GetType(), cancellationToken));

        await Task.WhenAll(tasks);
    }

    private async Task RunBackgroundTask(Type taskType, CancellationToken cancellationToken)
    {
        TimeSpan updateInterval = TimeSpan.FromSeconds(1);

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                using var scope = serviceProvider.CreateScope();

                var task = (IBackgroundTask)scope.ServiceProvider.GetRequiredService(taskType);

                updateInterval = task.UpdateInterval;

                await task.ExecuteAsync();
            }
            catch (Exception exception)
            {
                logger.LogError(exception, $"Error occured while executing {taskType.Name}");
            }

            await Task.Delay(updateInterval, cancellationToken);
        }
    }
}