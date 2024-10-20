using YCompanyClaimsApi.Models;
using System;
using System.Linq;
using System.IO;

namespace YCompanyClaimsApi.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            // Look for any existing claims
            if (context.Documents.Any())
            {
                return;   // DB has been seeded
            }



                        // Add claims with various statuses
            var claims = new Claim[]
            {
                new Claim
                {
                    UserId = 1,
                    PolicyNumber = "POL0000001",
                    Description = "Car accident on Main Street",
                    IncidentDate = DateTime.Now.AddDays(-10),
                    ClaimAmount = 5000.00m,
                    Status = "Initiated",
                    IncidentLocation = "Main Street, Cityville",
                    DocumentUuid = "doc-uuid-1",
                    SelectedWorkshopId = 0,
                    AdjustorNotes = "",
                    InvoiceDocumentUuid = "",
                    IsPaymentComplete = false,
                    PaymentDate = null
                },
                new Claim
                {
                    UserId = 2,
                    PolicyNumber = "POL0000002",
                    Description = "Theft of personal items from vehicle",
                    IncidentDate = DateTime.Now.AddDays(-5),
                    ClaimAmount = 1500.00m,
                    Status = "Surveyor Assigned",
                    IncidentLocation = "Parking Lot, Shopping Mall",
                    DocumentUuid = "doc-uuid-2",
                    SelectedWorkshopId = 1,
                    SurveyorId = 4,
                    AdjustorNotes = "",
                    InvoiceDocumentUuid = "",
                    IsPaymentComplete = false,
                    PaymentDate = null
                },
                new Claim
                {
                    UserId = 1,
                    PolicyNumber = "POL0000003",
                    Description = "Water damage due to pipe burst",
                    IncidentDate = DateTime.Now.AddDays(-15),
                    ClaimAmount = 8000.00m,
                    Status = "Surveyor Approved",
                    IncidentLocation = "123 Home Street, Townsville",
                    DocumentUuid = "doc-uuid-3",
                    SelectedWorkshopId = 2,
                    SurveyorId = 4,
                    AdjustorId = 5,
                    AdjustorNotes = "Awaiting final assessment",
                    InvoiceDocumentUuid = "",
                    IsPaymentComplete = false,
                    PaymentDate = null
                },
                new Claim
                {
                    UserId = 2,
                    PolicyNumber = "POL0000004",
                    Description = "Hail damage to car roof",
                    IncidentDate = DateTime.Now.AddDays(-20),
                    ClaimAmount = 3000.00m,
                    Status = "Surveyor Rejected",
                    IncidentLocation = "456 Park Avenue, Metropolis",
                    DocumentUuid = "doc-uuid-4",
                    SelectedWorkshopId = 0,
                    SurveyorId = 4,
                    AdjustorNotes = "",
                    InvoiceDocumentUuid = "",
                    IsPaymentComplete = false,
                    PaymentDate = null
                },
                new Claim
                {
                    UserId = 1,
                    PolicyNumber = "POL0000005",
                    Description = "Fender bender in parking lot",
                    IncidentDate = DateTime.Now.AddDays(-25),
                    ClaimAmount = 2000.00m,
                    Status = "Adjustor Assigned",
                    IncidentLocation = "Grocery Store Parking Lot, Suburbia",
                    DocumentUuid = "doc-uuid-5",
                    SelectedWorkshopId = 1,
                    SurveyorId = 4,
                    AdjustorId = 5,
                    AdjustorNotes = "",
                    InvoiceDocumentUuid = "",
                    IsPaymentComplete = false,
                    PaymentDate = null
                },
                new Claim
                {
                    UserId = 2,
                    PolicyNumber = "POL0000006",
                    Description = "Windshield crack from road debris",
                    IncidentDate = DateTime.Now.AddDays(-30),
                    ClaimAmount = 500.00m,
                    Status = "Adjustor Approved",
                    IncidentLocation = "Highway 101, Mile Marker 50",
                    DocumentUuid = "doc-uuid-6",
                    SelectedWorkshopId = 2,
                    SurveyorId = 4,
                    AdjustorId = 5,
                    AdjustorNotes = "Approved for windshield replacement",
                    InvoiceDocumentUuid = "",
                    IsPaymentComplete = false,
                    PaymentDate = null
                },
                new Claim
                {
                    UserId = 1,
                    PolicyNumber = "POL0000007",
                    Description = "Vehicle stolen from driveway",
                    IncidentDate = DateTime.Now.AddDays(-35),
                    ClaimAmount = 15000.00m,
                    Status = "Adjustor Rejected",
                    IncidentLocation = "789 Residential Lane, Hometown",
                    DocumentUuid = "doc-uuid-7",
                    SelectedWorkshopId = 0,
                    SurveyorId = 4,
                    AdjustorId = 5,
                    AdjustorNotes = "Insufficient evidence provided",
                    InvoiceDocumentUuid = "",
                    IsPaymentComplete = false,
                    PaymentDate = null
                },
                new Claim
                {
                    UserId = 2,
                    PolicyNumber = "POL0000008",
                    Description = "Rear-end collision at traffic light",
                    IncidentDate = DateTime.Now.AddDays(-40),
                    ClaimAmount = 4000.00m,
                    Status = "Work Started",
                    IncidentLocation = "Intersection of Main and Oak, Downtown",
                    DocumentUuid = "doc-uuid-8",
                    SelectedWorkshopId = 1,
                    SurveyorId = 4,
                    AdjustorId = 5,
                    AdjustorNotes = "Approved for repair",
                    InvoiceDocumentUuid = "",
                    IsPaymentComplete = false,
                    PaymentDate = null
                },
                new Claim
                {
                    UserId = 1,
                    PolicyNumber = "POL0000009",
                    Description = "Side mirror damaged by passing truck",
                    IncidentDate = DateTime.Now.AddDays(-45),
                    ClaimAmount = 300.00m,
                    Status = "Work Completed - Payment Pending",
                    IncidentLocation = "Narrow Street, Old Town",
                    DocumentUuid = "doc-uuid-9",
                    SelectedWorkshopId = 2,
                    SurveyorId = 4,
                    AdjustorId = 5,
                    AdjustorNotes = "Repair completed, awaiting payment",
                    InvoiceDocumentUuid = "invoice-uuid-9",
                    IsPaymentComplete = false,
                    PaymentDate = null
                },
                new Claim
                {
                    UserId = 2,
                    PolicyNumber = "POL0000010",
                    Description = "Engine failure due to oil leak",
                    IncidentDate = DateTime.Now.AddDays(-50),
                    ClaimAmount = 7000.00m,
                    Status = "Payment Completed",
                    IncidentLocation = "Highway 202, Mile Marker 75",
                    DocumentUuid = "doc-uuid-10",
                    SelectedWorkshopId = 1,
                    SurveyorId = 4,
                    AdjustorId = 5,
                    AdjustorNotes = "Engine replacement approved and completed",
                    InvoiceDocumentUuid = "invoice-uuid-10",
                    IsPaymentComplete = true,
                    PaymentDate = DateTime.Now.AddDays(-1)
                },
                new Claim
                {
                    UserId = 1,
                    PolicyNumber = "POL0000011",
                    Description = "Bumper damage from parking mishap",
                    IncidentDate = DateTime.Now.AddDays(-55),
                    ClaimAmount = 1200.00m,
                    Status = "Completed & Delivered",
                    IncidentLocation = "Multi-story Parking Garage, City Center",
                    DocumentUuid = "doc-uuid-11",
                    SelectedWorkshopId = 2,
                    SurveyorId = 4,
                    AdjustorId = 5,
                    AdjustorNotes = "Repair completed and vehicle returned to owner",
                    InvoiceDocumentUuid = "invoice-uuid-11",
                    IsPaymentComplete = true,
                    PaymentDate = DateTime.Now.AddDays(-2)
                }
            };
            // context.Claims.AddRange(claims);
            // context.SaveChanges();

            // Add assessments
            var assessments = new Assessment[]
            {
                new Assessment
                {
                    ClaimId = claims[1].ClaimId,
                    SurveyorId = 4,
                    Report = "Detailed assessment of theft claim. Recommend approval.",
                    SubmissionDate = DateTime.Now.AddDays(-2)
                },
                new Assessment
                {
                    ClaimId = claims[2].ClaimId,
                    SurveyorId = 4,
                    Report = "Water damage assessment complete. Repairs needed.",
                    SubmissionDate = DateTime.Now.AddDays(-1)
                }
            };
            // context.Assessments.AddRange(assessments);
            // context.SaveChanges();

            // Update claims with assessment IDs
            claims[1].AssessmentId = assessments[0].Id;
            claims[2].AssessmentId = assessments[1].Id;
            // context.SaveChanges();
 

  
        }
    }
}