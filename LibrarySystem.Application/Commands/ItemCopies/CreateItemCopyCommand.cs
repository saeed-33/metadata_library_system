using AutoMapper;
using LibrarySystem.Application.Interfaces;
using LibrarySystem.Domain.Entities;
using LibrarySystem.Domain.Enums;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using LibrarySystem.Application.DTOs.ItemCopies;


namespace LibrarySystem.Application.Commands.ItemCopies
{

    public class CreateItemCopyHandler : IRequestHandler<CreateItemCopyCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateItemCopyHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<int> Handle(CreateItemCopyCommand request, CancellationToken ct)
        {
            // 1. Safely fetch the item (Validation guarantees it exists and has a TemplateId)
            var item = await _unitOfWork.Items.GetByIdAsync(request.ItemId);

            // 2. Fetch the template
            var template = await _unitOfWork.ResourceTemplates.GetByIdAsync(item!.TemplateId!.Value);

            var template = await _unitOfWork.ResourceTemplates.GetByIdAsync(item.TemplateId.Value);

            var copy = _mapper.Map<ItemCopy>(request);
            // إذا كان القالب لا يسمح بالإعارة، النسخة تكون مراجع فقط وإلا متاحة
            copy.Status = (template != null && !template.IsBorrowable) ? ItemCopyStatus.ReferenceOnly : ItemCopyStatus.Available;

            // 5. Save to database
            await _unitOfWork.ItemCopies.AddAsync(copy);
            await _unitOfWork.SaveChangesAsync();

            return copy.Id;
        }
    }
}