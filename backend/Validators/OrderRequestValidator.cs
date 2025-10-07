using FluentValidation;
using TMKMiniApp.Models.OrderModels;

namespace TMKMiniApp.Validators
{
    /// <summary>
    /// Валидатор для запроса на создание заказа
    /// </summary>
    public class OrderRequestValidator : AbstractValidator<OrderRequest>
    {
        public OrderRequestValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("Имя обязательно для заполнения")
                .Length(2, 100).WithMessage("Имя должно содержать от 2 до 100 символов")
                .Matches(@"^[а-яА-ЯёЁa-zA-Z\s\-']+$").WithMessage("Имя может содержать только буквы, пробелы, дефисы и апострофы");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Фамилия обязательна для заполнения")
                .Length(2, 100).WithMessage("Фамилия должна содержать от 2 до 100 символов")
                .Matches(@"^[а-яА-ЯёЁa-zA-Z\s\-']+$").WithMessage("Фамилия может содержать только буквы, пробелы, дефисы и апострофы");

            RuleFor(x => x.INN)
                .NotEmpty().WithMessage("ИНН обязателен для заполнения")
                .Matches(@"^\d{10}$|^\d{12}$").WithMessage("ИНН должен содержать 10 или 12 цифр");

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Телефон обязателен для заполнения")
                .Matches(@"^(\+7|8)?[\s\-]?\(?[489][0-9]{2}\)?[\s\-]?[0-9]{3}[\s\-]?[0-9]{2}[\s\-]?[0-9]{2}$")
                .WithMessage("Телефон должен быть в формате +7 (XXX) XXX-XX-XX или 8 (XXX) XXX-XX-XX");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email обязателен для заполнения")
                .EmailAddress().WithMessage("Неверный формат email");

            RuleFor(x => x.OrderedItems)
                .NotEmpty().WithMessage("Список товаров обязателен")
                .Must(items => items != null && items.Count > 0).WithMessage("Заказ должен содержать хотя бы один товар");

            RuleForEach(x => x.OrderedItems)
                .SetValidator(new OrderedItemValidator());
        }


    }

    /// <summary>
    /// Валидатор для товара в заказе
    /// </summary>
    public class OrderedItemValidator : AbstractValidator<OrderedItem>
    {
        public OrderedItemValidator()
        {
            RuleFor(x => x.ID)
                .NotEmpty().WithMessage("ID товара обязателен");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Количество должно быть больше 0")
                .LessThanOrEqualTo(10000).WithMessage("Количество не может превышать 10000");

            RuleFor(x => x.Unit)
                .NotEmpty().WithMessage("Единица измерения обязательна")
                .Must(unit => unit == "т" || unit == "м" || unit == "t" || unit == "m").WithMessage("Единица измерения должна быть 'т', 'м', 't' или 'm'");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Цена за единицу должна быть больше 0")
                .LessThan(1000000).WithMessage("Цена за единицу не может превышать 1000000");
        }
    }
}
