using ManejoPresupuesto.Models.Validaciones;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuesto.Models
{
    public class TipoCuenta
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "El campo {0} es requerido") ]
        [PrimeraLetraMayuscula]
        [Remote(action: "VerificarExisteTipoCuenta", controller: "TiposCuentas")]
        public string Nombre { get; set; }

        public int UsuarioId { get; set; }
        public int OrdenId { get; set; }

       /* public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if(Nombre != null && Nombre.Length > 0)
            {
                var primeraLetra = Nombre[0].ToString();
                if (primeraLetra != primeraLetra.ToUpper()) 
                {
                    yield return new ValidationResult("La primera letra debe de ser mayúscula");
                }
        }*/

        /*Prueba de otras validaciones *//*
        [Required(ErrorMessage = "El Campo {0} es requerido")]
        [EmailAddress(ErrorMessage = "El Campo debe ser un correo electrónico válido")]
        public string? Email { get; set; }

        [Range(minimum:18, maximum:130, ErrorMessage = "El valor debe estar entre {1} y {2}")]
        [Required(ErrorMessage = "El Campo {0} es requerido.")]
        public int Edad { get; set; }

        [Url(ErrorMessage = "El campo debe de ser una URL válida")]
        [Required(ErrorMessage = "El Campo {0} es requerido.")]
        public string? URL { get; set; }

        [CreditCard(ErrorMessage = "La tarjeta de credito no es válida")]
        [Display(Name = "Tarjeta de Crédito")]
        [Required(ErrorMessage = "El Campo {0} es requerido.")]
        public string? TarjetaDeCredito { get; set; }*/

    }
}
