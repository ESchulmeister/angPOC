import { Component, OnInit } from '@angular/core';
import { User , State} from '@app/models'
import { AuthService, StateService }  from '@app/shared/services'

@Component({
  templateUrl: 'profile.component.html',
  styleUrls: [ './profile.component.scss' ]
})


export class ProfileComponent implements OnInit {
  user: User;
  colCountByScreen: object;

  states: State[];

  constructor(private authService: AuthService, private stateService: StateService) {


    this.colCountByScreen = {
      md: 4,
      sm: 2
    };

  }

  screen(width) {
    return width < 720 ? 'sm' : 'md';
  }



  async ngOnInit() {

    this.user = this.authService.userValue;

    this.stateService.getStates() 
      .subscribe(result => {
      this.states = result;
      },
      error => console.error(error));
  }
}
