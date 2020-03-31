import { AbstractControl } from "@angular/forms";

export function DateRequiredValidator(DateField: AbstractControl): { [key: string]: boolean } | null {
    const date = DateField.value;
    if (date === null || date === '') return { 'daterequired': true };
    return null;
}

