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
     * Send the text from the input area to the
     * AutoList Api and place the total length
     * and total area into the corresponding
     * text areas
     * @param text The text from the textarea
     */
    OnInputAreaKeyUp(text:string) {
        console.log(text);
    }

    ngOnInit() {

    }

}
