import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor, HttpXsrfTokenExtractor } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '@environments/environment';
import { AuthService } from '@app/shared/services';

@Injectable()
export class JwtInterceptor implements HttpInterceptor {
  constructor(private authService: AuthService, private tokenExtractor: HttpXsrfTokenExtractor) { }

  //HTTP Request Actions - AntiForgeryToken handling - data mofdifications
  private actions: string[] = ["POST", "PUT", "DELETE", "PATCH"];
  private forbiddenActions: string[] = ["HEAD", "OPTIONS"];

  // add auth header with jwt if user is logged in and request is to the api url

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        const user = this.authService.userValue;
        const isLoggedIn = (user != null && user.token != null);
        const isApiUrl :boolean = request.url.startsWith(environment.apiUrl);
        if (isLoggedIn && isApiUrl) {
            request = request.clone({
                setHeaders: {
                                Authorization: `Bearer ${user.token}`,
                                'Content-Type': 'application/json',
                            }
            });
        }

        //let token = this.tokenExtractor.getToken();
        //let permitted = this.findByActionName(request.method, this.actions);
        //let forbidden = this.findByActionName(request.method, this.forbiddenActions);

        //if (permitted !== undefined && forbidden === undefined && token !== null) {
        //  request = request.clone({ setHeaders: { "X-XSRF-TOKEN": token } });
        //}
        return next.handle(request);
    }

    private findByActionName(name: string, actions: string[]): string {
      return actions.find(action => action.toLocaleLowerCase() === name.toLocaleLowerCase());
    }
}
