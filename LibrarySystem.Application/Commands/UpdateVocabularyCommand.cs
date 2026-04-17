using LibrarySystem.Application.Interfaces;
using LibrarySystem.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public UpdateVocabularyCommandHandler(IGenericRepository<Vocabulary> repository)
        {
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
            vocabulary.Prefix = request.Prefix;
            vocabulary.NamespaceUri = request.NamespaceUri;
            vocabulary.Label = request.Label;

            // 3. Save changes
            _repository.Update(vocabulary);
            await _repository.SaveChangesAsync();

            return true;
        }
    }
}
