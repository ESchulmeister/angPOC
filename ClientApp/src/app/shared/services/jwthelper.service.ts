import { Injectable } from '@angular/core';
import { JwtHelperService } from '@auth0/angular-jwt';

@Injectable({
  providedIn: 'root'
})
export class jwtHelper {

  constructor(private jwtService: JwtHelperService) { }

  decodeToken(token) {
    return this.jwtService.decodeToken(token);
  }

  hasTokenExpired(token) {
    let decodedToken = this.decodeToken(token);
    if (decodedToken == null) {
      return true;
    }

    const expiryTime: number = decodedToken.exp;
    if (expiryTime == null) {
      return false;
    }
    return (((1000 * expiryTime) - (new Date()).getTime()) < 5000);
  }
}
