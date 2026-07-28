using FluentAssertions;
using Yarqua.Application.Features.Auth.Commands.Register;

namespace Yarqua.Application.Tests;

/// <summary>
/// Pruebas del validador de registro.
/// </summary>
public class RegisterCommandValidatorTests
{
    private readonly RegisterCommandValidator _validator = new();

    [Fact]
    public void Rejects_ShortName()
    {
        var result = _validator.Validate(new RegisterCommand
        {
            Name = "A",
            Country = "Colombia",
            Department = "Valle",
            City = "Roldanillo",
        });

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(RegisterCommand.Name));
    }

    [Fact]
    public void Accepts_ValidPayload()
    {
        var result = _validator.Validate(new RegisterCommand
        {
            Name = "Ana",
            Country = "Colombia",
            Department = "Valle del Cauca",
            City = "Roldanillo",
        });

        result.IsValid.Should().BeTrue();
    }
}
