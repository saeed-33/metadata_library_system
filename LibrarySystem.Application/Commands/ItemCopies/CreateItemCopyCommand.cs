using AutoMapper;
using LibrarySystem.Application.Interfaces;
using LibrarySystem.Domain.Entities;
using LibrarySystem.Domain.Enums;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LibrarySystem.Application.Commands.ItemCopies
{
    public record CreateItemCopyCommand(int ItemId, string Barcode, string? Notes) : IRequest<int>;

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
            var item = await _unitOfWork.Items.GetByIdAsync(request.ItemId);
            if (item == null) return 0;

            if (!item.TemplateId.HasValue)
                throw new InvalidOperationException("هذا الكتاب غير مرتبط بقالب مصادر.");

            var template = await _unitOfWork.ResourceTemplates.GetByIdAsync(item.TemplateId.Value);

            var copy = _mapper.Map<ItemCopy>(request);
            // إذا كان القالب لا يسمح بالإعارة، النسخة تكون مراجع فقط وإلا متاحة
            copy.Status = (template != null && !template.IsBorrowable) ? ItemCopyStatus.ReferenceOnly : ItemCopyStatus.Available;

            await _unitOfWork.ItemCopies.AddAsync(copy);
            await _unitOfWork.SaveChangesAsync();
            return copy.Id;
        }
    }
}
