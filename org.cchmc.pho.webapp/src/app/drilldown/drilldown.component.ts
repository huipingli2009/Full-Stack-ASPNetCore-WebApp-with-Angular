import { Component, OnInit, Inject, HostListener } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';


@Component({
  selector: 'app-drilldown',
  templateUrl: './drilldown.component.html',
  styleUrls: ['./drilldown.component.scss']
})



export class DrilldownComponent implements OnInit {

  selectedMeasureId: string;

  constructor(@Inject(MAT_DIALOG_DATA) public data: {
    measureId: string
}, private mdDialogRef: MatDialogRef<DrilldownComponent>)
{
  this.selectedMeasureId = data.measureId;
}

  ngOnInit(): void {
  }

  public cancel() {
    this.close(false);
  }
  public close(value) {
    this.mdDialogRef.close(value);
  }
  public confirm() {
    this.close(true);
  }
  @HostListener("keydown.esc") 
  public onEsc() {
    this.close(false);
  }

}
