import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ClaimService } from '../services/claim.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';

@Component({
  selector: 'app-invoice-upload-dialog',
  template: `
    <h2 mat-dialog-title>Upload Invoice for Claim {{data.claimId}}</h2>
    <mat-dialog-content>
      <form [formGroup]="form">
        <mat-form-field appearance="fill">
          <mat-label>Invoice Amount</mat-label>
          <input matInput type="number" formControlName="amount" required>
        </mat-form-field>
        <input type="file" (change)="onFileSelected($event)" accept=".pdf,.jpg,.jpeg,.png" required>
      </form>
    </mat-dialog-content>
    <mat-dialog-actions>
      <button mat-button (click)="onNoClick()">Cancel</button>
      <button mat-button [disabled]="!form.valid || !selectedFile" (click)="onSubmit()">Upload</button>
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
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule
  ]
})
export class InvoiceUploadDialogComponent {
  form: FormGroup;
  selectedFile: File | null = null;

  constructor(
    public dialogRef: MatDialogRef<InvoiceUploadDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { claimId: number },
    private fb: FormBuilder,
    private claimService: ClaimService,
    private snackBar: MatSnackBar
  ) {
    this.form = this.fb.group({
      amount: ['', [Validators.required, Validators.min(0)]]
    });
  }

  onNoClick(): void {
    this.dialogRef.close();
  }

  onFileSelected(event: any): void {
    this.selectedFile = event.target.files[0] as File;
  }

  onSubmit(): void {
    if (this.form.valid && this.selectedFile) {
      const amount = this.form.get('amount')?.value;
      this.claimService.uploadInvoice(this.data.claimId, amount, this.selectedFile).subscribe(
        () => {
          this.snackBar.open('Invoice uploaded successfully', 'Close', { duration: 3000 });
          this.dialogRef.close(true);
        },
        error => {
          console.error('Error uploading invoice:', error);
          this.snackBar.open('Error uploading invoice. Please try again.', 'Close', { duration: 3000 });
        }
      );
    }
  }
}