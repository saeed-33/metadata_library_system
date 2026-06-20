using AutoMapper;
using LibrarySystem.Application.DTOs.Circulation;
using LibrarySystem.Application.Interfaces;
using MediatR;

namespace LibrarySystem.Application.Queries.Circulation;

public record GetActiveLoansQuery() : IRequest<List<ActiveLoanDto>>;

public class GetActiveLoansHandler : IRequestHandler<GetActiveLoansQuery, List<ActiveLoanDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetActiveLoansHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<ActiveLoanDto>> Handle(GetActiveLoansQuery request, CancellationToken ct)
    {
        var records = await _unitOfWork.BorrowRecords.FindAsync(
            r => r.ReturnDate == null,
            r => r.Copy,
            r => r.Patron);

        return _mapper.Map<List<ActiveLoanDto>>(records);
    }
}
