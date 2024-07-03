import { Component, OnInit } from '@angular/core';
import { State} from '@app/models'
import { CommonService, StateService} from '@app/shared/services'
import CustomStore from 'devextreme/data/custom_store';
import { HttpClient } from '@angular/common/http';
import { environment } from '@environments/environment';


@Component({
  selector: 'app-grid-sample',
  templateUrl: './grid-sample.component.html',
  styleUrls: ['./grid-sample.component.scss'],
})


export class GridSampleComponent implements OnInit {

  isLoading: boolean = false;
  url: string =  environment.apiUrl + '/users'
  usersCustomStore: CustomStore;
  loadPanelPosition = { of: '#gridContainer' };

  clockRegex: any = '^[0-9]{4}$';      //4 numeric characters

  states: State[];

  //@Common
  applyFilterTypes: any;
  currentFilter: any;


  constructor(private http: HttpClient, private stateService: StateService) {  }

  onShown() {
      setTimeout(() => {
        this.isLoading = false;
      }, 2000);
  }



  async ngOnInit() {

    this.applyFilterTypes = CommonService.applyFilterTypes;
    this.currentFilter = CommonService.currentFilter;


    this.isLoading = true;
    await this.loadUsers();
    await this.loadStates();
    this.isLoading = false;
  }


  ///
  /// Set Up User Grid CRUD operations
  ///
  async loadUsers() {

    this.usersCustomStore = new CustomStore({
      key: 'id',
      load: () => {
        return this.http.get(this.url )
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
      update: (key, values) => {

        let url = this.url + "/" + key;

        return this.http.put(url, JSON.stringify(values) )
          .toPromise()
          .then(result => {
            console.log(result);
            return result;
          });
      },
      remove: (key) => {
        let body = [
          {
            "value": "false",
            "path": "/IsActive",
            "op": "replace"
          }
        ];

        let url = this.url + "/" + key;

        return this.http.patch(url, JSON.stringify(body) )
          .toPromise()
          .then(result => {
            console.log(result);
          });
      },

    });
  }

  async loadStates() {
    this.stateService.getStates()
      .subscribe(result => {
        this.states = result;
      },
        error => console.error(error));

  }


  onContentReady(e: any) {
    e.component.option("loadPanel.enabled", this.isLoading);
  }


  setfullName(data) {
    return [data.firstName, data.lastName].join(" ");
  }

 

  onRowUpdating(e: any) {
    e.newData = Object.assign({}, e.oldData, e.newData);
  }
}





