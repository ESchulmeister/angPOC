import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { AppComponent } from '@app/app.component';
import { SideNavOuterToolbarModule, SideNavInnerToolbarModule } from './layouts';
import { FooterModule } from './shared/components';
import { AuthService, ScreenService, AppInfoService } from './shared/services';
import { UnauthenticatedContentModule } from './unauthenticated-content';
import { AppRoutingModule } from './app-routing.module';
import { ErrorInterceptor, JwtInterceptor } from './helpers';
import { JwtModule } from "@auth0/angular-jwt"


@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    SideNavOuterToolbarModule,
    SideNavInnerToolbarModule,
    FooterModule,
    UnauthenticatedContentModule,
    AppRoutingModule,
    HttpClientModule,


    JwtModule.forRoot({
      config: {
        tokenGetter: () => localStorage.getItem('currentUser')
      }
    })
  ],
  providers: [
    AuthService, ScreenService, AppInfoService, 
    { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
