import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { CommonModule } from '@angular/common';
import { MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatSelectModule } from '@angular/material/select';
import { FormsModule } from '@angular/forms';
import { ClaimService } from '../services/claim.service';
import { AuthService } from '../services/auth.service';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-claim-details-dialog',
  template: `
    <h2 mat-dialog-title>Claim Details</h2>
    <mat-dialog-content>
      <div class="workflow-container">
        <div class="workflow-step" *ngFor="let step of workflowSteps; let i = index"
             [class.active]="i <= currentStepIndex">
          {{ step }}
        </div>
      </div>
      <ng-container *ngIf="isClaimManager && isEditing; else readOnlyView">
        <form>
          <mat-form-field>
            <mat-label>Policy Number</mat-label>
            <input matInput [(ngModel)]="editableData.PolicyNumber" name="policyNumber">
          </mat-form-field>
          <mat-form-field>
            <mat-label>Incident Date</mat-label>
            <input matInput [matDatepicker]="picker" [(ngModel)]="editableData.IncidentDate" name="incidentDate">
            <mat-datepicker-toggle matSuffix [for]="picker"></mat-datepicker-toggle>
            <mat-datepicker #picker></mat-datepicker>
          </mat-form-field>
          <mat-form-field>
            <mat-label>Claim Amount</mat-label>
            <input matInput type="number" [(ngModel)]="editableData.ClaimAmount" name="claimAmount">
          </mat-form-field>
          <mat-form-field>
            <mat-label>Incident Location</mat-label>
            <input matInput [(ngModel)]="editableData.IncidentLocation" name="incidentLocation">
          </mat-form-field>
          <mat-form-field>
            <mat-label>Status</mat-label>
            <mat-select [(ngModel)]="editableData.Status" name="status">
              <mat-option *ngFor="let status of statusOptions" [value]="status">{{status}}</mat-option>
            </mat-select>
          </mat-form-field>
          <mat-form-field>
            <mat-label>Description</mat-label>
            <textarea matInput [(ngModel)]="editableData.Description" name="description"></textarea>
          </mat-form-field>
        </form>
      </ng-container>
      <ng-template #readOnlyView>
        <div *ngFor="let field of objectKeys(data); let last = last">
          <p><strong>{{formatFieldName(field)}}:</strong> {{formatFieldValue(data[field], field)}}</p>
          <hr *ngIf="!last" class="field-separator">
        </div>
      </ng-template>
      
      <h6 *ngIf="assessmentDetails">Assessment Details</h6>
      <div *ngIf="assessmentDetails">
        <div *ngFor="let field of objectKeys(assessmentDetails); let last = last">
          <p><strong>{{formatFieldName(field)}}:</strong> {{formatFieldValue(assessmentDetails[field], field)}}</p>
          <hr *ngIf="!last" class="field-separator">
        </div>
      </div>
      <mat-spinner *ngIf="loading" diameter="30"></mat-spinner>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button (click)="dialogRef.close()">Close</button>
      <button mat-button color="primary" *ngIf="isClaimManager && !isEditing" (click)="toggleEdit()">Edit</button>
      <button mat-button color="primary" *ngIf="isClaimManager && isEditing" (click)="saveChanges()">Save Changes</button>
      <button mat-button *ngIf="isClaimManager && isEditing" (click)="cancelEdit()">Cancel</button>
    </mat-dialog-actions>
  `,
  styles: [`
    .workflow-container {
      display: flex;
      justify-content: space-between;
      margin-bottom: 20px;
      padding: 10px;
      background-color: #f0f0f0;
      border-radius: 5px;
    }
    .workflow-step {
      text-align: center;
      padding: 5px 10px;
      border-radius: 15px;
      background-color: #e0e0e0;
      font-size: 12px;
    }
    .workflow-step.active {
      background-color: #4caf50;
      color: white;
    }
    mat-form-field {
      width: 100%;
      margin-bottom: 15px;
    }
    .field-separator {
      border: 0;
      border-top: 1px solid #e0e0e0;
      margin: 10px 0;
    }
  `],
  standalone: true,
  imports: [
    CommonModule,
    MatDialogModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    MatFormFieldModule,
    MatInputModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatSelectModule,
    FormsModule
  ]
})
export class ClaimDetailsDialogComponent implements OnInit {
  objectKeys = Object.keys;
  assessmentDetails: any = null;
  loading = false;
  isClaimManager = false;
  isEditing = false;
  editableData: any;
  workflowSteps = [
    'Initiated',
    'Surveyor Assigned',
    'Surveyor Approved',
    'Adjustor Assigned',
    'Adjustor Approved',
    'Work Started',
    'Work Completed - Payment Pending',
    'Completed'
  ];
  currentStepIndex = 0;
  statusOptions = [
    'Initiated',
    'Surveyor Assigned',
    'Surveyor Approved',
    'Surveyor Rejected',
    'Adjustor Assigned',
    'Adjustor Approved',
    'Adjustor Rejected',
    'Work Started',
    'Work Completed - Payment Pending',
    'Payment Completed',
    'Completed & Delivered'
  ];

  constructor(
    public dialogRef: MatDialogRef<ClaimDetailsDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private claimService: ClaimService,
    private authService: AuthService,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit() {
    this.fetchAssessmentDetails();
    this.setCurrentStepIndex();
    this.isClaimManager = this.authService.hasRole('ClaimsManager');
    this.editableData = { ...this.data };

  }

  fetchAssessmentDetails() {
    this.loading = true;
    this.claimService.getAssessmentDetails(this.data.ClaimId).subscribe(
      (response: any) => {
        this.assessmentDetails = response;
        this.loading = false;
      },
      (error) => {
        console.error('Error fetching assessment details:', error);
        this.loading = false;
      }
    );
  }

  setCurrentStepIndex() {
    const currentStatus = this.data.Status.toLowerCase();
    this.currentStepIndex = this.workflowSteps.findIndex(step => 
      currentStatus.includes(step.toLowerCase())
    );
    if (this.currentStepIndex === -1) {
      this.currentStepIndex = 0; // Default to first step if status not found
    }
  }

  formatFieldName(field: string): string {
    return field.replace(/([A-Z])/g, ' $1').replace(/^./, str => str.toUpperCase());
  }

  formatFieldValue(value: any, field: string): string {
    if (value instanceof Date) {
      return value.toLocaleDateString();
    }
    if (typeof value === 'number' && (field.toLowerCase().includes('amount') || field.toLowerCase().includes('estimate'))) {
      return new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD' }).format(value);
    }
    if (value === null || value === undefined) {
      return 'N/A';
    }
    return value.toString();
  }

  toggleEdit() {
    this.isEditing = true;
  }

  cancelEdit() {
    this.isEditing = false;
  }

  saveChanges() {
    this.loading = true;
    this.claimService.updateClaim(this.data.ClaimId, this.editableData).subscribe(
      (updatedClaim) => {
        this.loading = false;
        this.snackBar.open('Claim updated successfully', 'Close', { duration: 3000 });
        this.dialogRef.close(updatedClaim);
      },
      (error) => {
        this.loading = false;
        console.error('Error updating claim:', error);
        this.snackBar.open('Error updating claim. Please try again.', 'Close', { duration: 3000 });
      }
    );
  }
}
