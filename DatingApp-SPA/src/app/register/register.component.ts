import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  @Input () valuesFromHome: any;
  @Output() cancelRegister = new EventEmitter(); // All outputs emit an event which, during bubbling will get caught by the parent
  model: any = {};

  constructor() { }

  ngOnInit() {
  }

  register() {
    console.log(this.model);
  }

  cancel() {
    this.cancelRegister.emit(false); // Anything can be emitted, but simple false in this case.
    console.log('Canceled');
  }

}
