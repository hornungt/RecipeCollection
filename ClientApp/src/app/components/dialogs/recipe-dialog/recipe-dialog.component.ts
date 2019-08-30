import { Component, Input } from '@angular/core';
import { MatDialogRef } from '@angular/material';
import { FormControl, Validators } from '@angular/forms';

@Component({
    selector: 'app-recipe-dialog',
    templateUrl: './recipe-dialog.component.html',
    styleUrls: ['./recipe-dialog.component.scss']
})
/** recipe-dialog component*/
export class RecipeDialogComponent {
/** recipe-dialog ctor */

  @Input()
  public dialogTitle: string;

  public name: string;
  public url: string;
  public tags: Array<string> = [];

  public nextTag: string;

  private urlPattern = new RegExp("(https?:\/\/(?:www\.|(?!www))[a-zA-Z0-9][a-zA-Z0-9-]+[a-zA-Z0-9]\.[^\s]{2,}|www\.[a-zA-Z0-9][a-zA-Z0-9-]+[a-zA-Z0-9]\.[^\s]{2,}|https?:\/\/(?:www\.|(?!www))[a-zA-Z0-9]+\.[^\s]{2,}|www\.[a-zA-Z0-9]+\.[^\s]{2,})");

  public urlValidator: FormControl = new FormControl('', [Validators.required, Validators.pattern(this.urlPattern)]);

  constructor(
    public dialogRef: MatDialogRef<RecipeDialogComponent>) {
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

  public submit() {
    if (this.urlValidator.valid) {
      this.dialogRef.close({ name: this.name, url: this.url, tags: this.tags });
    }
  }

  public cancel() {
    this.dialogRef.close();
  }

  public canSubmit(): boolean {
    let valid: boolean = false;
    if (this.name != null) {
      valid = this.name.length > 0 && this.urlValidator.valid;
      if (this.nextTag != null) {
        valid = valid && this.nextTag.length == 0;
      }
      // else -> this.nextTag has to be null which is fine, so valid can stay true
    }
    return valid;
  }
}
