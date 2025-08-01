using FluentValidation;
using TrilhaApiDesafio.DTOs;

namespace TrilhaApiDesafio.Validators
{
    public class TarefaRequestValidator : AbstractValidator<TarefaRequestDto>
    {
        public TarefaRequestValidator()
        {
            RuleFor(x => x.Titulo)
                .NotEmpty()
                .WithMessage("O título é obrigatório")
                .MaximumLength(200)
                .WithMessage("O título deve ter no máximo 200 caracteres");

            RuleFor(x => x.Descricao)
                .MaximumLength(1000)
                .WithMessage("A descrição deve ter no máximo 1000 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Descricao));

            RuleFor(x => x.Data)
                .NotEmpty()
                .WithMessage("A data é obrigatória")
                .Must(BeAValidDate)
                .WithMessage("A data deve ser uma data válida")
                .GreaterThanOrEqualTo(DateTime.Today.AddDays(-365))
                .WithMessage("A data não pode ser mais de 1 ano no passado")
                .LessThanOrEqualTo(DateTime.Today.AddYears(5))
                .WithMessage("A data não pode ser mais de 5 anos no futuro");

            RuleFor(x => x.Status)
                .IsInEnum()
                .WithMessage("Status deve ser Pendente (0) ou Finalizado (1)");
        }

        private bool BeAValidDate(DateTime date)
        {
            return date != default && date != DateTime.MinValue;
        }
    }
}
