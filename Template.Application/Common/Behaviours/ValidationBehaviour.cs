using Template.Application.Common.Models;
using FluentValidation;

namespace Template.Application.Common.Behaviours;
public class ValidationBehaviour<TRequest, TResponse> where TResponse : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }
    /// <summary>
    /// A lógica por trás do uso do 422 para erros de validação é que, embora a estrutura da solicitação esteja correta (a API entendeu o pedido), os dados enviados não atendem aos critérios de validação esperados, tornando-os "inprocessáveis".
    /// </summary>
    /// <param name="executeCore"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<ApiResponse<TResponse>> Handle(Func<Task<ApiResponse<TResponse>>> executeCore, TRequest request, CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(
                _validators.Select(v =>
                    v.ValidateAsync(context, cancellationToken)));

            var failures = validationResults
                .Where(r => r.Errors.Any())
                .SelectMany(r => r.Errors)
                .ToList();

            if (failures.Any())
            {
                var errors = failures
                    .Select(f => new NotificationError(f.PropertyName, f.ErrorMessage))
                    .ToList();

                return new ErrorResponse<TResponse>("Erro de validação", 422, default, errors);
            }
        }

        return await executeCore();
    }
}