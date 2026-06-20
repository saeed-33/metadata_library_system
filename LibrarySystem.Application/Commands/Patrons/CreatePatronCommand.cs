using AutoMapper;
using LibrarySystem.Application.Interfaces;
using LibrarySystem.Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LibrarySystem.Application.Commands.Patrons
{
    public record CreatePatronCommand(string FullName, string NationalId, string PhoneNumber, string? Email) : IRequest<int>;

    public class CreatePatronHandler : IRequestHandler<CreatePatronCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreatePatronHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<int> Handle(CreatePatronCommand request, CancellationToken ct)
        {
            // التحقق من عدم تكرار الرقم الوطني
            var patrons = await _unitOfWork.Patrons.GetAllAsync();
            if (patrons.Any(p => p.NationalId == request.NationalId))
                throw new Exception("المستعير مسجل مسبقاً بهذا الرقم الوطني.");

            var patron = _mapper.Map<Patron>(request);

            await _unitOfWork.Patrons.AddAsync(patron);
            await _unitOfWork.SaveChangesAsync();
            return patron.Id;
        }
    }
}
