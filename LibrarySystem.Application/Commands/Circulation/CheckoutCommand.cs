using MediatR;

namespace LibrarySystem.Application.Commands.Features
{
    // الطلب الذي سيرسله الـ API
    public record CheckoutCommand(
        string Barcode, 
        int PatronId, 
        DateTime? CustomDueDate = null
    ) : IRequest<bool>;
}