import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthenticationService } from '../services/authentication.service';


@Injectable({ providedIn: 'root' })
export class AuthGuard implements CanActivate {
  constructor(
    private router: Router,
    private authenticationService: AuthenticationService
  ) { }

  // canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
  //     const currentUser = this.authenticationService.currentUserValue;
  //     if (currentUser) {
  //         // logged in so return true
  //         return true;
  //     }

  //     // not logged in so redirect to login page with the return url
  //     this.router.navigate(['/login'], { queryParams: { returnUrl: state.url } });
  //     return false;
  // }
  canActivate(
    next: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean> | Promise<boolean> | boolean {
    if (this.authenticationService.isUserLoggedIn !== true) {
      window.alert("Access not allowed!"); //TODO: Remove for Prod
      this.router.navigate(['/login'])
    }
    return true;
  }
}