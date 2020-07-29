import { CommonModule } from '@angular/common';
import { NgModule} from '@angular/core';
import { DrilldownService } from '../services/drilldown.service';
import { MatDialogModule } from '@angular/material/dialog';
import { DrilldownComponent } from './drilldown.component';
@NgModule({
    imports: [
        CommonModule,
        MatDialogModule
    ],
    declarations: [
        DrilldownComponent
    ],
    exports: [DrilldownComponent],
    entryComponents: [DrilldownComponent],
    providers: [DrilldownService]
  })
  export class ConfirmDialogModule {
  }