import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { MainMenuComponent } from './main-menu/main-menu.component';
import { PhoMainComponent } from './pho-main/pho-main.component';
import { PhoRoutingModule } from './pho-routing.module';



@NgModule({
  declarations: [PhoMainComponent, MainMenuComponent],
  imports: [
    CommonModule,
    PhoRoutingModule
  ]
})
export class PhoModule { }
