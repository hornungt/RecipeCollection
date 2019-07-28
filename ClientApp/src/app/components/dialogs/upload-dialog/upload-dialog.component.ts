import { Component, ChangeDetectorRef, ChangeDetectionStrategy } from '@angular/core';
import { MatDialogRef } from '@angular/material';
import { RecipeService } from '../../../services/recipe.service';

@Component({
  selector: 'app-upload-dialog',
  templateUrl: './upload-dialog.component.html',
  styleUrls: ['./upload-dialog.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush

})
/** upload-dialog component*/
export class UploadDialogComponent {

  public file: File;

  public nameElement: HTMLElement;

  constructor(
    public dialogRef: MatDialogRef<UploadDialogComponent>,
    public service: RecipeService) {
  }

  onFileSelected(event) {
    if (event.target.files.length == 1) {
      let nameField: HTMLInputElement = document.getElementById("name-input") as HTMLInputElement;
      this.file = event.target.files[0] as File;

      //this is really sketchy -> setting name using ngModel messes up the formatting so this is the easiest workaround
      //downside is that it is not using angular concepts
      nameField.value = this.file.name.split(".")[0];

      let fileLabel: HTMLParagraphElement = document.getElementById("file-name") as HTMLParagraphElement;
      fileLabel.textContent = this.file.name;
    }
  }

  submit() {
    //do checks
    // make service calls
  }

  //remember to assert that the name of the file is the same as the one given in the name input field with this.file.name = "..."

  cancel() {
    this.dialogRef.close();
  }


}
