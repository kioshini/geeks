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
                .Matches(@"^\d{10}$|^\d{12}$").WithMessage("ИНН должен содержать 10 или 12 цифр")
                .Must(BeValidINN).WithMessage("Неверный формат ИНН");

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

        private bool BeValidINN(string inn)
        {
            if (string.IsNullOrEmpty(inn) || (inn.Length != 10 && inn.Length != 12))
                return false;

            // Простая проверка на цифры
            if (!inn.All(char.IsDigit))
                return false;

            // Дополнительная проверка контрольной суммы для ИНН (упрощенная)
            if (inn.Length == 10)
            {
                return ValidateINN10(inn);
            }
            else if (inn.Length == 12)
            {
                return ValidateINN12(inn);
            }

            return false;
        }

        private bool ValidateINN10(string inn)
        {
            if (inn.Length != 10) return false;

            int[] coefficients = { 2, 4, 10, 3, 5, 9, 4, 6, 8 };
            int sum = 0;

            for (int i = 0; i < 9; i++)
            {
                sum += int.Parse(inn[i].ToString()) * coefficients[i];
            }

            int remainder = sum % 11;
            int checkDigit = remainder < 2 ? remainder : remainder % 11;

            return checkDigit == int.Parse(inn[9].ToString());
        }

        private bool ValidateINN12(string inn)
        {
            if (inn.Length != 12) return false;

            int[] coefficients1 = { 7, 2, 4, 10, 3, 5, 9, 4, 6, 8 };
            int[] coefficients2 = { 3, 7, 2, 4, 10, 3, 5, 9, 4, 6, 8 };

            int sum1 = 0;
            for (int i = 0; i < 10; i++)
            {
                sum1 += int.Parse(inn[i].ToString()) * coefficients1[i];
            }

            int sum2 = 0;
            for (int i = 0; i < 11; i++)
            {
                sum2 += int.Parse(inn[i].ToString()) * coefficients2[i];
            }

            int remainder1 = sum1 % 11;
            int remainder2 = sum2 % 11;

            int checkDigit1 = remainder1 < 2 ? remainder1 : remainder1 % 11;
            int checkDigit2 = remainder2 < 2 ? remainder2 : remainder2 % 11;

            return checkDigit1 == int.Parse(inn[10].ToString()) && 
                   checkDigit2 == int.Parse(inn[11].ToString());
        }
    }

    /// <summary>
    /// Валидатор для товара в заказе
    /// </summary>
    public class OrderedItemValidator : AbstractValidator<OrderedItem>
    {
        public OrderedItemValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("ID товара обязателен");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Количество должно быть больше 0")
                .LessThanOrEqualTo(10000).WithMessage("Количество не может превышать 10000");

            RuleFor(x => x.Unit)
                .NotEmpty().WithMessage("Единица измерения обязательна")
                .Must(unit => unit == "т" || unit == "м").WithMessage("Единица измерения должна быть 'т' или 'м'");

            RuleFor(x => x.UnitPrice)
                .GreaterThan(0).WithMessage("Цена за единицу должна быть больше 0")
                .LessThan(1000000).WithMessage("Цена за единицу не может превышать 1000000");
        }
    }
}
