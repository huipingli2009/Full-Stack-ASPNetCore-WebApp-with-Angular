import { Injectable } from '@angular/core';
import { CanActivate, CanActivateChild, CanDeactivate, CanLoad, Route, UrlSegment, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { UserService } from '../services/user.service';
import { take } from 'rxjs/operators';
import { AuthenticationService } from '../services/authentication.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(private userService: UserService,
              private router: Router,             
              private authenticationService: AuthenticationService) { }        
  
       
  currentUserRole: string;  

  //get current and user and user's role
  currentUser = this.userService.getCurrentUser().pipe(take(1)).subscribe((data) => {
    this.currentUserRole = data.role.name;     
  });

  canActivate(
    next: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean> | Promise<boolean> | boolean {
       
    if (this.authenticationService.isUserLoggedIn !== true) {
      this.router.navigate(['/login'])
    }
    else
    { 
      //get roles allowed from the route user wants to navigate to  
      const allowedRoles = next.data.role;

      //check if curren user is in the role(s) allowed by the route
      const isAuthorized = this.isAuthorized(allowedRoles, this.currentUserRole);
      
      if (!isAuthorized) {
        this.router.navigate(['/dashboard']);
      }
      return isAuthorized;      
    }   
  }

  isAuthorized(routeRoles: string[], userRole: string): boolean {    
    
    // check if no role restrictions for the route requested, if it is, authorize the user to access the page
    if (routeRoles == null || routeRoles.length === 0) {
      return true;
    }    

    // check if the user role(s) is in the list of allowed route roles, return true if it is and false if it isn't
    return routeRoles.includes(userRole);   
  }

  //Leave the following methods here for additional developments in the future
  // canActivateChild(
  //   childRoute: ActivatedRouteSnapshot,
  //   state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
  //   return false;
  // }
  // canDeactivate(
  //   component: unknown,
  //   currentRoute: ActivatedRouteSnapshot,
  //   currentState: RouterStateSnapshot,
  //   nextState?: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
  //   return true;
  // }
  // canLoad(
  //   route: Route,
  //   segments: UrlSegment[]): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
  //   return true;
  // }
}
