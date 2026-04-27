using AutoMapper;
using LibrarySystem.Application.Interfaces;
using MediatR;

namespace LibrarySystem.Application.Commands.Properties;

public record UpdatePropertyCommand(
    int Id,
    int VocabularyId,
    string LocalName,
    string Label,
    string TermUri
) : IRequest<bool>;

public class UpdatePropertyCommandHandler : IRequestHandler<UpdatePropertyCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdatePropertyCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<bool> Handle(UpdatePropertyCommand request, CancellationToken cancellationToken)
    {
        // 1. البحث عن العنصر
        var property = await _unitOfWork.Properties.GetByIdAsync(request.Id);
        if (property == null) return false;

        // 2. تحديث البيانات (المابير سيقوم بنسخ القيم من request إلى property)
        _mapper.Map(request, property);

        // 3. إبلاغ الـ UnitOfWork بالتحديث والحفظ
        _unitOfWork.Properties.Update(property);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}