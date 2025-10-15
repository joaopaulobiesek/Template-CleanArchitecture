using FluentValidation;

namespace Template.Application.Domains.V1.ExternalServices.AzureBlobStorage.Commands.GenerateSasToken;

public class GenerateSasTokenCommandValidator : AbstractValidator<GenerateSasTokenCommand>
{
    public GenerateSasTokenCommandValidator()
    {
      
    }
}