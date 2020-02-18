import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { PhoRoutingModule } from './pho-routing.module';
import { PhoMainComponent } from './pho-main/pho-main.component';
import { MainMenuComponent } from './main-menu/main-menu.component';


@NgModule({
  declarations: [PhoMainComponent, MainMenuComponent],
  imports: [
    CommonModule,
    PhoRoutingModule
  ]
})
export class PhoModule { }
