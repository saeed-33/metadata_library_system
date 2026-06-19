using LibrarySystem.Application.Interfaces;
using LibrarySystem.Domain.entities;
using LibrarySystem.Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LibrarySystem.Application.Commands.Features
{
  public record GetOverdueLoansQuery() : IRequest<List<BorrowRecord>>;

public class GetOverdueLoansHandler : IRequestHandler<GetOverdueLoansQuery, List<BorrowRecord>>
{
    private readonly IUnitOfWork _unitOfWork;
    public GetOverdueLoansHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<List<BorrowRecord>> Handle(GetOverdueLoansQuery request, CancellationToken ct)
    {
        var records = await _unitOfWork.BorrowRecords.GetAllAsync();
        return records.Where(r => r.ReturnDate == null && r.DueDate < DateTime.UtcNow).ToList();
    }
}
}