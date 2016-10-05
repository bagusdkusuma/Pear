namespace DSLNG.PEAR.Services.Requests.CustomFormula
{
    public class GetFeedGasGSARequest
    {
        public double JccPrice { get; set; }
    }

    public class GetLNGPriceSpaRequest
    {
        public double JccPrice { get; set; }
        public double BunkerPrice { get; set; }
    }
}
