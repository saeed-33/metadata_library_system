using AutoMapper;
using LibrarySystem.Application.Interfaces;
using LibrarySystem.Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LibrarySystem.Application.Commands.Patrons;

public record UpdatePatronCommand(int Id, string FullName, string PhoneNumber, string? Email) : IRequest<bool>;

public class UpdatePatronHandler : IRequestHandler<UpdatePatronCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdatePatronHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<bool> Handle(UpdatePatronCommand request, CancellationToken ct)
    {
        var patron = await _unitOfWork.Patrons.GetByIdAsync(request.Id);
        if (patron == null) return false;

        _mapper.Map(request, patron);

        _unitOfWork.Patrons.Update(patron);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
