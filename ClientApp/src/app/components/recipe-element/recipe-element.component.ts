import { Component, Input, Output } from '@angular/core';
import { EventEmitter } from '@angular/core';

@Component({
    selector: 'app-recipe-element',
    templateUrl: './recipe-element.component.html',
    styleUrls: ['./recipe-element.component.scss']
})

export class RecipeElementComponent {

  @Input("name")
  public recipeName: string;

  @Input("path")
  public filePath: string;

  @Output() emitView: EventEmitter<string> = new EventEmitter();

  @Output() emitEdit: EventEmitter<string> = new EventEmitter();

  @Output() emitDelete: EventEmitter<string> = new EventEmitter();

  constructor() {
  }

  public viewRecipe() {
    this.emitView.emit(this.filePath);
  }

  public editRecipe() {
    this.emitEdit.emit(this.recipeName);
  }

  public deleteRecipe() {
    this.emitDelete.emit(this.recipeName);
  }

}
