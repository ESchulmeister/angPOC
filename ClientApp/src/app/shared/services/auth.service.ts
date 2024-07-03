import { Injectable, OnInit } from '@angular/core';
import { CanActivate, Router, ActivatedRouteSnapshot} from '@angular/router';
import { environment } from '@environments/environment';
import { HttpClient} from '@angular/common/http';
import { BehaviorSubject } from 'rxjs';
import { first, map } from 'rxjs/operators';
import { User } from '@app/models';
import { jwtHelper } from '../services/jwthelper.service';


const defaultPath = '/';

@Injectable({ providedIn: 'root' })

export class AuthService  {
  currentUserSubject: BehaviorSubject<User>;
  localStorageUser: any;

  get loggedIn(): boolean {
    return (this.currentUserSubject != null) && (this.currentUserSubject.value != null);
  }

  public _lastAuthenticatedPath: string = defaultPath;
    set lastAuthenticatedPath(value: string) {
    this._lastAuthenticatedPath = value;
  }

  constructor(private http: HttpClient, private router: Router)
  {
    this.localStorageUser = JSON.parse(localStorage.getItem('currentUser'));

    this.currentUserSubject = new BehaviorSubject<User>(this.localStorageUser);
  }


 
  public get userValue(): User {
    return this.currentUserSubject.value;
  }


  //user details - profile
  getUserDetails(jwt: string) {

    const url = environment.loginUrl + '/api/home/?jwt=' + jwt;

    //send request
    return this.http.get<User>(url)
      .pipe(map(user => {

        localStorage.setItem('currentUser', JSON.stringify(user));
        this.currentUserSubject.next(user);

        return user;
      }));
  }

  /// remove user from local storage and set current user to null
  logOut() {
     const url = environment.apiUrl + '/users/logout';

    //send request
    return this.http.post(url, null)
      .toPromise()
      .then(result => {
        localStorage.removeItem('currentUser');
        this.currentUserSubject.next(null);
        this.redirectToSso();
      });
  }

  public redirectToSso() {
    const url = environment.loginUrl + '?appID=' + environment.appID +
            '&returnUrl=' + environment.apiUrl.replace('api', '');
    window.location.href = url;
  }

}

@Injectable()
export class AuthGuardService implements CanActivate {

  jwt: string;

  constructor(private router: Router, private authService: AuthService, private jwtService: jwtHelper) {
      this.extractJwt();
  }


  canActivate(route: ActivatedRouteSnapshot): boolean {

    let isLoggedIn = this.authService.loggedIn;

    const isAuthForm = false;

    if (isLoggedIn && isAuthForm) {
      this.authService.lastAuthenticatedPath = defaultPath;
      this.router.navigate([defaultPath]);
      return false;
    }


    if (!isLoggedIn && !isAuthForm) {

      if (this.jwt == undefined || this.jwt == null) {
        this.authService.redirectToSso();

      }
      else {
        // call a method to get the User by token

        this.authService.getUserDetails(this.jwt)
        .pipe(first())
        .subscribe
        ({
            next: () => {
              if (!this.jwtService.hasTokenExpired(this.jwt)) {
                isLoggedIn = true;
                this.authService.lastAuthenticatedPath = defaultPath;
                this.router.navigate([defaultPath]);
                return true;
              }

            },
          error:
            err => {  console.log('Error:', err);          }
          }
        )

      }

      return true;
    }

    if (isLoggedIn) {
      this.authService.lastAuthenticatedPath = route.routeConfig.path;
    }

    let localStorageUser = this.authService.localStorageUser; //JSON.parse(localStorage.getItem('currentUser'));
    if (localStorageUser != null && this.jwtService.hasTokenExpired(localStorageUser.token)) {
      this.authService.logOut();
    }

    return isLoggedIn || isAuthForm;
  }


  ///extract jwt token
  extractJwt() {
    const queryString = location.search.replace('?', '');
    const parts = queryString.split('&');

    if (parts.length == 0) {
      return;
    }

    const jwtPart = parts.find(p => p.indexOf('jwt') == 0);
    if (jwtPart == undefined || jwtPart == null) {
      return;
    }

    const jwtParamParts = jwtPart.split('=');

    if (jwtParamParts.length > 0) {
      this.jwt = jwtParamParts[1];
    }

  }

 }
