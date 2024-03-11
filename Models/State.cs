
namespace JobListing.Models
{
    public class State
    {
        public int StateId { get; set; }
        public string StateName { get; set; }

        public static implicit operator List<object>(State v)
        {
            throw new NotImplementedException();
        }
    }
}
