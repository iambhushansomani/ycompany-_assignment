import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ClaimInitiationComponent } from './claim-initiation/claim-initiation.component';
import { WorkshopSelectionComponent } from './workshop-selection/workshop-selection.component';
// import { DocumentUploadComponent } from './document-upload/document-upload.component';
// import { ClaimApprovalComponent } from './claim-approval/claim-approval.component';
import { ClaimListComponent } from './claim-list/claim-list.component';
import { ClaimReportComponent } from './claim-report/claim-report.component';
import { LoginComponent } from './login/login.component';
import { AuthGuard } from './guards/auth.guard';
import { MainLayoutComponent } from './main-layout/main-layout.component';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  {
    path: '',
    // component: MainLayoutComponent,
    canActivate: [AuthGuard],
    children: [
      { path: 'claim-initiation', component: ClaimInitiationComponent },
      { path: 'workshop-selection/:claimId', component: WorkshopSelectionComponent },
      // { path: 'document-upload/:claimId', component: DocumentUploadComponent },
      // { path: 'claim-approval/:claimId', component: ClaimApprovalComponent },
      { path: 'claim-list', component: ClaimListComponent },
      { path: 'claim-report', component: ClaimReportComponent },
      { path: '', redirectTo: '/claim-list', pathMatch: 'full' }
    ]
  },
  { path: '**', redirectTo: '/claim-list' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes,{ useHash: true })],
  exports: [RouterModule]
})
export class AppRoutingModule { }
