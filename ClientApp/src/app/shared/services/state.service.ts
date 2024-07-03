import { Injectable, OnDestroy } from "@angular/core";
import { Observable, Subscription } from "rxjs";
import { environment } from "@environments/environment";
import { State } from "@app/models";
import { HttpClient} from "@angular/common/http";


@Injectable({ providedIn: 'root' })


export class StateService implements OnDestroy {
  private url = environment.apiUrl + '/states';
  private states$: Observable<State[]>;
  private subscription: Subscription;

  constructor(private http: HttpClient)   {  }
    ngOnDestroy(): void {
      this.subscription.unsubscribe();
  }


  getStates(): Observable<State[]> {

    this.states$ = this.http.get<State[]>(this.url);
    this.subscription = this.states$.subscribe(x => console.log(x));
    return this.states$;
  }

}
