// https://medium.com/@amcdnl/version-stamping-your-app-with-the-angular-cli-d563284bb94d

import { Component, OnInit } from '@angular/core';
import { VERSION } from '../../environments/version';


@Component({
  selector: 'app-version',
  //todo: add class file here
  template: `
  <div class="">{{appVersion}}</div>
  `
})

export class VersionComponent implements OnInit {
  appVersion = `v.${VERSION.version}-${VERSION.hash}`;

  constructor() { }

  ngOnInit() {

  }

}