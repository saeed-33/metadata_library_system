using AutoMapper;
using LibrarySystem.Application.DTOs.Media;
using LibrarySystem.Application.Interfaces;
using MediatR;

namespace LibrarySystem.Application.Queries.Media;

public record GetMediaByIdQuery(int Id) : IRequest<MediaResponse?>;

public class GetMediaByIdQueryHandler : IRequestHandler<GetMediaByIdQuery, MediaResponse?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetMediaByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<MediaResponse?> Handle(GetMediaByIdQuery request, CancellationToken cancellationToken)
    {
        // Using FindAsync instead of GetByIdAsync because we need
        // to Include Values — GetByIdAsync does a simple Find with no includes
        var results = await _unitOfWork.Medias.FindAsync(
            m => m.Id == request.Id,
            m => m.Values
        );

        var media = results.FirstOrDefault();
        return media == null ? null : _mapper.Map<MediaResponse>(media);
    }
}