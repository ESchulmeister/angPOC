import { CommonModule } from '@angular/common';
import { Component, NgModule, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { DxFormModule } from 'devextreme-angular/ui/form';
import { DxLoadIndicatorModule } from 'devextreme-angular/ui/load-indicator';
import { FormsModule, ReactiveFormsModule} from '@angular/forms';
import { AuthService } from '@app/shared/services';
import { DxTextBoxModule } from 'devextreme-angular';
import { first } from 'rxjs/operators';
import notify from 'devextreme/ui/notify';

@Component({
  selector: 'app-login-form',
  templateUrl: './login-form.component.html',
  styleUrls: ['./login-form.component.scss'],
  preserveWhitespaces: true
})


export class LoginFormComponent  implements OnInit{
  formData: any = {};
  loading: boolean = false;
  errorMsgInterval: number = 5000;


  iconInvisible: string =
    'data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABgAAAAYCAQAAABKfvVzAAAABGdBTUEAALGPC/xhBQAAACBjSFJNAAB6JgAAgIQAAPoAAACA6AAAdTAAAOpgAAA6mAAAF3CculE8AAAAAmJLR0QAAKqNIzIAAAAJcEhZcwAADsQAAA7EAZUrDhsAAAAHdElNRQfkAxoRLTO7G8yhAAABeElEQVQ4y53Uz0uUURTG8c/k4KKhAqdEiZAIpGXLoB+bcGULMSlGGogKElroRqioRS0CKQjxDxjShfgXtKiN0KYIJYRaKklQku3VmTster3emYGY6dzVe87zfd73cM59+a843Zn8tl3ldsV5HJVXwQJyzrnijD45275676PQik0JqsooWlVvOOseKvwbWWlC6r4ZbgYmBfUM6XFZQcFF80KG1DxO5U+jV9V4g9Go3Vh7sp+8Hn0OkLzn7shhOqldg16/YmLbtJAhRSteoNtWrG85wcvEYQkP1FWVUPRJEUuJYoaN5HEWF7IPK6HHKcwlio1DKkmL/fhiD10WlPy2iZOJ4jXHbCY9HMaj2H4JLCfzOAJDajH1DFxV8SbpZTUzGNp/0UQEau4l00+R4H46oLt2IvTWmEFnjfueIDeal+O8zy071NhLS3S55UOTOFgTUiTXgg24ZNBxwU/r3vlh0itB2WK7l+zvLlfdbB9gyo6RToCOfxQx/gAUl9Y+isex4QAAACV0RVh0ZGF0ZTpjcmVhdGUAMjAyMC0wMy0yNlQxNzo0NTo1MSswMDowMHB8Q0AAAAAldEVYdGRhdGU6bW9kaWZ5ADIwMjAtMDMtMjZUMTc6NDU6NTErMDA6MDABIfv8AAAAGXRFWHRTb2Z0d2FyZQB3d3cuaW5rc2NhcGUub3Jnm+48GgAAAABJRU5ErkJggg==';
  iconVisible: string =
    'data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABgAAAAYCAQAAABKfvVzAAAABGdBTUEAALGPC/xhBQAAACBjSFJNAAB6JgAAgIQAAPoAAACA6AAAdTAAAOpgAAA6mAAAF3CculE8AAAAAmJLR0QAAKqNIzIAAAAJcEhZcwAADsQAAA7EAZUrDhsAAAAHdElNRQfkAxoRKDkmudH6AAABSUlEQVQ4y93Sv0ocURgF8N+EFO6UrhsQLFWIIpF0QUlpIxbB0sfQfYGonUTyClGJWWMXhVR5AndwieID2Cg4skawUJkUXq+TXbE3323ud865f87H4flX0oVUzRg3gGOZH86eOj6m4UZRWte+GX1cnPrsVqHQtmbegjVthcKtVZVO+bDDcOemakT7NAJ6YKgsn3Qe5YnUskxmSSqxFZjcxL38vcsAtlWlWk79Uii0pGouAntpEkbk0eIXLDvR70NAFrEe+dwIR6WZLCDz04yrgDRRLymyl6665jVlKu6LDq7nhTnnsX2DnX8EOxiPXW72bkYPpvukWvED+ypeRdN/HuY0EY03JCoWNTV9VJHYDsyZd+WnhxwEYkstojXfA/rbYHc0PoVoXFhXV7cRPnNjpTsadzVq03VH+L56/XS8e017G+K9Z1fuv6u/LnGn0KoaVLwAAAAldEVYdGRhdGU6Y3JlYXRlADIwMjAtMDMtMjZUMTc6NDA6NTcrMDA6MDD1hb0+AAAAJXRFWHRkYXRlOm1vZGlmeQAyMDIwLTAzLTI2VDE3OjQwOjU3KzAwOjAwhNgFggAAABl0RVh0U29mdHdhcmUAd3d3Lmlua3NjYXBlLm9yZ5vuPBoAAAAASUVORK5CYII=';

  fieldPasswordMode: string = 'password';
  iconPassword: string;
  buttonPasswordEye: any = []; 


  constructor(private authService: AuthService, private router: Router, private route: ActivatedRoute )
  {

    this.iconPassword = this.iconInvisible;

  }
    ngOnInit(): void {
      this.setupBtnPasswordEye();
    }

  //password field  image/text
  async setupBtnPasswordEye() {

    let toogleBtnFunction = () => {
      let builtButton = [
        {
          name: 'passwordEye',
          location: 'after',
          options: {
            icon: this.iconPassword,
            stylingMode: 'underlined',
            onClick: () => {
              this.fieldPasswordMode =
                this.fieldPasswordMode === 'text' ? 'password' : 'text';
              this.iconPassword =
                this.fieldPasswordMode === 'text'
                  ? this.iconVisible
                  : this.iconInvisible;
            }
          }
        }
      ];
      this.buttonPasswordEye = builtButton;
    };
    toogleBtnFunction();
  }

  async onSubmit(e: any) {
    e.preventDefault();
    const { username, password } = this.formData;
    this.loading = true;

    //not used

    //this.authService.logIn(username, password)
    //  .pipe(first()) 
    //  .subscribe
    //  ({
    //      next: () => {
    //        let returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
    //        this.router.navigateByUrl(returnUrl);
    //      },
    //      error: error => {

    //        //notification - red div  @ login
    //        //error could be returned from sql - stored procedure THROW 
    //        //or be general
    //        notify({ message: error  , width:'auto' }, 'error', this.errorMsgInterval);

    //        this.loading = false;
    //      }
    //    }
    //  )
  }
}



@NgModule({
  imports: [
    CommonModule,
    RouterModule,
    DxFormModule,
    DxTextBoxModule,
    DxLoadIndicatorModule, 
    FormsModule,
    ReactiveFormsModule
  ],
  declarations: [LoginFormComponent],
  exports: [LoginFormComponent]
})
export class LoginFormModule { }
