using AutoMapper;
using LibrarySystem.Application.Interfaces;
using LibrarySystem.Domain.Entities;
using MediatR;

namespace LibrarySystem.Application.Commands
{
    public record UpdateVocabularyCommand(
        int Id,
        string Prefix,
        string NamespaceUri,
        string Label
    ) : IRequest<bool>; // Returns true if successful, false if not found

    // 2. The Handler
    public class UpdateVocabularyCommandHandler : IRequestHandler<UpdateVocabularyCommand, bool>
    {
        private readonly IGenericRepository<Vocabulary> _repository;
        private readonly IMapper _mapper;
        public UpdateVocabularyCommandHandler(IGenericRepository<Vocabulary> repository, IMapper mapper)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public async Task<bool> Handle(UpdateVocabularyCommand request, CancellationToken cancellationToken)
        {
            // 1. Fetch the existing entity from the database
            var vocabulary = await _repository.GetByIdAsync(request.Id);

            if (vocabulary == null)
            {
                return false; // Or throw a NotFoundException
            }

            // 2. Update the properties
            _mapper.Map(request, vocabulary);

            // 3. Save changes
            _repository.Update(vocabulary);
            await _repository.SaveChangesAsync();

            return true;
        }
    }
}
