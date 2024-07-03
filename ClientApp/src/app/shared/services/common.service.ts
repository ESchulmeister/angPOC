import { Injectable, OnInit } from "@angular/core";

@Injectable({ providedIn: 'root' })

export class CommonService implements OnInit   {

  currentFilter: any;
  applyFilterTypes: any;
  static applyFilterTypes: any;
  static currentFilter: any;

  constructor() { }


  ngOnInit() {
   this.applyFilterTypes = [{
      key: "auto",
      name: "Immediately"
    }];

    this.currentFilter = this.applyFilterTypes[0].key;


  }
  
 
}
