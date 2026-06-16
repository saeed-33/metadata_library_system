using LibrarySystem.Application.Interfaces;
using MediatR;

namespace LibrarySystem.Application.Commands.Vocabularies; // Adjust namespace as needed

public record UndeleteVocabularyCommand(int Id) : IRequest<bool>;

public class UndeleteVocabularyCommandHandler : IRequestHandler<UndeleteVocabularyCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UndeleteVocabularyCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<bool> Handle(UndeleteVocabularyCommand request, CancellationToken cancellationToken)
    {
        var vocabularies = await _unitOfWork.Vocabularies.FindWithDeletedAsync(v => v.Id == request.Id);
        var vocabulary = vocabularies.FirstOrDefault();

        if (vocabulary == null) return false;

        vocabulary.IsDeleted = false;
        vocabulary.DeletedAt = null;

        _unitOfWork.Vocabularies.Update(vocabulary);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}