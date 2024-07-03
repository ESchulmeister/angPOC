import {  Component, Input, AfterViewInit} from '@angular/core';
import { environment } from '@environments/environment';
import CustomStore from 'devextreme/data/custom_store';
import { HttpClient } from '@angular/common/http';
import { CommonService } from '@app/shared/services'


@Component({
  selector: 'app-detail-grid',
  templateUrl: './detail-grid.component.html',
  styleUrls: ['./detail-grid.component.scss'],
  preserveWhitespaces: true

})
export class DetailGridComponent implements  AfterViewInit {

  @Input() key: number;      //master application id
  permissionsCustomStore: CustomStore;
  url :string;

  applyFilterTypes: any;
  currentFilter: any;

  constructor(private http: HttpClient) {
  //  this.applyFilterTypes = Common.applyFilterTypes;
  //  this.currentFilter = Common.currentFilter;
  }


  async ngAfterViewInit() {
    this.url = environment.apiUrl + '/permissions?appId=' + this.key;

    this.applyFilterTypes = CommonService.applyFilterTypes;
    this.currentFilter = CommonService.currentFilter;


    await this.setupDataSource();
  
  }

  async setupDataSource() {

    this.permissionsCustomStore = new CustomStore({
      key: 'id',
      load: () => {
        return this.http.get(this.url  )
          .toPromise()
          .then(result => {
            console.log(result);
            return result;
          });
      },
      insert: (values) => {
        return this.http.post(this.url, JSON.stringify(values) )
          .toPromise()
          .then(result => {
            console.log(result);
            return result;
          });
      },
 
    });
  }
}


