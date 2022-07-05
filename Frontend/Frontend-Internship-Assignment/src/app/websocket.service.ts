import { Injectable } from '@angular/core';
import * as io from 'socket.io-client';
import { Observable, Subscriber } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class WebsocketService {
  socket: any;
  readonly uri: string = "ws://localhost:7032/message/chat";
  constructor() {
    this.socket = io.io(this.uri);
   }

  listen(eventName: string){
    return new Observable((subscriber) =>{
      this.socket.on(eventName, (data: any) =>{
        subscriber.next(data);
      })
    });
  }

  emit(eventName: string, data: any){
    this.socket.emit(eventName, data);
  }

}
