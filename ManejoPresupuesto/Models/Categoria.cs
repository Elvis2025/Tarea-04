﻿using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuesto.Models
{
    public class Categoria
    {
        public int Id { get; set; }

        [Required(ErrorMessage ="El campo {0} es requerido")]
        [StringLength(maximumLength:50, ErrorMessage ="No puede ser mayor a {1} carácteres")]
        /*[Display (Name ="Nombre")]*/
        public string Nombre { get; set; }
        public TipoOperacion TipoOperacionId { get; set; } 
        public int UsuarioId { get; set; }
        
    }
}
