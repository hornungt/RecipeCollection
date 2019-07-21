import { Component, EventEmitter, Output, Input } from '@angular/core';

@Component({
    selector: 'app-recipe-tag',
    templateUrl: './recipe-tag.component.html',
    styleUrls: ['./recipe-tag.component.scss']
})
/** recipe-tag component*/
export class RecipeTagComponent {

  @Input() tag: string;

  @Output() removeTag: EventEmitter<string> = new EventEmitter<string>();

  emitRemove() {
    this.removeTag.emit(this.tag);
  }
}
