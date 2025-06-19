namespace MESWebDev.Services
{
    public interface ILoggingService
    {
        void LogAction(string actionName, string actionType, Action action, string createdBy = "System", string additionalDetails = null);

        //Task LogActionAsync(string actionName, string actionType, Func<Task> action, string createdBy = "System", string additionalDetails = null);
        Task<T> LogActionAsync<T>(string actionName, string actionType, Func<Task<T>> action, string createdBy = "System", string additionalDetails = null);
    }
}