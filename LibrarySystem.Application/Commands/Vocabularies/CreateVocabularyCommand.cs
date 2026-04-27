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
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public CreateVocabularyCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<int> Handle(CreateVocabularyCommand request, CancellationToken cancellationToken)
        {
            var vocabulary = _mapper.Map<Vocabulary>(request);

            await _unitOfWork.Vocabularies.AddAsync(vocabulary);
            await _unitOfWork.SaveChangesAsync(); 

            return vocabulary.Id;
        }
    }
}
