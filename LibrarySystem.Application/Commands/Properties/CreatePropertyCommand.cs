using LibrarySystem.Application.Interfaces;
using LibrarySystem.Domain.Entities;
using MediatR;
using AutoMapper;

namespace LibrarySystem.Application.Commands.Properties;

public record CreatePropertyCommand(
    int VocabularyId,
    string LocalName,
    string Label,
    string TermUri,
     bool IsSearchable = false
) : IRequest<int>;

public class CreatePropertyCommandHandler : IRequestHandler<CreatePropertyCommand, int>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreatePropertyCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<int> Handle(CreatePropertyCommand request, CancellationToken cancellationToken)
    {
        var property = _mapper.Map<Property>(request);

        await _unitOfWork.Properties.AddAsync(property);

        // 3. الحفظ الفعلي (هنا يتم تفعيل الـ Audit في الـ DbContext وتوليد الـ ID)
        await _unitOfWork.SaveChangesAsync();

        return property.Id;
    }
}