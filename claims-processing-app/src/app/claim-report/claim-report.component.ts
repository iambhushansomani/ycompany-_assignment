import { ChangeDetectorRef, Component, OnInit, ViewChild } from '@angular/core';
import { ClaimService } from '../services/claim.service';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { BaseChartDirective, NgChartsModule } from 'ng2-charts';
import { ChartConfiguration, ChartData, ChartType } from 'chart.js';


@Component({
  selector: 'app-claim-report',
  templateUrl: './claim-report.component.html',
  styleUrls: ['./claim-report.component.css'],
  standalone: true,
  imports: [
    MatCardModule,
    MatTableModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatFormFieldModule,
    MatInputModule,
    FormsModule,
    CommonModule,
    NgChartsModule
  ]
})
export class ClaimReportComponent implements OnInit {
  @ViewChild(BaseChartDirective) chart?: BaseChartDirective;

  reportData: any;
  startDate: Date | undefined;
  endDate: Date | undefined;
  claims: any[] =[];
  displayedColumns: string[] = ['id', 'status', 'amount', 'submissionDate', 'claimType'];

  statuses:string[] = [
    "Initiated",
    "Surveyor Assigned",
    "Surveyor Approved",
    "Surveyor Rejected",
    "Adjustor Assigned",
    "Adjustor Approved",
    "Adjustor Rejected",
    "Work Started",
    "Work Completed - Payment Pending",
    "Payment Completed",
    "Completed & Delivered",
    "Pending"
  ];



  // Pie chart properties
  public pieChartOptions: ChartConfiguration['options'] = {
    responsive: true,
    plugins: {
      legend: {
        display: true,
        position: 'top',
      }
    }
  };
  public pieChartData: ChartData<'pie', number[], string | string[]> = {
    labels: this.statuses,
    datasets: [ { data: [] } ]
  };
  public pieChartType: ChartType = 'pie';


  constructor(private claimService: ClaimService,private cdr: ChangeDetectorRef) { }

  ngOnInit() {
    this.startDate = new Date(new Date().setMonth(new Date().getMonth() - 1));
    this.endDate = new Date();
    this.generateReport();
  }

  generateReport() {
    this.claimService.getClaims().subscribe(
      (claims) => {
        this.claims = claims.filter(claim => 
          this.startDate && this.endDate && 
          new Date(claim.IncidentDate) >= this.startDate &&
          new Date(claim.IncidentDate) <= this.endDate
        );
        this.reportData = this.processReportData(this.claims);
        this.updatePieChart();
        
        this.cdr.detectChanges();



      },
      (error) => {
        console.error('Error generating report:', error);
      }
    );
  }

  private processReportData(claims: any[]): any {
    const totalClaims = claims.length;
    const totalAmount = claims.reduce((sum, claim) => sum + claim.ClaimAmount, 0);
    const statusCounts = this.statuses.reduce((acc, status) => {
      acc[status] = claims.filter(claim => claim.Status === status).length;
      return acc;
    }, {} as { [status: string]: number });

    return {
      totalClaims,
      totalAmount,
      statusCounts
    };
  }

  getStatusColor(status: string): string {
    // Define colors for each status
    const statusColors: { [key: string]: string } = {
      'Initiated': '#FFA500',
      'Surveyor Assigned': '#1E90FF',
      'Surveyor Approved': '#32CD32',
      'Surveyor Rejected': '#FF6347',
      'Adjustor Assigned': '#9370DB',
      'Adjustor Approved': '#3CB371',
      'Adjustor Rejected': '#DC143C',
      'Work Started': '#4682B4',
      'Work Completed - Payment Pending': '#DAA520',
      'Payment Completed': '#228B22',
      'Completed & Delivered': '#008000'
    };

    return statusColors[status] || '#808080'; // Default to gray if status not found
  }

  private updatePieChart() {
    const nonZeroStatuses = this.statuses.filter(status => this.reportData.statusCounts[status] > 0);
    const nonZeroData = nonZeroStatuses.map(status => this.reportData.statusCounts[status]);

    this.pieChartData.labels = nonZeroStatuses;
    this.pieChartData.datasets[0].data = nonZeroData;

    this.cdr.detectChanges();
    if (this.chart) {
      this.chart.update();
    }
  }
}
