import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NGXLogger } from 'ngx-logger';
import { environment } from 'src/environments/environment';
import { AuthenticationService } from '../services/authentication.service';


@Component({ templateUrl: 'login.component.html' })
export class LoginComponent implements OnInit {
    loginForm: FormGroup;
    error = '';
    loading = false;
    submitted = false;
    defaultUrl = environment.apiURL;
    loginHeaderImg: string;
    errorMessage;

    constructor(
        private formBuilder: FormBuilder,
        private authenticationService: AuthenticationService,
        private logger: NGXLogger
    ) { }

    ngOnInit() {
        this.loginHeaderImg = 'assets/img/TSCHS_LOGO_PMS-SPOT-COLOR.png';
        this.loginForm = this.formBuilder.group({
            username: ['', Validators.required],
            password: ['', Validators.required]
        });
    }

    // convenience getter for easy access to form fields
    get f() { return this.loginForm.controls; }


    onSubmit() {
        this.submitted = true;
        this.loading = true;

        this.logger.log('On Submit', this.loginForm.value);
        this.authenticationService.login(this.loginForm.value);
        this.authenticationService.loginErrorMsg.subscribe(res => {
            this.errorMessage = res;
            if (this.errorMessage !== '') {
                this.loading = false;
            }
        });
    }
}