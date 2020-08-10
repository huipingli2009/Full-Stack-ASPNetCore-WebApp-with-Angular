import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map, take } from 'rxjs/operators';
import { DrilldownComponent } from '../drilldown/drilldown.component';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

@Injectable()
export class DrilldownService {  
  
    constructor(private dialog: MatDialog) { }  

    dialogRef: MatDialogRef<DrilldownComponent>;
  
    public open(options) 
    {
        this.dialogRef = this.dialog.open(DrilldownComponent, {    
            data: {
              measureId: options.measureId,
              filterId: options.filterId,
              displayText: options.displayText
            }
       });  
    }  
  
    public confirmed(): Observable<any> {
    
        return this.dialogRef.afterClosed().pipe(take(1), map(res => {
            return res;
          }
        ));
      }

}