import { Component } from '@angular/core';
import { RecipeService } from '../../services/recipe.service';
import { MatDialog, MatDialogConfig } from '@angular/material';
import { AddRecipeDialogComponent } from '../dialogs/add-recipe-dialog/add-recipe-dialog.component';
import { Recipe } from "../../model/recipe";
import { EditRecipeDialogComponent } from '../dialogs/edit-recipe-dialog/edit-recipe-dialog.component';
import { DeleteRecipeDialogComponent } from '../dialogs/delete-recipe-dialog/delete-recipe-dialog.component';
import { InstructionsDialogComponent } from "../dialogs/instructions-dialog/instructions-dialog.component";
import { UploadDialogComponent } from '../dialogs/upload-dialog/upload-dialog.component';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})

export class HomeComponent {

  public recipes: Array<Recipe>;

  constructor(private service: RecipeService, private matDialog: MatDialog) { }

  showInstructions(): void {
    const dialogRef = this.matDialog.open(InstructionsDialogComponent);
  }

  openRecipe(filePath): void {
    this.service.getRecipeFile(filePath).subscribe(res => {
      const fileURL = URL.createObjectURL(res);
      window.open(fileURL, '_blank');
    });
  }

  search(query?) {
    this.service.getRecipes(query).subscribe(r => this.recipes = r);
  }

  openAddDialog() {
    const dialogConfig = new MatDialogConfig();
    dialogConfig.autoFocus = true;

    const dialogRef = this.matDialog.open(AddRecipeDialogComponent, dialogConfig);

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.service.addRecipe({ name: result["name"], tags: result["tags"], path: null, url: result["url"] })
          .subscribe(
            next => this.search(""),
            error => console.log(error));
      }
    });
  }

  openEditDialog(recipeName) {
    let toEdit = this.recipes.filter(recipe => recipe.name == recipeName);
    if (toEdit.length == 1) {
      const dialogConfig = new MatDialogConfig();
      dialogConfig.data = toEdit[0];

      const dialogRef = this.matDialog.open(EditRecipeDialogComponent, dialogConfig);

      dialogRef.afterClosed().subscribe(result => {
        if (result) {
          let newRecipe = result as Recipe;
          this.service.editRecipe(toEdit[0], newRecipe).subscribe(
            next => this.search(""),
            error => console.error(error));
        }
      });
    }
    else {
      console.error("Should not have multiple recipes with the same name");
    }
  }

  openDeleteDialog(recipeName) {
    let toDelete = this.recipes.filter(recipe => recipe.name == recipeName);
    if (toDelete.length == 1) {
      const dialogConfig = new MatDialogConfig();
      dialogConfig.data = { name: toDelete[0].name };

      const dialogRef = this.matDialog.open(DeleteRecipeDialogComponent, dialogConfig);

      dialogRef.afterClosed().subscribe(result => {
        if (result) {
          this.service.deleteRecipe(toDelete[0]).subscribe(r => {
            this.search("") // refresh the results
          }, error => console.error(error));
        }
      });
    }
  }

  openUploadDialog() {
    const dialogConfig = new MatDialogConfig();
    dialogConfig.autoFocus = true;
    const dialogRef = this.matDialog.open(UploadDialogComponent, dialogConfig);

  }
}
