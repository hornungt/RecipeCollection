import { Component, Inject } from '@angular/core';
import { RecipeDialogComponent } from '../recipe-dialog/recipe-dialog.component';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { Recipe } from '../../../model/recipe';

@Component({
  selector: 'app-edit-recipe-dialog',
  templateUrl: '../recipe-dialog/recipe-dialog.component.html',
  styleUrls: ['../recipe-dialog/recipe-dialog.component.scss']
})
/** edit-recipe-dialog component*/
export class EditRecipeDialogComponent extends RecipeDialogComponent {

  public original: Recipe;

  constructor(public dialogRef: MatDialogRef<RecipeDialogComponent>, @Inject(MAT_DIALOG_DATA) data) {
    super(dialogRef);
    this.name = this.dialogTitle = data["name"];
    this.tags = data["tags"];
    this.url = data["url"];

    this.original = data as Recipe;
  }

  public submit() {
    let newRecipe = { name: this.name, path: null, url: this.url, tags: this.tags };
    if (this.urlValidator.valid && !this.recipeAreDeepEqual(this.original, newRecipe)) {
      this.dialogRef.close(newRecipe);
    }
  }

  private recipeAreDeepEqual(recipe1: Recipe, recipe2: Recipe): boolean {
    return recipe1.name == recipe2.name &&
      recipe1.path == recipe2.path &&
      this.arraysEqual(recipe1.tags, recipe2.tags) &&
      recipe1.url == recipe2.url;
  }

  private arraysEqual(array1, array2) {
    if (array1 === array2) return true;
    if (array1 == null || array2 == null) return false;
    if (array1.length != array2.length) return false;

    for (var i = 0; i < array1.length; ++i) {
      if (array1[i] !== array2[i]) return false;
    }
    return true;
  }
}
