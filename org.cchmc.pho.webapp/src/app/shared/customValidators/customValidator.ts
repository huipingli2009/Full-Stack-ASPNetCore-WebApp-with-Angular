import { AbstractControl } from "@angular/forms";
import * as moment from "moment";

export function DateRequiredValidator(DateField: AbstractControl): { [key: string]: boolean } | null {
    const date = DateField.value;
    if (date === null || date === '') return { 'daterequired': true };
    return null;
}

// export function DateFormatValidator(format = "MM/dd/YYYY"): any {
//     return (control: AbstractControl): { [key: string]: any } => {
//         const val = moment(control.value, format, true);

//         if (!val.isValid()) {
//             return { invalidDateFormat: true };
//         }

//         return null;
//     };
// }