import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-mat-snack-bar',
  templateUrl: './mat-snack-bar.component.html'
})
export class MatSnackBarComponent {

  constructor(public snackBar: MatSnackBar) { }

  // this function will open up snackbar on bottom center position with custom background color (defined in css)
  openSnackBar(message: string, action: string, className: string) {

    this.snackBar.open(message, action, {
      duration: 5000,
      verticalPosition: 'bottom',
      horizontalPosition: 'center',
      panelClass: [className],
    });
  }

}
