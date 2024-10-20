import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogContent, MatDialogModule } from '@angular/material/dialog';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatSortModule } from '@angular/material/sort';
import { MatTableModule } from '@angular/material/table';
import { RouterModule } from '@angular/router';
import { MatFormFieldModule, MatLabel } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';

@Component({
  selector: 'app-assessment-adjustment-dialog',
  template: `
    <h2 mat-dialog-title>{{ data.isSurveyor ? 'Create Assessment' : 'Adjust Claim' }} for Claim {{data.claim.ClaimId}}</h2>
    <mat-dialog-content>
      <form [formGroup]="form">
        <mat-form-field appearance="fill">
          <mat-label>Status</mat-label>
          <mat-select formControlName="status">
            <mat-option value="Approved">Approve</mat-option>
            <mat-option value="Rejected">Reject</mat-option>
          </mat-select>
        </mat-form-field>
        <mat-form-field appearance="fill">
          <mat-label>{{ data.isSurveyor ? 'Report' : 'Adjuster Notes' }}</mat-label>
          <textarea matInput formControlName="comments" rows="4"></textarea>
        </mat-form-field>
      </form>
    </mat-dialog-content>
    <mat-dialog-actions>
      <button mat-button (click)="onNoClick()">Cancel</button>
      <button mat-button [mat-dialog-close]="prepareData()" [disabled]="!form.valid">Submit</button>
    </mat-dialog-actions>
  `,
  styles: [`
    mat-form-field {
      width: 100%;
      margin-bottom: 15px;
    }
  `],
  standalone: true,
  imports: [
    CommonModule,
    MatInputModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    RouterModule,
    MatSnackBarModule,
    MatDialogModule,
    MatDialogContent,
    MatFormFieldModule,
    MatSelectModule,
    MatLabel,
    ReactiveFormsModule
  ]
})
export class AssessmentAdjustmentDialogComponent {
  form: FormGroup;

  constructor(
    public dialogRef: MatDialogRef<AssessmentAdjustmentDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { claim: any, isSurveyor: boolean, isAdjuster: boolean },
    private fb: FormBuilder
  ) {
    this.form = this.fb.group({
      status: ['', Validators.required],
      comments: ['', Validators.required]
    });
  }

  onNoClick(): void {
    this.dialogRef.close();
  }

  prepareData(): any {
    const formValue = this.form.value;
    if (this.data.isSurveyor) {
      return this.prepareSurveyorData(formValue);
    } else if (this.data.isAdjuster) {
      return this.prepareAdjusterData(formValue);
    }
    return null;
  }

  private prepareSurveyorData(formValue: any): any {
    return {
      claimId: this.data.claim.ClaimId,
      surveyorId: this.data.claim.SurveyorId,
      status: this.getNewStatus(formValue.status),
      report: formValue.comments,
    };
  }

  private prepareAdjusterData(formValue: any): any {
    return {
      AdjustorNotes: formValue.comments,
      NewStatus: this.getNewStatus(formValue.status)
    };
  }

  private getNewStatus(assessmentStatus: string): string {
    if (this.data.isSurveyor) {
      return assessmentStatus === 'Approved' ? 'Surveyor Approved' : 'Surveyor Rejected';
    } else if (this.data.isAdjuster) {
      return assessmentStatus === 'Approved' ? 'Adjustor Approved' : 'Adjustor Rejected';
    }
    return 'Initiated'; // Default status if role is unknown
  }
}
