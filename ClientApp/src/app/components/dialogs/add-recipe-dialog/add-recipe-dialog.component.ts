import { Component, Input } from '@angular/core';
import { RecipeDialogComponent } from '../recipe-dialog/recipe-dialog.component';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material';

@Component({
    selector: 'app-add-recipe-dialog',
    templateUrl: '../recipe-dialog/recipe-dialog.component.html',
    styleUrls: ['../recipe-dialog/recipe-dialog.component.scss']
})
/** add-recipe-dialog component*/
export class AddRecipeDialogComponent extends RecipeDialogComponent {

  public title: string;

  constructor(public dialogRef: MatDialogRef<RecipeDialogComponent>) {
    super(dialogRef);
    this.dialogTitle = "New Recipe";
  }

}
