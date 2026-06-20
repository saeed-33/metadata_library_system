using AutoMapper;
using LibrarySystem.Application.DTOs.Patrons;
using LibrarySystem.Application.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace LibrarySystem.Application.Queries.Patrons;

public record GetPatronByIdQuery(int Id) : IRequest<PatronResponse?>;

public class GetPatronByIdQueryHandler : IRequestHandler<GetPatronByIdQuery, PatronResponse?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetPatronByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PatronResponse?> Handle(GetPatronByIdQuery request, CancellationToken cancellationToken)
    {
        var patron = await _unitOfWork.Patrons.GetByIdAsync(request.Id);
        return _mapper.Map<PatronResponse?>(patron);
    }
}
