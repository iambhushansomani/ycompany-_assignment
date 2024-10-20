import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatSelectModule } from '@angular/material/select';
import { MatStepperModule } from '@angular/material/stepper';
import { ClaimService } from '../services/claim.service';
import { AuthService } from '../services/auth.service';
import { Router } from '@angular/router';
import { HttpHeaders } from '@angular/common/http';

@Component({
  selector: 'app-claim-initiation',
  templateUrl: './claim-initiation.component.html',
  styleUrls: ['./claim-initiation.component.scss'],
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatButtonModule,
    MatSnackBarModule,
    MatSelectModule,
    MatStepperModule
  ]
})
export class ClaimInitiationComponent implements OnInit {
  claimForm: FormGroup;
  workshopForm: FormGroup;
  selectedFile: File | null = null;
  workshops: any[] = [];
  surveyors: any[] = [];

  constructor(
    private fb: FormBuilder,
    private claimService: ClaimService,
    private authService: AuthService,
    private router: Router,
    private snackBar: MatSnackBar
  ) {
    this.claimForm = this.fb.group({
      description: ['', [Validators.required, Validators.minLength(10), Validators.maxLength(500)]],
      incidentDate: ['', Validators.required],
      policyNumber: ['', [Validators.required, Validators.pattern('^[0-9]{10}$')]],
      claimAmount: ['', [Validators.required, Validators.min(0)]],
      incidentLocation: ['', [Validators.required, Validators.maxLength(200)]],
    });

    this.workshopForm = this.fb.group({
      workshopId: ['', Validators.required]
    });
  }

  ngOnInit() {
    console.log('Component initialized');
    this.loadWorkshops();
    this.loadSurveyors();
  }

  loadWorkshops() {
    this.claimService.getWorkshops().subscribe(
      (response: any) => {
        if (Array.isArray(response)) {
          this.workshops = response;
        } else if (response && typeof response === 'object' && Array.isArray(response.$values)) {
          this.workshops = response.$values;
        } else {
          console.error('Unexpected response format from getWorkshops:', response);
          this.workshops = [];
        }
        console.log('Loaded workshops:', this.workshops); // Add this line for debugging
      },
      (error) => {
        console.error('Error loading workshops:', error);
        this.snackBar.open('Error loading workshops. Please try again.', 'Close', { duration: 3000 });
        this.workshops = [];
      }
    );
  }

  loadSurveyors() {
    this.authService.getSurveyors().subscribe(
      (surveyors) => {
        this.surveyors = Array.isArray(surveyors) ? surveyors : [];
      },
      (error) => {
        console.error('Error loading surveyors:', error);
        this.snackBar.open('Error loading surveyors. Please try again.', 'Close', { duration: 3000 });
      }
    );
  }

  onFileSelected(event: any): void {
    this.selectedFile = event.target.files[0];
  }

  onSubmit() {
    if (this.claimForm.valid && this.workshopForm.valid && this.selectedFile) {
      const formData = new FormData();
      Object.keys(this.claimForm.value).forEach(key => {
        if (key === 'incidentDate') {
          const date = new Date(this.claimForm.get(key)?.value);
          const formattedDate = date.toISOString().split('T')[0];
          formData.append(key, formattedDate);
        } else {
          formData.append(key, this.claimForm.get(key)?.value);
        }
      });
      formData.append('Document', this.selectedFile, this.selectedFile.name);
      formData.append('AdjustorNotes', '');
      formData.append('InvoiceDocumentUuid', '00000000-0000-0000-0000-000000000000');
  
      this.claimService.initiateClaim(formData).subscribe(
        (response) => {
          console.log('Claim initiated:', response);
          this.assignWorkshop(response.ClaimId);
          this.assignRandomSurveyor(response.ClaimId);
        },
        (error) => {
          console.error('Error initiating claim:', error);
          this.snackBar.open('Error initiating claim. Please try again.', 'Close', { duration: 3000 });
        }
      );
    } else {
      this.snackBar.open('Please fill all required fields correctly and upload a document.', 'Close', { duration: 3000 });
    }
  }

  assignWorkshop(claimId: number) {
    const workshopId = this.workshopForm.get('workshopId')?.value;
    const headers = new HttpHeaders({
      'Content-Type': 'application/json'
    });

    this.claimService.assignWorkshopToClaim(claimId, JSON.stringify(workshopId), { headers }).subscribe(
      (response) => {
        console.log('Workshop assigned:', response);
        this.snackBar.open('Claim initiated and workshop assigned successfully', 'Close', { duration: 3000 });
      },
      (error) => {
        console.error('Error assigning workshop:', error);
        this.snackBar.open('Error assigning workshop. Please try again.', 'Close', { duration: 3000 });
      }
    );
  }

  assignRandomSurveyor(claimId: number) {
    if (this.surveyors.length > 0) {
      const randomSurveyor = this.surveyors[Math.floor(Math.random() * this.surveyors.length)];
      this.claimService.assignSurveyorToClaim(claimId, randomSurveyor.Id).subscribe(
        (response) => {
          console.log('Surveyor assigned:', response);
          this.snackBar.open('Surveyor assigned successfully', 'Close', { duration: 3000 });
          // Redirect to the claim tracking page
          this.router.navigate(['/claim-list']);
        },
        (error) => {
          console.error('Error assigning surveyor:', error);
          this.snackBar.open('Error assigning surveyor. Please try again.', 'Close', { duration: 3000 });
        }
      );
    } else {
      console.error('No surveyors available');
      this.snackBar.open('No surveyors available. Please try again later.', 'Close', { duration: 3000 });
    }
  }
}