import { Component, NgModule, Input, OnInit} from '@angular/core';
import { CommonModule } from '@angular/common';
import { DxListModule } from 'devextreme-angular/ui/list';
import { DxContextMenuModule } from 'devextreme-angular/ui/context-menu';
import { User } from '@app/models'
import { AuthService } from '@app/shared/services';


@Component({
  selector: 'app-user-panel',
  templateUrl: 'user-panel.component.html',
  styleUrls: ['./user-panel.component.scss']
})

export class UserPanelComponent  implements OnInit {
  user: User

  @Input()
  menuItems: any;

  @Input()
  menuMode = '';


  constructor(private authService: AuthService) { }

   async ngOnInit() {
    this.user = this.authService.userValue;

  }
}

@NgModule({
  imports: [
    DxListModule,
    DxContextMenuModule,
   CommonModule
  ],
  declarations: [ UserPanelComponent ],
  exports: [ UserPanelComponent ]
})
export class UserPanelModule { }
