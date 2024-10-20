import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { CommonModule } from '@angular/common';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientModule } from '@angular/common/http';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatListModule } from '@angular/material/list';
import { MatTableModule } from '@angular/material/table';
import { MatCardModule } from '@angular/material/card';
import { MatToolbarModule } from '@angular/material/toolbar';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { ClaimInitiationComponent } from './claim-initiation/claim-initiation.component';
import { WorkshopSelectionComponent } from './workshop-selection/workshop-selection.component';
// import { DocumentUploadComponent } from './document-upload/document-upload.component';
// import { ClaimApprovalComponent } from './claim-approval/claim-approval.component';
import { ClaimListComponent } from './claim-list/claim-list.component';
import { ClaimReportComponent } from './claim-report/claim-report.component';
import { MainLayoutComponent } from './main-layout/main-layout.component';
import { AssessmentAdjustmentDialogComponent } from './claim-list/assessment-adjustment-dialog.component';

@NgModule({
  declarations: [
    AppComponent,
    ClaimInitiationComponent,
    WorkshopSelectionComponent,
    // DocumentUploadComponent,
    // ClaimApprovalComponent,
    ClaimListComponent,
    ClaimReportComponent,
    MainLayoutComponent,
    AssessmentAdjustmentDialogComponent
  ],
  imports: [
    BrowserModule,
    CommonModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    HttpClientModule,
    ReactiveFormsModule,
    FormsModule,
    MatButtonModule,
    MatInputModule,
    MatFormFieldModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatListModule,
    MatTableModule,
    MatCardModule,
    MatToolbarModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }