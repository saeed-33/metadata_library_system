using MediatR;
using LibrarySystem.Application.Interfaces;
using AutoMapper;
using LibrarySystem.Application.DTOs.Patrons;

namespace LibrarySystem.Application.Queries.Patrons;
public record GetPatronsQuery(string? Search = null) : IRequest<List<PatronResponse>>;

public class GetPatronsHandler : IRequestHandler<GetPatronsQuery, List<PatronResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetPatronsHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<PatronResponse>> Handle(GetPatronsQuery request, CancellationToken ct)
    {
        var patrons = await _unitOfWork.Patrons.GetAllAsync();
        var allRecords = await _unitOfWork.BorrowRecords.GetAllAsync();

        var result = _mapper.Map<List<PatronResponse>>(patrons);

        foreach (var p in result)
        {
            // حساب الإعارات النشطة لكل مستعير
            p.ActiveLoansCount = allRecords.Count(r => r.PatronId == p.Id && r.ReturnDate == null);
        }

        if (!string.IsNullOrEmpty(request.Search))
        {
            result = result.Where(p => p.FullName.Contains(request.Search) || p.NationalId.Contains(request.Search)).ToList();
        }

        return result;
    }
}