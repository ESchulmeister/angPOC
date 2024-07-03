import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable,  throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthService } from '../shared/services';

@Injectable()

export class ErrorInterceptor implements HttpInterceptor {

  errorMessage: string;

  constructor(private authService: AuthService) {}

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {

      return next.handle(request).pipe(catchError(err => {

        console.log('error:',err);

        // auto logout if 401 or 403 response returned from api
        if ([401, 403].includes(err.status) && this.authService.loggedIn) {
            this.authService.logOut();
        }

        //Error Text
        const error = (err.error == null || err.error == undefined) ? err.statusText :
                        (err.error.message == undefined) ? err.error.Message : err.error.message;

        //Throw error - e.g will be shown @ datagrid error row
        return throwError(error);


        }))
    }
}


