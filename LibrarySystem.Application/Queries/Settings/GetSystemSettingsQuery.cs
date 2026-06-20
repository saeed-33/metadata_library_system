using AutoMapper;
using LibrarySystem.Application.DTOs.SystemSettings;
using LibrarySystem.Application.Interfaces;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LibrarySystem.Application.Queries.SystemSettings;

public record GetSystemSettingsQuery : IRequest<List<SystemSettingResponse>>;

public class GetSystemSettingsQueryHandler : IRequestHandler<GetSystemSettingsQuery, List<SystemSettingResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetSystemSettingsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<SystemSettingResponse>> Handle(GetSystemSettingsQuery request, CancellationToken cancellationToken)
    {
        var settings = await _unitOfWork.SystemSettings.GetAllAsync();
        return _mapper.Map<List<SystemSettingResponse>>(settings);
    }
}
