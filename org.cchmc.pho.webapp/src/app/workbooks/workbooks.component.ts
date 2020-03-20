import { Component, OnInit } from '@angular/core';
import { RestService } from '../rest.service';
import { WorkbookReportingMonths, WorkbookProvider, WorkbookPatient } from '../models/workbook';
import { DatePipe } from '@angular/common';
import { NGXLogger } from 'ngx-logger';
import { FormBuilder, FormControl, FormArray } from '@angular/forms';
import { ChangeDetectorRef } from '@angular/core';
@Component({
  selector: 'app-workbooks',
  templateUrl: './workbooks.component.html',
  styleUrls: ['./workbooks.component.scss']
})
export class WorkbooksComponent implements OnInit {
  workbookReportingMonths: WorkbookReportingMonths[];
  workbookProviders: WorkbookProvider[];
  workbookPatient: WorkbookPatient[];
  workbookProviderDetail: WorkbookProvider;
  formResponseId: number;
  phqsFinal: number;
  totalFinal: number;
  constructor(private rest: RestService, private fb: FormBuilder, private datePipe: DatePipe, private logger: NGXLogger, private ref: ChangeDetectorRef) { }

  ngOnInit(): void {
    this.getWorkbookReportingMonths();
    this.onProviderValueChanges();
  }

  selectedFormResponseID = new FormControl('');

 
  ProvidersForWorkbookForm = this.fb.group({
    ProviderWorkbookArray: this.fb.array([
      this.fb.group({
        formResponseID: [''],
        staffID: [''],
        provider: [''],
        phqs: [''],
        total: ['']
      })
    ])
  });
  onProviderValueChanges(): void {
    this.ProvidersForWorkbookForm.get('ProviderWorkbookArray').valueChanges.subscribe(values => {
      // reset the total amount  
      this.phqsFinal = 0;
      this.totalFinal = 0;
      const ctrl = <FormArray>this.ProvidersForWorkbookForm.controls['ProviderWorkbookArray'];
      // iterate each object in the form array
      ctrl.controls.forEach(x => {
        // get the itemmt value and need to parse the input to number
        let parsedphqs = parseInt(x.get('phqs').value)
        let parsedtotal = parseInt(x.get('total').value)
        // add to total
        this.phqsFinal += parsedphqs
        this.totalFinal += parsedtotal
        this.ref.detectChanges()
      });
    })
  }

  updateWorkbookProviders(workbookProviderDetails: WorkbookProvider) {

    this.rest.updateWorkbookForProvider(workbookProviderDetails).subscribe(res => {
      this.logger.log("workbook");
    })
  }

  onProviderWorkbookChange(index: number) {
    var provider = this.ProviderWorkbookArray.at(index);

    console.log(this.workbookProviderDetail);
    this.workbookProviderDetail = provider.value;
    this.workbookProviderDetail.phqs = Number(this.workbookProviderDetail.phqs);
    this.workbookProviderDetail.total = Number(this.workbookProviderDetail.total);
 
    this.updateWorkbookProviders(this.workbookProviderDetail);
    this.logger.log(`logging on input change:${index}`);
  }


  AssignProviderWorkbookArray() {
    this.logger.log(this.workbookProviders);
    const providerArray = this.ProviderWorkbookArray;
    while (providerArray.length) {
      providerArray.removeAt(0);
    }

    this.workbookProviders.forEach(provider => {
      providerArray.push(this.fb.group(provider));
    });
    // providerArray.patchValue(this.workbookProviders);
    this.logger.log(this.ProviderWorkbookArray.value);
    this.logger.log(this.ProvidersForWorkbookForm.value);
  }

  get ProviderWorkbookArray() {
    return this.ProvidersForWorkbookForm.get('ProviderWorkbookArray') as FormArray;
  }

  getWorkbookReportingMonths() {
    this.rest.getWorkbookReportingMonths().subscribe((data) => {
      this.workbookReportingMonths = data;
      this.workbookReportingMonths.forEach((element, index, reportData) => {
        this.workbookReportingMonths[index].reportingMonth = this.datePipe.transform(this.workbookReportingMonths[index].reportingMonth, 'MM/dd/yyyy');
      });
    })
  }

  onReportingDateSelectionChange() {
    this.formResponseId = this.selectedFormResponseID.value;
    this.getWorkbookProviders(this.formResponseId);
    this.getWorkbookPatients(this.formResponseId);


  }
  getWorkbookProviders(formResponseid: number) {
    this.rest.getWorkbookProviders(formResponseid).subscribe((data) => {
      this.workbookProviders = data;
      this.AssignProviderWorkbookArray();
      //this.ProviderArray.setValue(data);

    })
  }

  getWorkbookPatients(formResponseid: number) {
    this.rest.getWorkbookPatients(formResponseid).subscribe((data) => {
      this.workbookPatient = data;
      // this.AssignProviderWorkbookArray();
      //this.ProviderArray.setValue(data);

    })
  }


}
