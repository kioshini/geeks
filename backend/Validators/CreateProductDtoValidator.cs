using FluentValidation;
using TMKMiniApp.Models.DTOs;

namespace TMKMiniApp.Validators
{
    /// <summary>
    /// Validator for CreateProductDto
    /// </summary>
    public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
    {
        public CreateProductDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Название продукта обязательно")
                .MaximumLength(200).WithMessage("Название не должно превышать 200 символов");

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Код продукта обязателен")
                .MaximumLength(50).WithMessage("Код не должен превышать 50 символов")
                .Matches("^[A-Za-z0-9_-]+$").WithMessage("Код может содержать только буквы, цифры, дефисы и подчеркивания");

            RuleFor(x => x.Type)
                .NotEmpty().WithMessage("Тип продукта обязателен")
                .MaximumLength(100).WithMessage("Тип не должен превышать 100 символов");

            RuleFor(x => x.Material)
                .NotEmpty().WithMessage("Материал обязателен")
                .MaximumLength(100).WithMessage("Материал не должен превышать 100 символов");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Цена должна быть больше 0")
                .LessThan(1000000).WithMessage("Цена не должна превышать 1,000,000");

            RuleFor(x => x.StockQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("Количество на складе не может быть отрицательным")
                .LessThan(1000000).WithMessage("Количество на складе не должно превышать 1,000,000");

            RuleFor(x => x.Diameter)
                .GreaterThan(0).When(x => x.Diameter.HasValue)
                .WithMessage("Диаметр должен быть больше 0")
                .LessThan(10000).When(x => x.Diameter.HasValue)
                .WithMessage("Диаметр не должен превышать 10,000");

            RuleFor(x => x.Thickness)
                .GreaterThan(0).When(x => x.Thickness.HasValue)
                .WithMessage("Толщина должна быть больше 0")
                .LessThan(1000).When(x => x.Thickness.HasValue)
                .WithMessage("Толщина не должна превышать 1,000");

            RuleFor(x => x.Length)
                .GreaterThan(0).When(x => x.Length.HasValue)
                .WithMessage("Длина должна быть больше 0")
                .LessThan(100000).When(x => x.Length.HasValue)
                .WithMessage("Длина не должна превышать 100,000");

            RuleFor(x => x.Unit)
                .MaximumLength(20).WithMessage("Единица измерения не должна превышать 20 символов");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Описание не должно превышать 1000 символов");
        }
    }
}
