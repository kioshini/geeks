using FluentValidation;
using TMKMiniApp.Models.DTOs;

namespace TMKMiniApp.Validators
{
    /// <summary>
    /// Validator for CreateOrderDto
    /// </summary>
    public class CreateOrderDtoValidator : AbstractValidator<CreateOrderDto>
    {
        public CreateOrderDtoValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("ID пользователя должен быть больше 0");

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("Имя обязательно")
                .MaximumLength(100).WithMessage("Имя не должно превышать 100 символов")
                .Matches("^[а-яА-ЯёЁa-zA-Z\\s-]+$").WithMessage("Имя может содержать только буквы, пробелы и дефисы");

            RuleFor(x => x.LastName)
                .MaximumLength(100).WithMessage("Фамилия не должна превышать 100 символов")
                .Matches("^[а-яА-ЯёЁa-zA-Z\\s-]*$").WithMessage("Фамилия может содержать только буквы, пробелы и дефисы");

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Телефон обязателен")
                .Matches("^[+]?[0-9\\s\\-\\(\\)]{10,20}$").WithMessage("Неверный формат телефона");

            RuleFor(x => x.Email)
                .EmailAddress().When(x => !string.IsNullOrEmpty(x.Email))
                .WithMessage("Неверный формат email")
                .MaximumLength(255).WithMessage("Email не должен превышать 255 символов");

            RuleFor(x => x.Items)
                .NotEmpty().WithMessage("Список товаров не может быть пустым")
                .Must(items => items != null && items.Count > 0)
                .WithMessage("Заказ должен содержать хотя бы один товар");

            RuleForEach(x => x.Items)
                .ChildRules(item =>
                {
                    item.RuleFor(i => i.ProductId)
                        .GreaterThan(0).WithMessage("ID продукта должен быть больше 0");
                    
                    item.RuleFor(i => i.Quantity)
                        .GreaterThan(0).WithMessage("Количество должно быть больше 0")
                        .LessThan(10000).WithMessage("Количество не должно превышать 10,000");
                });

            RuleFor(x => x.Notes)
                .MaximumLength(1000).WithMessage("Комментарий не должен превышать 1000 символов");
        }
    }
}
