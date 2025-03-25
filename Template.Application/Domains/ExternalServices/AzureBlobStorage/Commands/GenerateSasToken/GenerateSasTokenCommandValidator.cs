using FluentValidation;

namespace Template.Application.Domains.ExternalServices.AzureBlobStorage.Commands.GenerateSasToken;

public class GenerateSasTokenCommandValidator : AbstractValidator<GenerateSasTokenCommand>
{
    public GenerateSasTokenCommandValidator()
    {
      
    }
}