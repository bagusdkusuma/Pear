
namespace DSLNG.PEAR.Services.Requests.CalculatorConstant
{
    public class SaveCalculatorConstantRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public double Value { get; set; }
        public string Measurement { get; set; }
        public bool IsAjaxRequest { get; set; }
    }
}
