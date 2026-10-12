using System.ComponentModel.DataAnnotations;

namespace blazorvacio.Data;

public sealed class Product
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(120, ErrorMessage = "El nombre no puede superar los 120 caracteres.")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "La descripción no puede superar los 500 caracteres.")]
    public string? Description { get; set; }

    [Range(typeof(decimal), "0", "999999999", ErrorMessage = "El precio debe ser igual o mayor que cero.")]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "El stock debe ser igual o mayor que cero.")]
    public int Stock { get; set; }
}