namespace Agencies.Shared
{
    public class IdentificationResult
    {
        public Face Face { get; set; }

        public Person Person { get; set; }

        public float Confidence { get; set; }
    }
}