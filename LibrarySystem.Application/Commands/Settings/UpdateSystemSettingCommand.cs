using LibrarySystem.Application.Interfaces;
using LibrarySystem.Domain.entities;
using LibrarySystem.Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LibrarySystem.Application.Commands.Features
{
   public record UpdateSystemSettingCommand(string Key, string Value) : IRequest<bool>;

public class UpdateSystemSettingHandler : IRequestHandler<UpdateSystemSettingCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    public UpdateSystemSettingHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<bool> Handle(UpdateSystemSettingCommand request, CancellationToken ct)
    {
        var settings = await _unitOfWork.SystemSettings.GetAllAsync();
        var setting = settings.FirstOrDefault(s => s.Key == request.Key);

        if (setting == null)
        {
            await _unitOfWork.SystemSettings.AddAsync(new SystemSetting { Key = request.Key, Value = request.Value });
        }
        else
        {
            setting.Value = request.Value;
             _unitOfWork.SystemSettings.Update(setting);
        }

        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
}