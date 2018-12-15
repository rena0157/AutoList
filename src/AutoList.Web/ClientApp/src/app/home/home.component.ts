import { Component, OnInit } from '@angular/core';
import { AutoListApiService } from '../auto-list-api.service';

@Component({
    selector: 'app-home',
    templateUrl: './home.component.html',
    styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {

    constructor(private _autoListApi:AutoListApiService) {

    }

    /**
     * The Total Length from the input
     */
    public TotalLength:number = 0.0;

    /**
     * The Total Area from the input
     */
    public TotalArea:number = 0.0;

    /**
     * The Linear unit for Display
     */
    public LinearUnit:string = "m";

    /**
     * The Squared Unit for Display
     */
    public SquareUnit:string = "m²";

    public ObjectsCsv:string;

    /**
     * Send the text from the input area to the
     * AutoList Api and place the total length
     * and total area into the corresponding
     * text areas
     * @param text The text from the textarea
     */
    OnInputAreaKeyUp(text:string):void {
        this._autoListApi.GetTotalLength(text)
            .subscribe(r => {
                this.TotalLength = r;
            });

        this._autoListApi.GetTotalArea(text)
            .subscribe(r => {
                this.TotalArea = r
            });
    }

    GetObjectsOnClick(text:string):void {
        this._autoListApi.GetBlocksCsv(text)
            .subscribe(r => {
                console.log(r);
            });

        this._autoListApi.GetBlocksJson(text)
            .subscribe(r => console.log(r));

    }

    ngOnInit() {

    }

}
