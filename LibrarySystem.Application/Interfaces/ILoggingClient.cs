namespace LibrarySystem.Application.Interfaces
{
    public interface ILoggingClient
    // Proxy Pattern
    {
        Task LogAsync(string message, string createdBy);
    }
}
