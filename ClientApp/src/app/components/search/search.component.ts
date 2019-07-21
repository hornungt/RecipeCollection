import { Component, Output, EventEmitter } from '@angular/core';

@Component({
    selector: 'app-search',
    templateUrl: './search.component.html',
    styleUrls: ['./search.component.scss']
})
/** search component*/
export class SearchComponent {

  @Output() enter: EventEmitter<string> = new EventEmitter();

  constructor() {

  }

  // on enter
  onEnter(query) {
    this.enter.emit(query);
  }
}
