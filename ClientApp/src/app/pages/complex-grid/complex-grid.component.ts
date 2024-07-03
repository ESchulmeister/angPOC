import { Component, OnInit } from '@angular/core';
import CustomStore from 'devextreme/data/custom_store';
import { environment } from '@environments/environment';
import { HttpClient } from '@angular/common/http';
import { CommonService } from '@app/shared/services'

@Component({
  selector: 'app-complex-grid',
  templateUrl: './complex-grid.component.html',
  styleUrls: ['./complex-grid.component.scss'],
})


export class ComplexGridComponent implements OnInit {

  isLoading: boolean = false;
  applicationCustomStore: CustomStore;    
  loadPanelPosition = { of: '#gridContainer' };

  currentFilter: any;
  applyFilterTypes: any;

  url: string = environment.apiUrl + '/applications';


  constructor(private http: HttpClient) {
    this.onShown = this.onShown.bind(this);
  }

  async ngOnInit() {

    this.applyFilterTypes = CommonService.applyFilterTypes;
    this.currentFilter = CommonService.currentFilter;

    this.isLoading = true;
    await this.setupDataSource();
    this.isLoading = false;

  }

  onShown() {
    setTimeout(() => {
      this.isLoading = false;
    }, 2000);
  }

  onContentReady(e: any) {
    e.component.option("loadPanel.enabled", this.isLoading);
  }

  async setupDataSource() {


    this.applicationCustomStore= new CustomStore({
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

        let url = this.url + "/" + key;


        return this.http.delete(url)
          .toPromise()
          .then(result => {
            console.log(result);
          });
      }
    });
  }


}
