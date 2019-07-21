import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { MatInputModule } from '@angular/material/input';
import { MatDialogModule, MAT_DIALOG_DEFAULT_OPTIONS } from '@angular/material/dialog';
import { MatCardModule, MatFormFieldModule, MatButtonModule } from '@angular/material';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';

import { RecipeElementComponent } from "./components/recipe-element/recipe-element.component";
import { HomeComponent } from "./components/home/home.component";
import { SearchComponent } from "./components/search/search.component";
import { RecipeTagComponent } from "./components/recipe-tag/recipe-tag.component";
import { RecipeDialogComponent } from "./components/dialogs/recipe-dialog/recipe-dialog.component";
import { AddRecipeDialogComponent } from "./components/dialogs/add-recipe-dialog/add-recipe-dialog.component";
import { EditRecipeDialogComponent } from "./components/dialogs/edit-recipe-dialog/edit-recipe-dialog.component";
import { DeleteRecipeDialogComponent } from "./components/dialogs/delete-recipe-dialog/delete-recipe-dialog.component";

import { RecipeService } from "./services/recipe.service";

@NgModule({
  declarations: [
    AppComponent,
    RecipeElementComponent,
    HomeComponent,
    SearchComponent,
    RecipeDialogComponent,
    AddRecipeDialogComponent,
    EditRecipeDialogComponent,
    DeleteRecipeDialogComponent,
    RecipeTagComponent
  ],
  entryComponents: [
    AddRecipeDialogComponent,
    EditRecipeDialogComponent,
    DeleteRecipeDialogComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    MatInputModule,
    MatDialogModule,
    MatCardModule,
    MatButtonModule,
    MatFormFieldModule,
    BrowserAnimationsModule,
    FormsModule,
    ReactiveFormsModule
  ],
  providers: [
    RecipeService,
    { provide: MAT_DIALOG_DEFAULT_OPTIONS, useValue: { hasBackdrop: true } }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
