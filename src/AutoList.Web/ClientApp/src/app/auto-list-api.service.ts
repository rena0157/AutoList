import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class AutoListApiService {

    constructor(private http: HttpClient) {

    }

    public SayHello():Observable<string> {
        return this.http.post('./api/AutoListApi/SayHello/', 'Adam', {responseType: 'text'});
    }

    public GetTotalLength(text:string):Observable<number> {
        return this.http.post<number>('./api/AutoListApi/GetTotalLength/', text);
    }

    public GetTotalArea(text:string):Observable<number> {
        return this.http.post<number>('./api/AutoListApi/GetTotalArea/', text);
    }

}
