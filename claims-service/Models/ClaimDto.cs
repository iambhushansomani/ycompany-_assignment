using System;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace YCompanyClaimsApi.Models
{
    public class ClaimDto
    {
        [Required]
        [StringLength(500, MinimumLength = 10)]
        public string Description { get; set; }

        [Required]
        public DateTime IncidentDate { get; set; }

        [Required]
        public string PolicyNumber { get; set; }

        public IFormFile Document { get; set; }

        [Range(0, double.MaxValue)]
        public decimal ClaimAmount { get; set; }

        [StringLength(200)]
        public string IncidentLocation { get; set; }
    }
}