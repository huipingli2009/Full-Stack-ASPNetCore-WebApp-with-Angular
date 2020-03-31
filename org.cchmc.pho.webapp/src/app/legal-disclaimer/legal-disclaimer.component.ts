import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { NGXLogger } from 'ngx-logger';
import { RestService } from '../rest.service';

@Component({
  selector: 'app-legal-disclaimer',
  templateUrl: './legal-disclaimer.component.html',
  styleUrls: ['./legal-disclaimer.component.scss']
})
export class LegalDisclaimerComponent implements OnInit {
  currentUserId: number;
  constructor(public dialogRef: MatDialogRef<LegalDisclaimerComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private rest: RestService,
    private logger: NGXLogger) { }

  ngOnInit(): void { }

  SaveLegalDisclaimer() {
    this.rest.updateUserLegalDisclaimer().subscribe(
      (data: boolean) => {
        this.logger.log(`Save Disclaimer success with value of : ${data}`, data)
      }
    ).add(() => {
      //using this similar to a finally... so this dialog closers after a response is received.
      this.dialogRef.close();
    });

  }

}
