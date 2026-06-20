using AutoMapper;
using LibrarySystem.Application.DTOs.Items;
using LibrarySystem.Application.DTOs.Media;
using LibrarySystem.Application.Interfaces;
using LibrarySystem.Domain.Entities;
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

    public async Task<MediaResponse?> Handle(
        GetMediaByIdQuery request, CancellationToken cancellationToken)
    {
        // 1. Fetch media — no Values include
        var results = await _unitOfWork.Medias.FindAsync(m => m.Id == request.Id);
        var media = results.FirstOrDefault();
        if (media == null) return null;

        // 2. Fetch values separately
        var values = await _unitOfWork.Values.FindAsync(
            v => v.ResourceId == media.Id,
            v => v.Property!);

        // 3. Build response via mapper
        var response = _mapper.Map<MediaResponse>(media);
        response.MetadataValues = _mapper.Map<List<ItemValueResponse>>(values);
        return response;
    }
}
