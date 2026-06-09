namespace LibrarySystem.Application.Interfaces
{
    public interface ILoggingClient
    {
        Task LogAsync(string message, string createdBy);
    }
}
