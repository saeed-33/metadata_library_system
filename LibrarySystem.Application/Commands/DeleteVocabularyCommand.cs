using LibrarySystem.Application.Interfaces;
using LibrarySystem.Domain.Entities;
using MediatR;

namespace LibrarySystem.Application.Commands
{
    public record DeleteVocabularyCommand(int Id) : IRequest<bool>;

    // 2. The Handler that executes the delete
    public class DeleteVocabularyCommandHandler : IRequestHandler<DeleteVocabularyCommand, bool>
    {
        private readonly IGenericRepository<Vocabulary> _repository;

        public DeleteVocabularyCommandHandler(IGenericRepository<Vocabulary> repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(DeleteVocabularyCommand request, CancellationToken cancellationToken)
        {
            // 1. Find the entity
            var vocabulary = await _repository.GetByIdAsync(request.Id);

            if (vocabulary == null)
            {
                // You can throw a custom NotFoundException here if you have one
                return false;
            }

            // 2. Call Delete. 
            // Because your Generic Repository handles Soft Delete in the DbContext, 
            // you just call Remove/Delete, and EF Core will change IsDeleted to true and set DeletedAt!
            _repository.Delete(vocabulary);
            await _repository.SaveChangesAsync();

            return true;
        }
    }
}
