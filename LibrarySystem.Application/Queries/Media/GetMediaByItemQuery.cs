using AutoMapper;
using LibrarySystem.Application.DTOs.Media;
using LibrarySystem.Application.Interfaces;
using MediatR;

namespace LibrarySystem.Application.Queries.Media;

public record GetMediaByItemQuery(int ItemId) : IRequest<IEnumerable<MediaResponse>>;

public class GetMediaByItemQueryHandler : IRequestHandler<GetMediaByItemQuery, IEnumerable<MediaResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetMediaByItemQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<MediaResponse>> Handle(GetMediaByItemQuery request, CancellationToken cancellationToken)
    {
        var medias = await _unitOfWork.Medias.FindAsync(
            m => m.ItemId == request.ItemId,
            m => m.Values // جلب القيم الوصفية للملف
        );

        return _mapper.Map<IEnumerable<MediaResponse>>(medias);
    }
}