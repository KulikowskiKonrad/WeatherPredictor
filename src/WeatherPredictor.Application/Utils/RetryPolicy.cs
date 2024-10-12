using WeatherPredictor.Application.Exceptions;

namespace WeatherPredictor.Infrastructure.Utils;

public static class RetryPolicy
{
    public static async Task<T> ExecuteWithRetryAsync<T>(Func<Task<T>> action, int maxAttempts = 3,
        int delayMilliseconds = 1000)
    {
        int attempts = 0;

        while (true)
        {
            try
            {
                return await action();
            }
            catch (Exception)
            {
                attempts++;
                if (attempts >= maxAttempts)
                {
                    throw new ReachedMaxRetryAttemptsException();
                }

                await Task.Delay(delayMilliseconds);
            }
        }
    }


    public static async Task ExecuteWithRetryAsync(Func<Task> action, int maxRetryAttempts = 3,
        int delayInMilliseconds = 1000)
    {
        for (int i = 0; i < maxRetryAttempts; i++)
        {
            try
            {
                await action();
                return;
            }
            catch (Exception)
            {
                if (i == maxRetryAttempts - 1)
                {
                    throw new ReachedMaxRetryAttemptsException();
                }

                await Task.Delay(delayInMilliseconds);
            }
        }
    }
}