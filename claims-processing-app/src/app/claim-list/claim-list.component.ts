import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { RouterModule } from '@angular/router';
import { ClaimService } from '../services/claim.service';
import { AuthService } from '../services/auth.service';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { HttpClient } from '@angular/common/http';
import { MatDialog, MatDialogContent, MatDialogModule } from '@angular/material/dialog';
import { AssessmentAdjustmentDialogComponent } from './assessment-adjustment-dialog.component';
import { ReactiveFormsModule } from '@angular/forms';
import { ClaimDetailsDialogComponent } from './claim-details-dialog.component';
import { MatSelectModule } from '@angular/material/select';
import { InvoiceUploadDialogComponent } from './invoice-upload-dialog.component';
interface Claim {
  claimId: number;
  policyNumber: string;
  incidentDate: string;
  claimAmount: number;
  incidentLocation: string;
  status: string;
  description: string;
  documentId: string | null;
}

@Component({
  selector: 'app-claim-list',
  templateUrl: './claim-list.component.html',
  styleUrls: ['./claim-list.component.scss'],
  standalone: true,
  imports: [
    CommonModule,
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
    ReactiveFormsModule,
    MatSelectModule
  ]
})
export class ClaimListComponent implements OnInit {
  claims: Claim[] = [];
  displayedColumns: string[] = ['claimId', 'policyNumber', 'description', 'incidentDate', 'claimAmount', 'incidentLocation', 'status', 'document', 'actions'];
  isCustomer: boolean = false;
  isSurveyor: boolean = false;
  isAdjuster: boolean = false;
  isWorkshop: boolean = false;

  constructor(
    private claimService: ClaimService,
    public authService: AuthService,
    private snackBar: MatSnackBar,
    private http: HttpClient,
    private dialog: MatDialog
  ) {}

  ngOnInit() {
    this.loadClaims();
    this.isCustomer = this.authService.hasRole('Customer');
    this.isSurveyor = this.authService.isSurveyor();
    this.isAdjuster = this.authService.isAdjuster();
    this.isWorkshop = this.authService.isWorkshop();
    // if (this.isCustomer) {
    //   this.displayedColumns = this.displayedColumns.filter(col => col !== 'actions');
    // }
  }

  openClaimDetailsDialog(claim: any) {
    const dialogRef = this.dialog.open(ClaimDetailsDialogComponent, {
      width: '800px',
      data: claim
    });
    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        // Refresh the claims list if changes were made
        this.loadClaims();
      }
    });
  }

  loadClaims() {
    const userId = this.authService.currentUserId;
    if (userId) {
      this.claimService.getClaims().subscribe(
        (data: Claim[]) => {
          this.claims = data;
          console.log('Claims loaded:', this.claims);
          if (this.claims.length === 0) {
            this.snackBar.open('No claims found for this user.', 'Close', { duration: 3000 });
          }
        },
        (error) => {
          console.error('Error fetching claims:', error);
          this.snackBar.open('Error loading claims. Please try again.', 'Close', { duration: 3000 });
        }
      );
    } else {
      console.error('No user ID available');
      this.snackBar.open('Please log in to view your claims.', 'Close', { duration: 3000 });
    }
  }

  getStatusColor(status: string): string {
    switch (status.toLowerCase()) {
      case 'initiated':
      case 'surveyor assigned':
      case 'adjustor assigned':
        return 'blue';
      case 'surveyor approved':
      case 'adjustor approved':
      case 'work started':
      case 'work completed - payment pending':
      case 'payment completed':
      case 'completed & delivered':
        return 'green';
      case 'surveyor rejected':
      case 'adjustor rejected':
        return 'red';
      default:
        return 'grey';
    }
  }

  openInvoiceUploadDialog(claim: any): void {
    const dialogRef = this.dialog.open(InvoiceUploadDialogComponent, {
      width: '400px',
      data: { claimId: claim.ClaimId }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadClaims();
      }
    });
  }

  viewDocument(documentId: string | null) {
    if (!documentId) {
      this.snackBar.open('No document available for this claim.', 'Close', { duration: 3000 });
      return;
    }

    this.http.get(`http://api.ycompany.local/documents/api/Documents/${documentId}`, { responseType: 'blob', observe: 'response' })
      .subscribe(
        (response) => {
          const contentDisposition = response.headers.get('content-disposition');
          let filename = 'document';
          if (contentDisposition) {
            const filenameRegex = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/;
            const matches = filenameRegex.exec(contentDisposition);
            if (matches != null && matches[1]) {
              filename = matches[1].replace(/['"]/g, '');
            }
          }
          const blob = new Blob([response.body as BlobPart], { type: response.headers.get('content-type') || 'application/octet-stream' });
          const url = window.URL.createObjectURL(blob);
          const link = document.createElement('a');
          link.href = url;
          link.download = filename;
          link.click();
          window.URL.revokeObjectURL(url);
        },
        (error) => {
          console.error('Error downloading document:', error);
          this.snackBar.open('Error downloading document. Please try again.', 'Close', { duration: 3000 });
        }
      );
  }

  openAssessmentAdjustmentDialog(claim: any) {
    const dialogRef = this.dialog.open(AssessmentAdjustmentDialogComponent, {
      width: '500px',
      data: { claim, isSurveyor: this.isSurveyor, isAdjuster: this.isAdjuster }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        if (this.isSurveyor) {
          this.claimService.createAssessment(result).subscribe(
            () => {
              this.snackBar.open('Assessment created successfully', 'Close', { duration: 3000 });
              this.loadClaims();
            },
            error => console.error('Error creating assessment:', error)
          );
        } else if (this.isAdjuster) {
          this.claimService.adjustClaim(claim.ClaimId, result).subscribe(
            () => {
              this.snackBar.open('Claim adjusted successfully', 'Close', { duration: 3000 });
              this.loadClaims();
            },
            error => console.error('Error adjusting claim:', error)
          );
        }
      }
    });
  }

  canUploadInvoice(claim: any): boolean {
    return this.isWorkshop && claim.Status === 'Work Completed - Payment Pending';
  }

  updateClaimStatus(claim:any, newStatus: string) {
    this.claimService.updateClaimStatus(claim.ClaimId, newStatus).subscribe(
      () => {
        this.snackBar.open(`Claim status updated to ${newStatus}`, 'Close', { duration: 3000 });
        this.loadClaims();
      },
      error => {
        console.error('Error updating claim status:', error);
        this.snackBar.open('Error updating claim status. Please try again.', 'Close', { duration: 3000 });
      }
    );
  }

  canAssessOrAdjust(claim: any): boolean {
    if (this.isSurveyor) {
      return claim.Status === 'Surveyor Assigned';
    } else if (this.isAdjuster) {
      return claim.Status === 'Adjustor Assigned' || claim.Status === 'Surveyor Approved';
    }
    return false;
  }

  canUpdateStatus(claim: any): boolean {
    if (this.isWorkshop) {
      return ['Adjustor Approved', 'Work Started','Payment Completed'].includes(claim.Status);
    }
    return false;
  }

  getNextStatus(currentStatus: string): string {
    switch (currentStatus) {
      case 'Adjustor Approved':
        return 'Work Started';
      case 'Work Started':
        return 'Work Completed - Payment Pending';
      case 'Payment Completed':
        return 'Completed & Delivered';
      default:
        return '';
    }
  }

  canViewInvoice(claim: any): boolean {
    return this.isCustomer && claim.Status === 'Work Completed - Payment Pending';
  }

  canPayInvoice(claim: any): boolean {
    return this.isCustomer && claim.Status === 'Work Completed - Payment Pending';
  }

  viewInvoice(claim: any): void {
    this.claimService.getInvoice(claim.ClaimId).subscribe(
      (response: Blob) => {
        const url = window.URL.createObjectURL(response);
        window.open(url, '_blank');
      },
      error => {
        console.error('Error viewing invoice:', error);
        this.snackBar.open('Error viewing invoice. Please try again.', 'Close', { duration: 3000 });
      }
    );
  }

  payInvoice(claim:any): void {
    this.claimService.payInvoice(claim.ClaimId).subscribe(
      () => {
        this.snackBar.open('Invoice paid successfully', 'Close', { duration: 3000 });
        this.loadClaims();
      },
      error => {
        console.error('Error paying invoice:', error);
        this.snackBar.open('Error paying invoice. Please try again.', 'Close', { duration: 3000 });
      }
    );
  }
}
