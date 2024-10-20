import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { WorkshopService } from '../services/workshop.service';
import { ClaimService } from '../services/claim.service';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatListModule } from '@angular/material/list';
import { MatFormFieldModule } from '@angular/material/form-field';

@Component({
  selector: 'app-workshop-selection',
  templateUrl: './workshop-selection.component.html',
  styleUrls: ['./workshop-selection.component.css'],
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatInputModule,
    MatButtonModule,
    MatListModule,
    MatFormFieldModule
  ]
})
export class WorkshopSelectionComponent implements OnInit {
  claimId: string = '';
  workshops: any[] = [];
  selectedWorkshop: any;
  pincode: string = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private workshopService: WorkshopService,
    private claimService: ClaimService
  ) { }

  ngOnInit() {
    this.claimId = this.route.snapshot.paramMap.get('claimId') || '';
  }

  searchWorkshops() {
    if (this.pincode) {
      this.workshopService.getWorkshopsByPincode(this.pincode).subscribe(
        (workshops) => {
          this.workshops = workshops;
        },
        (error) => {
          console.error('Error fetching workshops:', error);
        }
      );
    }
  }

  selectWorkshop(workshop: any) {
    this.selectedWorkshop = workshop;
  }

  submitWorkshop() {
    if (this.selectedWorkshop && this.claimId) {
      this.claimService.selectWorkshop(this.claimId, this.selectedWorkshop.id).subscribe(
        (response) => {
          console.log('Workshop selected:', response);
          this.router.navigate(['/document-upload', this.claimId]);
        },
        (error) => {
          console.error('Error selecting workshop:', error);
        }
      );
    }
  }
}