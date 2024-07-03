import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Routes, RouterModule } from '@angular/router';
import { AuthGuardService } from './shared/services';
import { HomeComponent } from './pages/home/home.component';
import { ProfileComponent } from './pages/profile/profile.component';
import { GridSampleComponent } from './pages/grid-sample/grid-sample.component';
import { ComplexGridComponent } from './pages/complex-grid/complex-grid.component';
import { DetailGridComponent } from './pages/detail-grid/detail-grid.component';
import {
  DxDataGridModule,
  DxFormModule,
  DxButtonModule,
  DxTextBoxModule,
  DxLoadPanelModule,
  DxTemplateModule,
  DxDateBoxModule,
  DxDropDownBoxModule
} from 'devextreme-angular';


const routes: Routes = [
  { path: 'complex-grid',    component: ComplexGridComponent,    canActivate: [ AuthGuardService ]  },
  { path: 'grid-sample', component: GridSampleComponent, canActivate: [ AuthGuardService ] },
  { path: 'home',  component: HomeComponent,  canActivate: [ AuthGuardService ]  },
  { path: 'profile', component: ProfileComponent, canActivate: [AuthGuardService] },
  { path: '**', redirectTo: 'home'  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes, { useHash: true }),
    CommonModule,
    DxDataGridModule,
    DxFormModule,
    DxButtonModule,
    DxTextBoxModule,
    DxLoadPanelModule,
    DxTemplateModule,
    DxDropDownBoxModule,
    DxDateBoxModule
  ],
    providers: [AuthGuardService],
    exports: [RouterModule],
  declarations: [HomeComponent,
                ProfileComponent,
                GridSampleComponent,
                ComplexGridComponent,
                DetailGridComponent]
})
export class AppRoutingModule { }
