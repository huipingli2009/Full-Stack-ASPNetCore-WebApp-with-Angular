import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DashboardComponent } from './dashboard/dashboard.component';
import { FilesComponent } from './files/files.component';
import { AuthGuard } from './helpers/auth.guard';
import { LoginLayoutComponent } from './layouts/login-layout.component';
import { MainLayoutComponent } from './layouts/main-layout.component';
import { LoginComponent } from './login/login.component';
import { PatientsComponent } from './patients/patients.component';
import { ReportComponent } from './report/report.component';
import { StaffComponent } from './staff/staff.component';
import { WorkbooksComponent } from './workbooks/workbooks.component';

const routes: Routes = [
  {
    path: '',
    component: MainLayoutComponent,
    canActivate: [AuthGuard],
    children: [
      {
        path: '',
        component: DashboardComponent,
      },
      {
        path: 'dashboard',
        component: DashboardComponent,
        data: {
          title: 'Dashboard' // Keeping this as ex. in case we need to use Dynamic switching for menu titles etc.
        }
      },
      {
        path: 'patients',
        component: PatientsComponent,
        canActivate: [AuthGuard]
      },
      {
        path: 'staff',
        component: StaffComponent,
      },
      {
        path: 'files',
        component: FilesComponent,
      },
      {
        path: 'workbooks',
        component: WorkbooksComponent,
      },
    ]
  },
  {
    path: '',
    component: LoginLayoutComponent,
    children: [
      {
        path: 'login',
        component: LoginComponent
      },
      {
        path: 'edreport',
        component: ReportComponent

      }
    ]
  },
  { path: '**', redirectTo: '' }];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }