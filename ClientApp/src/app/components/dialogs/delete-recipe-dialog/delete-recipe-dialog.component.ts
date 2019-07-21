import { Component, Inject, EventEmitter, Output } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';

@Component({
    selector: 'app-delete-recipe-dialog',
    templateUrl: './delete-recipe-dialog.component.html',
    styleUrls: ['./delete-recipe-dialog.component.scss']
})
/** delete-recipe-dialog component*/
export class DeleteRecipeDialogComponent {

  public recipeName: string;

  constructor(public dialogRef: MatDialogRef<DeleteRecipeDialogComponent>, @Inject(MAT_DIALOG_DATA) data) {
    this.recipeName = data["name"];
  }

  public delete() {
    this.dialogRef.close(true);
  }

  public cancel() {
    this.dialogRef.close(false);
  }
}
