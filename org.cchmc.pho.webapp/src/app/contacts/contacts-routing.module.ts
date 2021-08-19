import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { PracticeDetailComponent } from '../shared/practice-detail/practice-detail.component';

import { ContactsComponent } from './contacts.component';

const routes: Routes = [
  { 
    path: '', 
    component: ContactsComponent 
  },
  {
    path:'../shared/practice-detail',
    component: PracticeDetailComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ContactsRoutingModule { }
