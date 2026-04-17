using LibrarySystem.Application.Interfaces;
using LibrarySystem.Domain.Entities;
using MediatR;

namespace LibrarySystem.Application.Commands
{
    public record CreateVocabularyCommand(
        string Prefix,
        string NamespaceUri,
        string Label
    ) : IRequest<int>;

    public class CreateVocabularyCommandHandler : IRequestHandler<CreateVocabularyCommand, int>
    {
        private readonly IGenericRepository<Vocabulary> _repository;

        public CreateVocabularyCommandHandler(IGenericRepository<Vocabulary> repository)
        {
            _repository = repository;
        }

        public async Task<int> Handle(CreateVocabularyCommand request, CancellationToken cancellationToken)
        {
            var vocabulary = new Vocabulary
            {
                Prefix = request.Prefix,
                NamespaceUri = request.NamespaceUri,
                Label = request.Label
            };

            await _repository.AddAsync(vocabulary);
            await _repository.SaveChangesAsync();

            return vocabulary.Id;
        }
    }
}
