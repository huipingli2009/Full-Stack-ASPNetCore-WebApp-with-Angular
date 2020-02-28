import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { PhoMainComponent } from './pho-main/pho-main.component';


const routes: Routes = [{
  path: 'pho-main',
  component: PhoMainComponent,
  data: {
    title: 'Dashboard'
  }
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class PhoRoutingModule { }
