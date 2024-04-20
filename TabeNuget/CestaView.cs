namespace TabeNuget
{
    public class CestaView
    {
        public List<ProductoCestaView> Cesta { get; set; }
        public decimal Total { get; set; }
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
    }
}
