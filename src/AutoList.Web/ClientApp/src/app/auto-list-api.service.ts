import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Block } from './Models/Block';

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

    public GetBlocksCsv(text:string) {
        return this.http.post('./api/AutoListApi/GetBlocksCsv', text, {responseType: 'text'});
    }

    public GetBlocksJson(text:string):Observable<Block> {
        return this.http.post<Block>('./api/AutoListApi/GetBlocksJson', text);
    }
}
