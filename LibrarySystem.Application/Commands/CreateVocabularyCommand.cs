using LibrarySystem.Application.Interfaces;
using LibrarySystem.Domain.Entities;
using MediatR;
using AutoMapper;

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
        private readonly IMapper _mapper;
        public CreateVocabularyCommandHandler(IGenericRepository<Vocabulary> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<int> Handle(CreateVocabularyCommand request, CancellationToken cancellationToken)
        {
            var vocabulary = _mapper.Map<Vocabulary>(request);

            await _repository.AddAsync(vocabulary);
            await _repository.SaveChangesAsync();

            return vocabulary.Id;
        }
    }
}
