using JobListing.DAL;
using JobListing.Models;
using Microsoft.AspNetCore.Mvc;

namespace JobListing.ViewModel
{
    public class ApplyViewModel
    {
        public JobApplication Apply { get; set; }
        public List<State> States { get; set; }
        public List<City> Cities { get; set; }


    }

}
