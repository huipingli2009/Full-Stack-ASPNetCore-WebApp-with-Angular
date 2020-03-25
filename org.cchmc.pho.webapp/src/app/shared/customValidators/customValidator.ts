import { AbstractControl } from "@angular/forms";
import * as moment from "moment";

export function DateRequiredValidator(DateField: AbstractControl): { [key: string]: boolean } | null {
    const date = DateField.value;
    if (date === null || date === '') return { 'daterequired': true };
    return null;
}

