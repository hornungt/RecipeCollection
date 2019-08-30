import { Component, ChangeDetectorRef, ChangeDetectionStrategy } from '@angular/core';
import { MatDialogRef } from '@angular/material';
import { RecipeService } from '../../../services/recipe.service';
import { Recipe } from '../../../model/recipe';

@Component({
  selector: 'app-upload-dialog',
  templateUrl: './upload-dialog.component.html',
  styleUrls: ['./upload-dialog.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush

})
/** upload-dialog component*/
export class UploadDialogComponent {

  public file: File;

  public nameField: HTMLInputElement;

  public nextTag: string;
  public tags: Array<string>;

  constructor(
    public dialogRef: MatDialogRef<UploadDialogComponent>) {
    this.tags = [];
  }

  onFileSelected(event) {
    if (event.target.files.length == 1) {
      this.nameField = document.getElementById("name-input") as HTMLInputElement;
      this.file = event.target.files[0] as File;

      //this is really sketchy -> setting name using ngModel messes up the formatting so this is the easiest workaround
      //downside is that it is not using angular concepts
      this.nameField.value = this.file.name.split(".")[0];

      let fileLabel: HTMLParagraphElement = document.getElementById("file-name") as HTMLParagraphElement;
      fileLabel.textContent = this.file.name;
    }
  }

  public pushNextTag() {
    if (this.nextTag && (this.tags.findIndex((t) => t == this.nextTag) == -1)) {
      this.tags.push(this.nextTag);
      this.nextTag = null;
    }
  }

  public removeTag(tag) {
    if (tag) {
      this.tags = this.tags.filter(t => t != tag);
    }
  }

  submit() {
    if (this.file && this.nameField.value.length > 0) {

      // keep the names the same between the user input and the file -> easier to search for later
      let fileBlob = this.file.slice(0, this.file.size, this.file.type);
      let renamedFile = new File([fileBlob], this.nameField.value + this.file.name.substring(this.file.name.lastIndexOf('.')));//this.file.name.split(".")[1]);

      let recipe: Recipe = { name: this.nameField.value, path: null, url: null, tags: this.tags };

      this.dialogRef.close({ recipe: recipe, file: renamedFile });
    }
  }

  cancel() {
    this.dialogRef.close();
  }


}
