using AutoMapper;
using LibrarySystem.Application.DTOs.Patrons;
using LibrarySystem.Application.Interfaces;
using MediatR;

namespace LibrarySystem.Application.Queries.Patrons;

public record GetAllPatronsWithDeletedQuery() : IRequest<List<PatronAdminResponse>>;

public class GetAllPatronsWithDeletedHandler : IRequestHandler<GetAllPatronsWithDeletedQuery, List<PatronAdminResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllPatronsWithDeletedHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<PatronAdminResponse>> Handle(GetAllPatronsWithDeletedQuery request, CancellationToken ct)
    {
        var patrons = await _unitOfWork.Patrons.GetAllWithDeletedAsync();
        var allRecords = await _unitOfWork.BorrowRecords.GetAllAsync();

        var result = _mapper.Map<List<PatronAdminResponse>>(patrons);

        foreach (var p in result)
        {
            p.ActiveLoansCount = allRecords.Count(r => r.PatronId == p.Id && r.ReturnDate == null);
        }

        return result;
    }
}
