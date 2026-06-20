using AutoMapper;
using LibrarySystem.Application.DTOs.BorrowRecords;
using LibrarySystem.Application.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LibrarySystem.Application.Queries.Patrons
{
  public record GetOverdueLoansQuery() : IRequest<List<BorrowRecordResponse>>;

public class GetOverdueLoansHandler : IRequestHandler<GetOverdueLoansQuery, List<BorrowRecordResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetOverdueLoansHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<BorrowRecordResponse>> Handle(GetOverdueLoansQuery request, CancellationToken ct)
    {
        var records = await _unitOfWork.BorrowRecords.GetAllAsync();
        return _mapper.Map<List<BorrowRecordResponse>>(records.Where(r => r.ReturnDate == null && r.DueDate < DateTime.UtcNow).ToList());
    }
}
}
