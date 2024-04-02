namespace ManejoPresupuesto.Models
{
    public class TransaccionActualizacionViewModel : TransaccionesCreacionViewModel
    {
        public int CuentaAnteriorId { get; set; }
        public decimal MontoAnteriorId { get; set; }
        public string urlReturn { get; set; }
    }
}
