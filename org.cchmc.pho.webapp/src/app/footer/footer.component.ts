import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { LegalDisclaimerComponent } from '../legal-disclaimer/legal-disclaimer.component';

@Component({
  selector: 'app-footer',
  templateUrl: './footer.component.html'
})
export class FooterComponent implements OnInit {
  footerDate = Date.now();

  constructor(private dialog: MatDialog) { }

  openDialog(): void {
    const dialogRef = this.dialog.open(LegalDisclaimerComponent, {});
  }

  ngOnInit(): void {
  }
}
